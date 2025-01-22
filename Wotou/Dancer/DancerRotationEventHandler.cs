using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
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
        private Dictionary<string, string> qtKeyDictionary;
        private Dictionary<string, string?> hotkeyDictionary;

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
            
            ECHelper.Commands.AddHandler("/Wotou_DNC", new CommandInfo(DancerCommandHandler));
            
            qtKeyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var fi in typeof(QTKey).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy))
            {
                if (fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                {
                    string value = fi.GetValue(null).ToString();
                    string key = value.ToLower();
                    string fieldName = fi.Name.ToLower();

                    // 添加中文键
                    if (!qtKeyDictionary.ContainsKey(key))
                    {
                        qtKeyDictionary.Add(key, value);
                    }

                    // 添加英文键
                    if (!qtKeyDictionary.ContainsKey(fieldName))
                    {
                        qtKeyDictionary.Add(fieldName, value);
                    }
                }
            }

            hotkeyDictionary = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                { "防击退", "ArmsLength" },
                { "armslength", "ArmsLength" },
                { "内丹", "SecondWind" },
                { "secondwind", "SecondWind" },
                { "桑巴", "ShieldSamba" },
                { "shieldsamba", "ShieldSamba" },
                { "华尔兹", "CuringWaltz" },
                { "curingwaltz", "CuringWaltz" },
                { "秒开关即兴", "Improvisation" },
                { "improvisation", "Improvisation" },
                { "疾跑", "Run" },
                { "run", "Run" },
                { "前冲步", "EnAvant" },
                { "enavant", "EnAvant" },
                { "爆发药", "Potion" },
                { "potion", "Potion" },
                { "极限技", "LimitBreak" },
                { "limitbreak", "LimitBreak" },
                { "停止自动移动", "StopMove" },
                { "stopmove", "StopMove" }
            };
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
            //DancerRotationEntry.QT.Reset();
            
            //重置扇舞保留层数
            DancerSettings.Instance.FanDanceSaveStack = 3;
            
            _randomTime = 0;
            
            // 根据用户的自定义设置或默认值，重置所有 QT
            foreach (var key in DancerSettings.Instance.DefaultQtValues.Keys)
            {
                // 从 CustomQtValues 获取用户配置的值；如果没有配置，则使用默认值
                bool qtValue = DancerSettings.Instance.CustomQtValues.TryGetValue(key, out var customValue)
                    ? customValue
                    : DancerSettings.Instance.DefaultQtValues[key].DefaultValue;

                DancerRotationEntry.QT.SetQt(key, qtValue);
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
                var keyPart = lowerArgs.Substring(0, lowerArgs.Length - 3); // 去除 "_qt"
                var matchedKey = GetMatchingQtKey(keyPart);
                ToggleQtSetting(matchedKey);
                return;
            }

            // 检查是否是 Hotkey + "_hk" 格式的命令
            else if (lowerArgs.EndsWith("_hk"))
            {
                string keyPart = lowerArgs.Substring(0, lowerArgs.Length - 3); // 去除 "_hk"
                if (hotkeyDictionary.TryGetValue(keyPart.ToLower(), out var matchedHotkey))
                {
                    ExecuteHotkey(GetHotkeyResolver(matchedHotkey));
                    return;
                }
                else
                {
                    ChatHelper.SendMessage($"未知 Hotkey 参数: {keyPart}");
                    return;
                }
            }

            ChatHelper.SendMessage($"未知参数: {args}");
        }
        
        private string GetMatchingQtKey(string keyPart)
        {
            string lowerKeyPart = keyPart.ToLower();
            if (qtKeyDictionary.TryGetValue(lowerKeyPart, out string matchedKey))
            {
                return matchedKey;
            }
            return null;
        }
        
        private IHotkeyResolver? GetHotkeyResolver(string? hotkey)
        {
            return hotkey switch
            {
                "ArmsLength" => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.ArmsLength, SpellTargetType.Target),
                "SecondWind" => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.SecondWind, SpellTargetType.Self),
                "ShieldSamba" => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.ShieldSamba, SpellTargetType.Self),
                "CuringWaltz" => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.CuringWaltz, SpellTargetType.Self),
                "Improvisation" => new ImprovisationHotkeyResolver(),
                "Run" => new HotKeyResolver_疾跑(),
                "EnAvant" => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.EnAvant, SpellTargetType.Target),
                "Potion" => new HotKeyResolver_Potion(),
                "LimitBreak" => new HotKeyResolver_LB(),
                "StopMove" => new StopMoveHotkeyResolver(),
                _ => null // 如果找不到匹配的 Hotkey，返回 null
            };
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
    }
}
