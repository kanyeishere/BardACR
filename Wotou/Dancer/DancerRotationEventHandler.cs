using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer
{
    public class DancerRotationEventHandler : IRotationEventHandler
    {
        private bool _originalValueForNoClipGcd3;
        private int _originalMaxAbilityTimesInGcd;
        
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
        }

        public void OnBattleUpdate(int currTimeInMs)
        {
            SmartUseHighPrioritySlot();
            if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
            {
                LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                ChatHelper.SendMessage("/e <se.1>");
            }
        }

        public void OnEnterRotation()
        {
            if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
            {
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的舞者ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
                LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                ChatHelper.SendMessage("/e <se.1>");
            }
            else
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的舞者ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
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
            else if (DancerRotationEntry.QT.GetQt("自动舞伴") && !Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition) && PartyHelper.Party.Count > 1 && DancerDefinesData.Spells.ClosedPosition.IsUnlock() && DancerDefinesData.Spells.ClosedPosition.GetSpell().IsReadyWithCanCast())
            {
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

        public async Task OnPreCombat()
        {
            DancerRotationEntry.UpdateDancerPartnerPanel();
            SmartUseHighPrioritySlot();
            
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
                
                if (DancerSettings.Instance.EnableAutoDancing)
                {
                    var nearbyEnemies = TargetHelper.GetNearbyEnemyCount(35);
                    if (nearbyEnemies > 0 && DancerDefinesData.Spells.StandardStep.GetSpell().IsReadyWithCanCast())
                    {
                        await DancerDefinesData.Spells.StandardStep.GetSpell().Cast();
                    }
                }
                
                if (DancerSettings.Instance.EnableAutoPeloton && !Core.Resolve<JobApi_Dancer>().IsDancing && !Core.Me.InCombat())
                {
                    if ((!Core.Me.HasAura(DancerDefinesData.Buffs.Peloton) ||
                         !Core.Me.HasMyAuraWithTimeleft(DancerDefinesData.Buffs.Peloton, 4000)) && 
                        !Core.Me.InCombat())
                    {
                        await Task.Delay(new Random().Next(1000, 3000));
                        if ((!Core.Me.HasAura(DancerDefinesData.Buffs.Peloton) ||
                            !Core.Me.HasMyAuraWithTimeleft(DancerDefinesData.Buffs.Peloton, 4000)) && 
                           !Core.Me.InCombat()) 
                            await DancerDefinesData.Spells.Peloton.GetSpell().Cast();
                    }
                    
                }
            }
        }

        public void OnResetBattle()
        {
            DancerRotationEntry.UpdateDancerPartnerPanel();
            // 重置战斗中缓存的数据
            DancerBattleData.Instance = new DancerBattleData();
            DancerRotationEntry.QT.Reset();
            
            //重置扇舞保留层数
            DancerSettings.Instance.FanDanceSaveStack = 3;
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
