using System.Numerics;
using System.Text.RegularExpressions;
using Wotou.Bard.Setting;
using Wotou.Bard.Data;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Wotou.Bard.Utility;
using Wotou.Common;

namespace Wotou.Bard;

/// <summary>
/// 事件回调处理类 参考接口里的方法注释
/// </summary>
public class BardRotationEventHandler : IRotationEventHandler
{
    private long _randomTime = 0;
    private int _lastNotifyTime = 0;
    private static void HandleMovingToTarget()
    {
        var instanceTargetPosition = BardBattleData.Instance.TargetPosition;
        if (instanceTargetPosition == null) return;
        const float offset = 0.05f;
        var targetPosition = (Vector3)instanceTargetPosition;
            
        Core.Resolve<MemApiMove>().MoveToTarget(targetPosition);
            
        var currentPos = Core.Me.Position;
            
        float dx = targetPosition.X - currentPos.X;
        float dz = targetPosition.Z - currentPos.Z;
        float distance = MathF.Sqrt(dx * dx + dz * dz);
            
        if (distance > offset)
        {
            Core.Resolve<MemApiMove>().MoveToTarget(targetPosition);
        }
        else
        {
            // 到达目标位置，停止移动
            Core.Resolve<MemApiMove>().CancelMove();
            BardBattleData.Instance.TargetPosition = null; // 清除目标位置
        }
    }
    
    public async Task OnPreCombat()
    {
        BardRotationEntry.UpdateWardensPaeanPanel();
        SmartUseHighPrioritySlot();
        CancelMoving();
        // HandleMovingToTarget();
        
        /*if (LowVipRestrictor.IsRestrictedZoneForLowVip())
        {
            if (BardSettings.Instance.QwertyList.Count <= 7)
            {
                BardSettings.Instance.QwertyList = PartyHelper.Party.Select(player => LowVipRestrictor.ComputeMd5Hash(player.Name.ToString())).ToList();
                BardSettings.Instance.Save();
            }
        }*/
        
        /*if (LowVipRestrictor.IsRestrictedZoneForLowVip() && !LowVipRestrictor.IsInStaticParty(BardSettings.Instance.QwertyList))
            PlayerOptions.Instance.Stop = true;*/
        
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
        
        if (BardSettings.Instance.IsDailyMode)
        {
            BardRotationEntry.QT.SetQt("强对齐", false);
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("攒碎心箭", false);
            BardRotationEntry.QT.SetQt(QTKey.SmartAoeTarget, true);
            
            if (BardSettings.Instance.EnableAutoPeloton && 
                Core.Me.IsMoving() &&
                !Core.Me.InCombat() && 
                !BardDefinesData.Spells.Peloton.RecentlyUsed(5000))
            {
                if ((!Core.Me.HasAura(BardDefinesData.Buffs.Peloton) || 
                     !Core.Me.HasMyAuraWithTimeleft(BardDefinesData.Buffs.Peloton, 4000)) && 
                    !Core.Me.InCombat() && Core.Me.IsMoving())
                {
                    if (_randomTime == 0 || TimeHelper.Now() > _randomTime)
                        await BardDefinesData.Spells.Peloton.GetSpell().Cast();
                }
                else
                    _randomTime = TimeHelper.Now() + RandomHelper.RandomInt(1000, 2000);
            }
        }
        
        /*if (PartyHelper.CastableParty.Any(characterAgent => 
                (characterAgent.HasAura(1896U) && BardSettings.Instance.NaturesMinneWithRecitation) ||  //秘策
                (characterAgent.HasAura(2611U) && BardSettings.Instance.NaturesMinneWithZoe) ||         //活化
                (characterAgent.HasAura(1892U) && BardSettings.Instance.NaturesMinneWithNeutralSect)) &&   //中间学派
            BardDefinesData.Spells.NaturesMinne.IsUnlockWithCDCheck() &&
            BardRotationEntry.QT.GetQt(QTKey.NatureMinne))
            await BardDefinesData.Spells.NaturesMinne.GetSpell().Cast();*/
    }

