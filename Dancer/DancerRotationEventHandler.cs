using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Wotou.Common;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer
{
    public class DancerRotationEventHandler : IRotationEventHandler
    {
        private long _randomTime = 0;
        private Spell? _lastSpell = null;
        private DateTime _lastCastTime = DateTime.MinValue;
        private bool _hasDetected = false;

        private static Dictionary<Jobs, int> jobPriorities = new()
        {
            { Jobs.Viper, 1 },           // 蝰蛇剑士，优先级最高
            { Jobs.Pictomancer, 2 },     // 绘灵法师，高优先级支援职业
            { Jobs.Monk, 3 },            // 高输出近战职业
            { Jobs.BlackMage, 4 },       // 高输出法系职业
            { Jobs.Reaper, 5 },          // 强力近战职业
            { Jobs.Samurai, 6 },         // 高爆发近战职业
            { Jobs.Ninja, 7 },           // 兼具支援和爆发能力
            { Jobs.Dragoon, 8 },         // 稳定的近战输出
            { Jobs.RedMage, 9 },         // 灵活的法系职业
            { Jobs.Summoner, 10 },       // 持续高输出的召唤职业
            { Jobs.Machinist, 11 },      // 高爆发远程职业
            { Jobs.Bard, 12 },           // 兼具支援和输出的远程职业
            { Jobs.Dancer, 13 },         // 舞伴职业，低优先级自身
            { Jobs.BlueMage, 14 },       // 特殊职业，低优先级
            { Jobs.DarkKnight, 15 },     // 坦克职业
            { Jobs.Gunbreaker, 16 },     // 坦克职业
            { Jobs.Paladin, 17 },        // 坦克职业
            { Jobs.Warrior, 18 },        // 坦克职业
            { Jobs.Sage, 19 },           // 治疗职业
            { Jobs.WhiteMage, 20 },      // 治疗职业
            { Jobs.Scholar, 21 },        // 治疗职业
            { Jobs.Astrologian, 22 },    // 治疗职业
            { Jobs.Pugilist, 23 },       // 格斗家，基础职业，最高优先级
            { Jobs.Thaumaturge, 24 },    // 咒术师，基础职业，次高优先级
            { Jobs.Archer, 25 },         // 弓箭手，基础职业，第三优先级
            { Jobs.Rogue, 26 },          // 双剑师，基础职业，第四优先级
            { Jobs.Arcanist, 27 },       // 秘术师，基础职业，第五优先级
            { Jobs.Gladiator, 28 },      // 剑术师，基础职业
            { Jobs.Marauder, 29 },       // 斧术师，基础职业
            { Jobs.Lancer, 30 },         // 枪术师，基础职业
            { Jobs.Conjurer, 31 },       // 幻术师，基础职业
        };
        public void AfterSpell(Slot slot, Spell spell)
        {
            if (spell.Id == DancerDefinesData.Spells.TechnicalStep)
                DancerBattleData.Instance.TechnicalStepCount++;
            if (spell.Id == DancerDefinesData.Spells.DanceOfTheDawn)
                DancerBattleData.Instance.DanceOfTheDawnCount ++;
            var nowUtc   = DateTime.UtcNow;
            var battleMs = AI.Instance.BattleData.CurrBattleTimeInMs;
            var inWindow =
                _lastSpell != null &&
                _lastCastTime != DateTime.MinValue &&
                _lastSpell.IsAbility() &&
                spell.IsAbility() &&
                Core.Me.InCombat() &&
                battleMs > 0;

            if (DancerSettings.Instance.IsDailyMode == false &&!_hasDetected)
            {
                if (inWindow)
                {
                    if ((nowUtc - _lastCastTime).TotalMilliseconds > 620)
                        DancerBattleData.Instance.EnableThreeOGcd = false; // 保持原判断与赋值
                    _hasDetected = true;
                }
                _lastSpell    = spell;
                _lastCastTime = nowUtc;
            }
        }

        public void OnBattleUpdate(int currTimeInMs)
        {
            SmartUseHighPrioritySlot();
            SmartStop();
            
            if (LowVipRestrictor.IsLowVip() 
                && DancerSettings.Instance.IsDailyMode == false
                && (SettingMgr.GetSetting<GeneralSettings>().OptimizeGcd == false
                    || DancerBattleData.Instance.EnableThreeOGcd == false 
                    || SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3
                    || SettingMgr.GetSetting<GeneralSettings>().Ping > 10 
                    || SettingMgr.GetSetting<GeneralSettings>().Ping < 5)
                )
            {
                const int totalTimeoutMs = 120000;
                var timeLeft = totalTimeoutMs - currTimeInMs;
                if (timeLeft >= 0 && currTimeInMs - DancerBattleData.Instance.LastCountDownTime > 1000)
                {
                    ChatHelper.Print.ErrorMessage($"[警告] 你未按照要求设置 ACR, {(int)(timeLeft / 1000)} 秒后将爆炸！");
                    if (SettingMgr.GetSetting<GeneralSettings>().OptimizeGcd == false
                        || SettingMgr.GetSetting<GeneralSettings>().Ping > 10 
                        || SettingMgr.GetSetting<GeneralSettings>().Ping < 5)
                        ChatHelper.Print.ErrorMessage($"[警告] 请开启“优化 GCD 偏移”，并将数值设为 5 到 10（含 5 和 10）");
                    if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
                        ChatHelper.Print.ErrorMessage($"[警告] 请关闭全局能力技能不卡 GCD");
                    if (DancerBattleData.Instance.EnableThreeOGcd == false)
                    {
                        ChatHelper.Print.ErrorMessage($"[警告] 请开启 FuckAnimation 三插设置");
                        ChatHelper.Print.ErrorMessage($"[警告] 请检查当前模式（日随/高难），与副本是否匹配");
                        ChatHelper.Print.ErrorMessage($"[警告] 请检查你的网络延迟");
                    }
                    DancerBattleData.Instance.LastCountDownTime = currTimeInMs;
                }
                if (timeLeft <= 0)
                {
                    var number = new Random().Next(1, 17); // [1,17)，也就是 1 到 16
                    ChatHelper.SendMessage($"/e [爆炸] <se.{number}>");
                    // PlayerOptions.Instance.Stop = true;
                }
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

        public async void OnEnterRotation()
        {
            try
            {
                if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
                {
                    Core.Resolve<MemApiChatMessage>()
                        .Toast2("欢迎使用窝头的舞者ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
                    LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
                    ChatHelper.Print.ErrorMessage("[警告] 你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
                }
                else if (DancerSettings.Instance.WelcomeVoice)
                    Core.Resolve<MemApiChatMessage>()
                        .Toast2("欢迎使用窝头的舞者ACR", 1, 5000);
            } catch (MissingFieldException ex)
            {
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的舞者ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
                LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
                ChatHelper.Print.ErrorMessage("[警告] 你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
            }
            if (DancerSettings.Instance.WelcomeVoice)
                ChatHelper.SendMessage("/pdr tts 你好，欢迎你使用窝头舞者");
            try
            {
                await TimeLineUpdater.UpdateFiles("https://raw.githubusercontent.com/kanyeishere/ACR-Timeline/refs/heads/main/Wotou-DancerMaster.json", DancerSettings.Instance.SelectedTimeLinesForUpdate);
            }
            catch (Exception ex)
            {
                LogHelper.PrintError($"时间轴更新失败: {ex.Message}");
            }
            
            try
            {
                ECHelper.Commands.RemoveHandler("/Wotou_DNC");
            }
            catch (Exception) { }
            ECHelper.Commands.AddHandler("/Wotou_DNC", new CommandInfo(DancerCommandHandler));
        }
        
        public void OnExitRotation()
        {
            ECHelper.Commands.RemoveHandler("/Wotou_DNC");
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
                DancerRotationEntry.QT.SetQt(QTKey.SmartAoeTarget, true);
                
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
                    var nearbyEnemies = TargetHelper.GetNearbyEnemyCount(30);
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
                    // Core.Resolve<MemApiDuty>().InMission &&
                    !Core.Resolve<MemApiDuty>().IsOver &&
                    DancerDefinesData.Spells.ClosedPosition.IsUnlockWithCDCheck())
                {
                    // LogHelper.Print("寻找舞伴");
                    IBattleChara targetPlayer = PartyHelper.Party[^1];
                    foreach (var player in PartyHelper.Party)
                    {
                        if (player != Core.Me && player.IsTargetable && jobPriorities.TryGetValue(player.CurrentJob(), out var playerPriority) && playerPriority <= jobPriorities[targetPlayer.CurrentJob()])
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
            //DancerRotationEntry.QT.Reset();
            
            //重置扇舞保留层数
            DancerSettings.Instance.FanDanceSaveStack = 3;
            
            _randomTime = 0;
            _hasDetected = false;
            _lastSpell = null;
            _lastCastTime = DateTime.MinValue;
            
            // 根据用户的自定义设置或默认值，重置所有 QT
            foreach (var def in DancerQtHotkeyRegistry.Qts) // [CHANGED]
            {
                bool qtValue = DancerSettings.Instance.UserDefinedQtValues.TryGetValue(def.Key, out var customValue)
                    ? customValue
                    : def.Default;

                DancerRotationEntry.QT.SetQt(def.Key, qtValue);
            }

            if (DancerSettings.Instance.IsDailyMode)
            {
                DancerRotationEntry.QT.SetQt(QTKey.SmartAoeTarget, true);
            }
        }

        public void OnSpellCastSuccess(Slot slot, Spell spell)
        {
        }

        public void OnTerritoryChanged()
        {
            DancerRotationEntry.UpdateDancerPartnerPanel();
        }
        
        private void DancerCommandHandler(string command, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                LogHelper.Print("Wotou_DNC 命令无效，请提供参数。");
                return;
            }

            // 将参数转换为小写
            var lowerArgs = args.Trim().ToLower();

            // 检查是否是 QTKey + "_qt" 格式的命令
            if (lowerArgs.EndsWith("_qt"))
            {
                var keyPart = lowerArgs[..^3]; // 去除 "_qt"
                var matchedKey = DancerQtHotkeyRegistry.ParseQtKey(keyPart);
                ToggleQtSetting(matchedKey);
                return;
            }

            // 检查是否是 Hotkey + "_hk" 格式的命令
            else if (lowerArgs.EndsWith("_hk"))
            {
                string keyPart = lowerArgs[..^3]; // 去除 "_hk"
                if (DancerQtHotkeyRegistry.HotkeyAliasToId.TryGetValue(keyPart.ToLower(), out var id)) // [CHANGED]
                {
                    var resolver = DancerQtHotkeyRegistry.CreateResolver(id); // [CHANGED]
                    ExecuteHotkey(resolver);
                    return;
                }
                ChatHelper.SendMessage($"未知 Hotkey 参数: {keyPart}");
                return;
            }

            ChatHelper.SendMessage($"未知参数: {args}");
        }
        
        private void ExecuteHotkey(IHotkeyResolver? resolver)
        {
            if (resolver == null)
            {
                LogHelper.Print("快捷键解析器未正确初始化。");
                return;
            }

            if (resolver.Check() >= 0)
            {
                resolver.Run();
            }
            else
            {
                LogHelper.Print("无法执行该快捷键命令，可能条件不满足或技能不可用。");
            }
        }
        
        private void ToggleQtSetting(string qtKey)
        {
            bool currentValue = DancerRotationEntry.QT.GetQt(qtKey);
            DancerRotationEntry.QT.SetQt(qtKey, !currentValue);

            LogHelper.Print($"QT \"{qtKey}\" 已设置为 {(!currentValue).ToString().ToLower()}。");
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
        
        private void SmartStop()
        {
            if (!DancerSettings.Instance.IsDailyMode)
                return;
            if (Core.Me.GetCurrTarget() != null)
                PlayerOptions.Instance.Stop = false;
            PlayerOptions.Instance.Stop = !Core.Me.GetCurrTarget().NotInvulnerable();
        }
    }
}
