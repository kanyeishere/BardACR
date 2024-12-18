using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Common;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer
{
    public class DancerRotationEventHandler : IRotationEventHandler
    {
        private long _randomTime = 0;

        private static Dictionary<Jobs, int> jobPriorities = new(){
            { Jobs.Viper, 1 },
            { Jobs.Monk, 2 },
            { Jobs.Pictomancer, 3 },
            { Jobs.BlackMage, 4 },
            { Jobs.Reaper, 5 },
            { Jobs.Samurai, 6},
            { Jobs.Ninja, 7},
            { Jobs.Summoner, 8},
            { Jobs.Dragoon, 9},
            { Jobs.Machinist, 10},
            { Jobs.Bard, 11},
            { Jobs.Dancer, 12},
            { Jobs.DarkKnight, 13},
            { Jobs.Gunbreaker, 14},
            { Jobs.Paladin, 15},
            { Jobs.Warrior, 16},
            { Jobs.Sage, 17},
            { Jobs.WhiteMage, 18},
            { Jobs.Scholar, 19},
            { Jobs.Astrologian, 20}
        };
        public void AfterSpell(Slot slot, Spell spell)
        {
            if (spell.Id == DancerDefinesData.Spells.TechnicalStep)
                DancerBattleData.Instance.TechnicalStepCount++;
            if (spell.Id == DancerDefinesData.Spells.DanceOfTheDawn)
                DancerBattleData.Instance.DanceOfTheDawnCount ++;
        }

        public void OnBattleUpdate(int currTimeInMs)
        {
            SmartUseHighPrioritySlot();
            if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
            {
                LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                ChatHelper.SendMessage("/e 警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                ChatHelper.SendMessage("/e <se.1>");
            }
            if (DancerBattleData.Instance.LastWarningTime == 0)
                DancerBattleData.Instance.LastWarningTime = currTimeInMs;
            // 检查舞步的CD和距离，并发送提醒
            var standardStepSpell = Core.Resolve<MemApiSpell>().CheckActionChange(DancerDefinesData.Spells.StandardStep).GetSpell();
            var technicalStepSpell = DancerDefinesData.Spells.TechnicalStep.GetSpell();
            var target = Core.Me.GetCurrTarget();
            if (((standardStepSpell.Cooldown.TotalMilliseconds < 5000 && 
                  standardStepSpell.IsUnlock() &&
                  DancerRotationEntry.QT.GetQt(QTKey.StandardStep)) ||
                 (technicalStepSpell.Cooldown.TotalMilliseconds < 5000 &&
                  technicalStepSpell.IsUnlock() &&
                  DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))) &&
                target != null && 
                Core.Me.Distance(target) > 15 &&
                DancerSettings.Instance.DanceDistanceWarning 
                &&
                currTimeInMs - DancerBattleData.Instance.LastWarningTime >= 10000
                ) // 10秒内不重复提醒
            {
                DancerBattleData.Instance.LastWarningTime = currTimeInMs; // 更新提醒时间
                ChatHelper.SendMessage("/pdr tts 舞步即将释放，请注意距离");
                ChatHelper.SendMessage("/e 舞步即将释放，请注意距离");
            }
        }

        public void OnEnterRotation()
        {
            try
            {
                if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
                {
                    Core.Resolve<MemApiChatMessage>()
                        .Toast2("欢迎使用窝头的舞者ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
                    LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                    ChatHelper.SendMessage("/e 警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                    ChatHelper.SendMessage("/e <se.1>");
                }
                else if (DancerSettings.Instance.WelcomeVoice)
                    Core.Resolve<MemApiChatMessage>()
                        .Toast2("欢迎使用窝头的舞者ACR", 1, 5000);
            } catch (MissingFieldException ex)
            {
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的舞者ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
                LogHelper.PrintError("警告，你没有进行全局能力技能不卡GCD设置，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中先开启全局能力技能不卡GCD后，再重新关闭一次");
                ChatHelper.SendMessage("/e 警告，你没有进行全局能力技能不卡GCD设置，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中先开启全局能力技能不卡GCD后，再重新关闭一次");
                ChatHelper.SendMessage("/e <se.1>");
            }
            if (DancerSettings.Instance.WelcomeVoice)
                ChatHelper.SendMessage("/pdr tts 你好，欢迎你使用窝头舞者");
            TimeLineUpdater.UpdateFiles("https://raw.githubusercontent.com/kanyeishere/ACR-Timeline/refs/heads/main/Wotou-DancerMaster.json");
        }

        public void OnExitRotation()
        {
        }

        public async Task OnNoTarget()
        {
            if (Core.Resolve<JobApi_Dancer>().IsDancing)
            {
                if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.StandardStep))
                {
                    if (Core.Resolve<JobApi_Dancer>().CompleteSteps < 2)
                    {
                        await DancerUtil.GetStep().Cast();
                    }
                    else if (DancerRotationEntry.QT.GetQt("小舞") && TargetHelper.GetNearbyEnemyCount(10) > 0 && AI.Instance.BattleData.CurrBattleTimeInMs > 0)
                    {
                        await DancerDefinesData.Spells.DoubleStandardFinish.GetSpell().Cast();
                    }

                }
                else if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.TechnicalStep))
                {
                    if (Core.Resolve<JobApi_Dancer>().CompleteSteps < 4)
                    {
                        await DancerUtil.GetStep().Cast();
                    }
                    else if (DancerRotationEntry.QT.GetQt("大舞") && TargetHelper.GetNearbyEnemyCount(10) > 0 && AI.Instance.BattleData.CurrBattleTimeInMs > 0)
                    {
                        await DancerDefinesData.Spells.QuadrupleTechnicalFinish.GetSpell().Cast();
                    }

                }
            }
        }

        public async Task OnPreCombat()
        {
            DancerRotationEntry.UpdateDancerPartnerPanel();
            SmartUseHighPrioritySlot();
            
            if (Core.Me.IsMoving())
                Core.Resolve<MemApiMove>().CancelMove();
            if (Core.Resolve<MemApiMove>().IsMoving())
                Core.Resolve<MemApiMove>().CancelMove();
            
            if (DancerSettings.Instance.IsDailyMode)
            {
                
                if (Core.Resolve<JobApi_Dancer>().IsDancing && DancerSettings.Instance.EnableAutoDancing)
                {
                    if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.StandardStep))
                    {
                        if (Core.Resolve<JobApi_Dancer>().CompleteSteps < 2)
                        {
                            await DancerUtil.GetStep().Cast();
                        }
                        else if (DancerRotationEntry.QT.GetQt("小舞") && TargetHelper.GetNearbyEnemyCount(10) > 0 && AI.Instance.BattleData.CurrBattleTimeInMs > 0)
                        {
                            await DancerDefinesData.Spells.DoubleStandardFinish.GetSpell().Cast();
                        }

                    }
                    else if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.TechnicalStep))
                    {
                        if (Core.Resolve<JobApi_Dancer>().CompleteSteps < 4)
                        {
                            await DancerUtil.GetStep().Cast();
                        }
                        else if (DancerRotationEntry.QT.GetQt("大舞") && TargetHelper.GetNearbyEnemyCount(10) > 0 && AI.Instance.BattleData.CurrBattleTimeInMs > 0)
                        {
                            await DancerDefinesData.Spells.QuadrupleTechnicalFinish.GetSpell().Cast();
                        }

                    }
                }
                
                if (DancerSettings.Instance.EnableAutoDancing && Core.Resolve<MemApiDuty>().IsBoundByDuty())
                {
                    var nearbyEnemies = TargetHelper.GetNearbyEnemyCount(35);
                    if (nearbyEnemies > 0 && DancerDefinesData.Spells.StandardStep.GetSpell().IsReadyWithCanCast())
                    {
                        await DancerDefinesData.Spells.StandardStep.GetSpell().Cast();
                    }
                }
                
                if (DancerSettings.Instance.EnableAutoPeloton && 
                    !Core.Resolve<JobApi_Dancer>().IsDancing && 
                    !Core.Me.InCombat() && 
                    Core.Me.IsMoving() && 
                    !DancerDefinesData.Spells.Peloton.RecentlyUsed(5000))
                {
                    if ((!Core.Me.HasAura(DancerDefinesData.Buffs.Peloton) || 
                         !Core.Me.HasMyAuraWithTimeleft(DancerDefinesData.Buffs.Peloton, 4000)) && 
                        !Core.Me.InCombat() && Core.Me.IsMoving())
                    {
                        if (_randomTime == 0 || TimeHelper.Now() > _randomTime)
                            await DancerDefinesData.Spells.Peloton.GetSpell().Cast();
                    }
                    else
                        _randomTime = TimeHelper.Now() + RandomHelper.RandomInt(1000, 2000);
                }
                
                if (!Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition) && 
                    PartyHelper.Party.Count > 1 && 
                    DancerSettings.Instance.EnableAutoDancePartner && 
                    DancerDefinesData.Spells.ClosedPosition.IsUnlockWithCDCheck())
                {
                    LogHelper.Print("寻找舞伴");
                    IBattleChara targetPlayer = PartyHelper.Party[^1];
                    foreach (var player in PartyHelper.Party)
                    {
                        if (player != Core.Me && jobPriorities.TryGetValue(player.CurrentJob(), out var playerPriority) && playerPriority <= jobPriorities[targetPlayer.CurrentJob()])
                        {
                            if (targetPlayer.CurrentJob() == player.CurrentJob() && targetPlayer.MaxHp >= player.MaxHp)
                            {
                                break;
                            }
                            else
                            {
                                targetPlayer = player;
                            }
                        }
                    }
                    if (!targetPlayer.IsDead) {
                        await new Spell(DancerDefinesData.Spells.ClosedPosition,targetPlayer).Cast();
                    }
                }
            }
        }

        public void OnResetBattle()
        {
            DancerRotationEntry.UpdateDancerPartnerPanel();
            DancerBattleData.Instance = new DancerBattleData();
            DancerRotationEntry.QT.Reset();
            
            //重置扇舞保留层数
            DancerSettings.Instance.FanDanceSaveStack = 3;
            
            _randomTime = 0;
        }

        public void OnSpellCastSuccess(Slot slot, Spell spell)
        {

        }

        public void OnTerritoryChanged()
        {
            DancerRotationEntry.UpdateDancerPartnerPanel();
        }

        private void SmartUseHighPrioritySlot()
        {
            if (Core.Resolve<MemApiCondition>().IsInCombat() && 
                Core.Me.GetCurrTarget() != null &&
                Core.Me.GetCurrTarget().CanAttack())
                DancerBattleData.Instance.HotkeyUseHighPrioritySlot = true;
            else
                DancerBattleData.Instance.HotkeyUseHighPrioritySlot = false;

        }
    }
}
