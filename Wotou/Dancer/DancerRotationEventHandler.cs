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
        }

        public void OnEnterRotation()
        {
            // 处理全局能力技不卡GCD
            _originalValueForNoClipGcd3 = SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3;
            _originalMaxAbilityTimesInGcd = SettingMgr.GetSetting<GeneralSettings>().MaxAbilityTimesInGcd;
            SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3 = false;
            SettingMgr.GetSetting<GeneralSettings>().MaxAbilityTimesInGcd = 2;
        }

        public void OnExitRotation()
        {
            SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3 = _originalValueForNoClipGcd3;
            SettingMgr.GetSetting<GeneralSettings>().MaxAbilityTimesInGcd = _originalMaxAbilityTimesInGcd;
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
                if (DancerSettings.Instance.EnableAutoPeloton && !Core.Resolve<JobApi_Dancer>().IsDancing)
                {
                    await Task.Delay(new Random().Next(600, 3000));
                    if (!Core.Me.HasAura(DancerDefinesData.Buffs.Peloton) || !Core.Me.HasMyAuraWithTimeleft(DancerDefinesData.Buffs.Peloton, 4000))
                        await DancerDefinesData.Spells.Peloton.GetSpell().Cast();
                    
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