    public void OnResetBattle()
    {
        BardRotationEntry.UpdateWardensPaeanPanel();
        SmartUseHighPrioritySlot();
        CancelMoving();
        //BardRotationEntry.QT.Reset();
        BardBattleData.Instance = new BardBattleData();
        
        // 重置碎心箭保留层数
        BardSettings.Instance.HeartBreakSaveStack = 0; 
        
        _randomTime = 0;
        
        if (BardSettings.Instance.ResetSongOrder)
            BardUtil.ResetSongOrderCustom();
        
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
        
        // 重置 QT 值
        foreach (var def in BardQtHotkeyRegistry.Qts)
        {
            bool v = BardSettings.Instance.UserDefinedQtValues.TryGetValue(def.Key, out var saved)
                ? saved
                : def.Default;

            // 这里按需触发回调，让逻辑实时生效
            BardRotationEntry.QT.SetQt(def.Key, v);
        }
        
        if (BardSettings.Instance.IsDailyMode)
        {
            BardRotationEntry.QT.SetQt("强对齐", false);
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("攒碎心箭", false);
            BardRotationEntry.QT.SetQt(QTKey.SmartAoeTarget, true);
        }
    }

    public async Task OnNoTarget()
    {
        // 无目标切歌
        if (BardSongSwitchUtil.CanSwitchFromFirstToSecond())
            await BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().Cast();

        if (BardSongSwitchUtil.CanSwitchFromSecondToThird())
            await BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().Cast();

        if (BardSongSwitchUtil.CanSwitchFromThirdToFirst())
            await BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().Cast();

        if (BardSongSwitchUtil.CanSwitchFromNone())
        {
            if (BardBattleData.Instance.LastSong == BardSettings.Instance.FirstSong && 
                BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().IsReadyWithCanCast())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().Cast();
                return;
            }
            if (BardBattleData.Instance.LastSong == BardSettings.Instance.SecondSong && 
                BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().IsReadyWithCanCast())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().Cast();
                return;
            }
            if (BardBattleData.Instance.LastSong == BardSettings.Instance.ThirdSong && 
                BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().IsReadyWithCanCast())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().Cast();
                return;
            }
            
            if (BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().IsReadyWithCanCast())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().Cast();
                return;
            }
            if (BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().IsReadyWithCanCast())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().Cast();
                return;
            }
            if (BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().IsReadyWithCanCast())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().Cast();
                return;
            }
        }
        
        if (PartyHelper.CastableParty.Any(characterAgent => 
                (characterAgent.HasAura(1896U) && BardSettings.Instance.NaturesMinneWithRecitation) ||  //秘策
                (characterAgent.HasAura(2611U) && BardSettings.Instance.NaturesMinneWithZoe) ||         //活化
                (characterAgent.HasAura(1892U) && BardSettings.Instance.NaturesMinneWithNeutralSect)) &&   //中间学派
            BardDefinesData.Spells.NaturesMinne.IsUnlockWithCDCheck() &&
            BardRotationEntry.QT.GetQt(QTKey.NatureMinne))
            await BardDefinesData.Spells.NaturesMinne.GetSpell().Cast();
        
        await Task.CompletedTask;
    }

    public void OnSpellCastSuccess(Slot slot, Spell spell)
    {
       
    }

    public void AfterSpell(Slot slot, Spell spell)
    {
        if (spell.Id == BardDefinesData.Spells.IronJaws && BardUtil.HasAllPartyBuff())
            BardBattleData.Instance.HasUseIronJawsInCurrentBursting = true;
        if (spell.Id == BardDefinesData.Spells.ApexArrow && BardUtil.HasNoPartyBuff())
            BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod = true;
        if (BardBattleData.Instance.TotalStopTime > 0 &&
            (spell.Id == Core.Resolve<MemApiSpell>().CheckActionChange(BardDefinesData.Spells.BurstShot)|| 
             spell.Id == Core.Resolve<MemApiSpell>().CheckActionChange(BardDefinesData.Spells.RefulgentArrow)||
             spell.Id == Core.Resolve<MemApiSpell>().CheckActionChange(BardDefinesData.Spells.Ladonsbite)||
             spell.Id == Core.Resolve<MemApiSpell>().CheckActionChange(BardDefinesData.Spells.Shadowbite)))
            BardBattleData.Instance.GcdCountDown --;
        // 没有记录到第一个120秒的buff，就记录下来
        if (!BardBattleData.Instance.HasFirst120SBuff)
            switch (spell.Id)
            {
                case BardDefinesData.Spells.RagingStrikes:
                    BardBattleData.Instance.HasFirst120SBuff = true;
                    BardBattleData.Instance.First120SBuffSpellId = BardDefinesData.Spells.RagingStrikes;
                    BardBattleData.Instance.First120SBuffId = BardDefinesData.Buffs.RagingStrikes;
                    break;
                case BardDefinesData.Spells.BattleVoice:
                    BardBattleData.Instance.HasFirst120SBuff = true;
                    BardBattleData.Instance.First120SBuffSpellId = BardDefinesData.Spells.BattleVoice;
                    BardBattleData.Instance.First120SBuffId = BardDefinesData.Buffs.BattleVoice;
                    break;
                case BardDefinesData.Spells.RadiantFinale:
                    BardBattleData.Instance.HasFirst120SBuff = true;
                    BardBattleData.Instance.First120SBuffSpellId = BardDefinesData.Spells.RadiantFinale;
                    BardBattleData.Instance.First120SBuffId = BardDefinesData.Buffs.RadiantFinale;
                    break;
            }
        else if (!BardBattleData.Instance.HasSecond120SBuff)
            switch (spell.Id)
            {
                case BardDefinesData.Spells.RagingStrikes:
                    BardBattleData.Instance.HasSecond120SBuff = true;
                    BardBattleData.Instance.Second120SBuffSpellId = BardDefinesData.Spells.RagingStrikes;
                    BardBattleData.Instance.Second120SBuffId = BardDefinesData.Buffs.RagingStrikes;
                    break;
                case BardDefinesData.Spells.BattleVoice:
                    BardBattleData.Instance.HasSecond120SBuff = true;
                    BardBattleData.Instance.Second120SBuffSpellId = BardDefinesData.Spells.BattleVoice;
                    BardBattleData.Instance.Second120SBuffId = BardDefinesData.Buffs.BattleVoice;
                    break;
                case BardDefinesData.Spells.RadiantFinale:
                    BardBattleData.Instance.HasSecond120SBuff = true;
                    BardBattleData.Instance.Second120SBuffSpellId = BardDefinesData.Spells.RadiantFinale;
                    BardBattleData.Instance.Second120SBuffId = BardDefinesData.Buffs.RadiantFinale;
                    break;
            }
        else if (!BardBattleData.Instance.HasThird120SBuff)
            switch (spell.Id)
            {
                case BardDefinesData.Spells.RagingStrikes:
                    BardBattleData.Instance.HasThird120SBuff = true;
                    BardBattleData.Instance.Third120SBuffSpellId = BardDefinesData.Spells.RagingStrikes;
                    BardBattleData.Instance.Third120SBuffId = BardDefinesData.Buffs.RagingStrikes;
                    break;
                case BardDefinesData.Spells.BattleVoice:
                    BardBattleData.Instance.HasThird120SBuff = true;
                    BardBattleData.Instance.Third120SBuffSpellId = BardDefinesData.Spells.BattleVoice;
                    BardBattleData.Instance.Third120SBuffId = BardDefinesData.Buffs.BattleVoice;
                    break;
                case BardDefinesData.Spells.RadiantFinale:
                    BardBattleData.Instance.HasThird120SBuff = true;
                    BardBattleData.Instance.Third120SBuffSpellId = BardDefinesData.Spells.RadiantFinale;
                    BardBattleData.Instance.Third120SBuffId = BardDefinesData.Buffs.RadiantFinale;
                    break;
            }
        
        
        BardBattleData.Instance.LastSong = spell.Id switch
        {
            BardDefinesData.Spells.TheWanderersMinuet => Song.Wanderer,
            BardDefinesData.Spells.MagesBallad => Song.Mage,
            BardDefinesData.Spells.ArmysPaeon => Song.Army,
            _ => BardBattleData.Instance.LastSong
        };
        
        if (spell.Id == BardDefinesData.Spells.TheWanderersMinuet ||
            spell.Id == BardDefinesData.Spells.MagesBallad ||
            spell.Id == BardDefinesData.Spells.ArmysPaeon)
            BardBattleData.Instance.LastSongTime = AI.Instance.BattleData.CurrBattleTimeInMs;
        
        if (spell.Id == BardDefinesData.Spells.RagingStrikes)
            BardBattleData.Instance.HasUseIronJawsInCurrentBursting = false;
        
        if (spell.Id == BardDefinesData.Spells.BattleVoice)
            BardUtil.LogDebug("GcdCooldown 3: ",GCDHelper.GetGCDCooldown().ToString());
    }

    public void OnBattleUpdate(int currTimeInMs)
    {
        SmartUseHighPrioritySlot();
        HandleMovingToTarget();
        
        /*if (LowVipRestrictor.IsRestrictedZoneForLowVip() && !LowVipRestrictor.IsInStaticParty(BardSettings.Instance.QwertyList))
            PlayerOptions.Instance.Stop = true;*/
        
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
        if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3 && currTimeInMs - _lastNotifyTime > 1000)
        {
            LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
            ECHelper.Chat.PrintError("/e 警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
            _lastNotifyTime = currTimeInMs;
        }
    }

    public async void OnEnterRotation()
    {
        try
        {
            if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
            {
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的诗人ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
                LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
                ECHelper.Chat.PrintError("/e 警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
            }
            else if (BardSettings.Instance.WelcomeVoice)
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的诗人ACR", 1, 5000);
        } catch (MissingFieldException ex)
        {
            Core.Resolve<MemApiChatMessage>()
                .Toast2("欢迎使用窝头的诗人ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
            LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
            ECHelper.Chat.PrintError("/e 警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭 <se.1>");
        }
        if (BardSettings.Instance.WelcomeVoice)
            ChatHelper.SendMessage("/pdr tts 你好，欢迎你使用窝头诗人");
        try
        {
            await TimeLineUpdater.UpdateFiles("https://raw.githubusercontent.com/kanyeishere/ACR-Timeline/refs/heads/main/Wotou-BardMaster.json", BardSettings.Instance.SelectedTimeLinesForUpdate);
        }
        catch (Exception ex)
        {
            LogHelper.PrintError($"时间轴更新失败: {ex.Message}");
        }
        
        try
        {
            ECHelper.Commands.RemoveHandler("/Wotou_BRD");
        }
        catch (Exception) { }
        ECHelper.Commands.AddHandler("/Wotou_BRD", new CommandInfo(BardCommandHandler));
    }
    
    private void BardCommandHandler(string command, string args)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            LogHelper.Print("Wotou_BRD 命令无效，请提供参数。");
            return;
        }
        
        // 将参数转换为小写，以实现不区分大小写的匹配
        var lowerArgs = args.Trim().ToLower();

        // 检查是否是 QTKey + "_qt" 格式的命令
        if (lowerArgs.EndsWith("_qt"))
        {
            // 提取 QTKey 部分，保持原始大小写以匹配 QTKey 常量
            var keyPart = lowerArgs.Substring(0, lowerArgs.Length - 3); // 去除 "_qt"

            // 尝试匹配 QTKey 常量
            var matchedKey = BardQtHotkeyRegistry.ParseQtKey(keyPart);
            if (matchedKey != null)
                ToggleQtSetting(matchedKey);
            return;
        }

        // 检查是否是 Hotkey + "_hk" 格式的命令
        else if (lowerArgs.EndsWith("_hk"))
        {
            var keyPart = lowerArgs[..^3]; // 去掉 "_hk"
            if (BardQtHotkeyRegistry.HotkeyAliasToId.TryGetValue(keyPart.ToLower(), out var id))
            {
                ExecuteHotkey(BardQtHotkeyRegistry.CreateResolver(id));
                return;
            }
            ChatHelper.SendMessage($"未知 Hotkey 参数: {keyPart}");
            return;
        }

        // 处理其他命令
        switch (lowerArgs)
        {
            case "hello":
                ChatHelper.SendMessage("你好！这是一条测试消息！");
                break;
            case var move when move.StartsWith("moveto", StringComparison.OrdinalIgnoreCase):
            {
                // 匹配格式 moveTo (x,y,z) 或 moveTo(x,y,z)（可带空格）
                var match = Regex.Match(move, @"moveto\s*\(\s*(-?\d+(\.\d+)?)\s*,\s*(-?\d+(\.\d+)?)\s*,\s*(-?\d+(\.\d+)?)\s*\)(?:\s+delay\s+(\d+))?", RegexOptions.IgnoreCase);
                
                if (match.Success)
                {
                    float x = float.Parse(match.Groups[1].Value);
                    float y = float.Parse(match.Groups[3].Value);
                    float z = float.Parse(match.Groups[5].Value);
                    
                    var delay = 0;
                    if (match.Groups[7].Success)
                        delay = int.Parse(match.Groups[7].Value);
                    
                    Vector3 target = new Vector3(x, y, z);

                    _ = Task.Run(async () =>
                    {
                        LogHelper.Print($"延迟 {delay}ms 后移动到坐标: ({x}, {y}, {z})");

                        if (delay > 0)
                            await Task.Delay(delay);

                        BardBattleData.Instance.TargetPosition = target;
                        Core.Resolve<MemApiMove>().MoveToTarget(target);
                    });
                }
                else
                {
                    LogHelper.PrintError("坐标格式错误，正确格式示例：/Wotou_BRD moveTo (100.5, 0, 92.8)");
                }

                break;
            }
            
            case var follow when follow.StartsWith("follow", StringComparison.OrdinalIgnoreCase):
            {
                // 匹配格式：follow ID for 3000 或 follow ID for 3000 delay 1500
                var match = Regex.Match(args, @"^follow\s+(\d+)\s+for\s+(\d+)(?:\s+delay\s+(\d+))?$", RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    if (!uint.TryParse(match.Groups[1].Value, out uint entityId))
                    {
                        LogHelper.PrintError("无法解析 EntityId！");
                        break;
                    }

                    int duration = int.Parse(match.Groups[2].Value);
                    int delay = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;
                    
                    BardBattleData.Instance.FollowingTarget = PartyHelper.Party.FirstOrDefault(p => p.EntityId == entityId);
                    if (BardBattleData.Instance.FollowingTarget == null)
                    {
                        LogHelper.PrintError($"找不到 ID 为 {entityId} 的小队成员！");
                        break;
                    }

                    LogHelper.Print($"将在 {delay} 毫秒后开始跟随 {entityId}，持续 {duration} 毫秒。");

                    _ = Task.Run(async () =>
                    {
                        if (delay > 0)
                            await Task.Delay(delay);

                        var startTime = DateTime.UtcNow;
                        BardBattleData.Instance.IsFollowing = true;
                        while ((DateTime.UtcNow - startTime).TotalMilliseconds < duration)
                        {
                            if (BardBattleData.Instance.FollowingTarget == null || BardBattleData.Instance.FollowingTarget.IsDead)
                            {
                                LogHelper.Print($"跟随目标 {entityId} 不存在或者已被清除，停止跟随。");
                                BardBattleData.Instance.IsFollowing = false;
                                break;
                            }
                            Core.Resolve<MemApiMove>().MoveToTarget(BardBattleData.Instance.FollowingTarget.Position);
                            BardBattleData.Instance.IsFollowing = true;
                        }

                        LogHelper.Print($"已停止跟随 {entityId}。");
                        BardBattleData.Instance.IsFollowing = false;
                    });
                }
                else
                {
                    LogHelper.PrintError("格式错误，示例：/Wotou_BRD follow 266637666 for 3000 delay 1500");
                }

                break;
            }

            default:
                ChatHelper.SendMessage($"未知参数: {args}");
                break;
        }
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
        bool currentValue = BardRotationEntry.QT.GetQt(qtKey);
        BardRotationEntry.QT.SetQt(qtKey, !currentValue);

        LogHelper.Print($"QT \"{qtKey}\" 已设置为 {(!currentValue).ToString().ToLower()}。");
    }
    
    public void OnExitRotation()
    {
        ECHelper.Commands.RemoveHandler("/Wotou_BRD");
    }

    public void OnTerritoryChanged()
    {
        BardRotationEntry.UpdateWardensPaeanPanel();
        
        /*if (LowVipRestrictor.IsRestrictedZoneForLowVip())
        {
            if (BardSettings.Instance.QwertyList.Count <= 7)
            {
                BardSettings.Instance.QwertyList = PartyHelper.Party.Select(player => LowVipRestrictor.ComputeMd5Hash(player.Name.ToString())).ToList();
                BardSettings.Instance.Save();
            }
        }*/
    }
    
    private void SmartUseHighPrioritySlot()
    {
        if (Core.Resolve<MemApiCondition>().IsInCombat() && 
            Core.Me.GetCurrTarget() != null &&
            Core.Me.GetCurrTarget().CanAttack())
            BardBattleData.Instance.HotkeyUseHighPrioritySlot = true;
        else
            BardBattleData.Instance.HotkeyUseHighPrioritySlot = false;
    }

    private void CancelMoving()
    {
        Core.Resolve<MemApiMove>().CancelMove();
        BardBattleData.Instance.TargetPosition = null;
        BardBattleData.Instance.FollowingTarget = null;
        BardBattleData.Instance.IsFollowing = false;
    }
}
