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
using Wotou.Bard.Utility;
using Wotou.Common;

namespace Wotou.Bard;

/// <summary>
/// 事件回调处理类 参考接口里的方法注释
/// </summary>
public class BardRotationEventHandler : IRotationEventHandler
{
    private long _randomTime = 0;
    
    public async Task OnPreCombat()
    {
        BardRotationEntry.UpdateWardensPaeanPanel();
        SmartUseHighPrioritySlot();
        
        if (Core.Me.IsMoving())
            Core.Resolve<MemApiMove>().CancelMove();
        if (Core.Resolve<MemApiMove>().IsMoving())
            Core.Resolve<MemApiMove>().CancelMove();
        
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
        
        if (PartyHelper.CastableParty.Any(characterAgent => 
                (characterAgent.HasAura(1896U) && BardSettings.Instance.NaturesMinneWithRecitation) ||  //秘策
                (characterAgent.HasAura(2611U) && BardSettings.Instance.NaturesMinneWithZoe) ||         //活化
                (characterAgent.HasAura(1892U) && BardSettings.Instance.NaturesMinneWithNeutralSect)) &&   //中间学派
            BardDefinesData.Spells.NaturesMinne.IsUnlockWithCDCheck() &&
            BardRotationEntry.QT.GetQt(QTKey.NatureMinne))
            await BardDefinesData.Spells.NaturesMinne.GetSpell().Cast();
    }

    public void OnResetBattle()
    {
        BardRotationEntry.UpdateWardensPaeanPanel();
        BardRotationEntry.QT.Reset();
        BardBattleData.Instance = new BardBattleData();
        
        // 重置碎心箭保留层数
        BardSettings.Instance.HeartBreakSaveStack = 0; 
        
        _randomTime = 0;
        
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
            BardDefinesData.Spells.TheWanderersMinuet => Song.WANDERER,
            BardDefinesData.Spells.MagesBallad => Song.MAGE,
            BardDefinesData.Spells.ArmysPaeon => Song.ARMY,
            _ => BardBattleData.Instance.LastSong
        };
        
        if (spell.Id == BardDefinesData.Spells.RagingStrikes)
            BardBattleData.Instance.HasUseIronJawsInCurrentBursting = false;
    }

    public void OnBattleUpdate(int currTimeInMs)
    {
        SmartUseHighPrioritySlot();
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
        if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
        {
            LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
            ChatHelper.SendMessage("/e 警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
            ChatHelper.SendMessage("/e <se.1>");
        }
    }

    public void OnEnterRotation()
    {
        try
        {
            if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
            {
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的诗人ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
                LogHelper.PrintError("警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                ChatHelper.SendMessage("/e 警告，你开启了全局能力技能不卡GCD，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中关闭");
                ChatHelper.SendMessage("/e <se.1>");
            }
            else if (BardSettings.Instance.WelcomeVoice)
                Core.Resolve<MemApiChatMessage>()
                    .Toast2("欢迎使用窝头的诗人ACR", 1, 5000);
        } catch (MissingFieldException ex)
        {
            Core.Resolve<MemApiChatMessage>()
                .Toast2("欢迎使用窝头的诗人ACR\n请关闭全局能力技能不卡GCD\n打开此设置会导致本ACR产生能力技插入问题", 1, 5000);
            LogHelper.PrintError("警告，你没有进行全局能力技能不卡GCD设置，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中先开启全局能力技能不卡GCD后，再重新关闭一次");
            ChatHelper.SendMessage("/e 警告，你没有进行全局能力技能不卡GCD设置，请进入 AE悬浮图标->ACR->首页->设置->基础设置->能力技 中先开启全局能力技能不卡GCD后，再重新关闭一次");
            ChatHelper.SendMessage("/e <se.1>");
        }
        if (BardSettings.Instance.WelcomeVoice)
            ChatHelper.SendMessage("/pdr tts 你好，欢迎你使用窝头诗人");
        TimeLineUpdater.UpdateFiles("https://raw.githubusercontent.com/kanyeishere/ACR-Timeline/refs/heads/main/Wotou-BardMaster.json");
        
        try
        {
            ECHelper.Commands.RemoveHandler("/Wotou_BRD");
        }
        catch (Exception)
        {
            // ignored
        }

        // 注册命令
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
        string lowerArgs = args.Trim().ToLower();

        // 检查是否是 QTKey + "_qt" 格式的命令
        if (lowerArgs.EndsWith("_qt"))
        {
            // 提取 QTKey 部分，保持原始大小写以匹配 QTKey 常量
            string keyPart = lowerArgs.Substring(0, lowerArgs.Length - 3); // 去除 "_qt"

            // 尝试匹配 QTKey 常量
            string matchedKey = GetMatchingQtKey(keyPart);
            if (matchedKey != null)
            {
                ToggleQtSetting(matchedKey);
                return;
            }
            else
            {
                LogHelper.Print($"未知 QTKey 参数: {keyPart}");
                return;
            }
        }

        switch (lowerArgs)
        {
            case "hello":
                LogHelper.Print("你好！这是一条测试消息！");
                break;

            case "防击退_hk":
                ExecuteHotkey(new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.ArmsLength, SpellTargetType.Target));
                break;

            case "续毒_hk":
                ExecuteHotkey(new IronJawsHotkeyResolver(BardDefinesData.Spells.IronJaws, SpellTargetType.Target));
                break;

            case "内丹_hk":
                ExecuteHotkey(new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.SecondWind, SpellTargetType.Target));
                break;

            case "行吟_hk":
                ExecuteHotkey(new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.Troubadour, SpellTargetType.Target));
                break;

            case "大地神_hk":
                ExecuteHotkey(new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.NaturesMinne, SpellTargetType.Target));
                break;

            case "疾跑_hk":
                ExecuteHotkey(new HotKeyResolver_疾跑());
                break;

            case "后跳_hk":
                ExecuteHotkey(new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.RepellingShot, SpellTargetType.Target));
                break;

            case "爆发药_hk":
                ExecuteHotkey(new HotKeyResolver_Potion());
                break;

            case "极限技_hk":
                ExecuteHotkey(new HotKeyResolver_LB());
                break;

            case "停止自动移动_hk":
                ExecuteHotkey(new StopMoveHotkeyResolver());
                break;

            default:
                LogHelper.Print($"未知参数: {args}");
                break;
        }
    }
    
    private void ExecuteHotkey(IHotkeyResolver resolver)
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
    
    private string GetMatchingQtKey(string keyPart)
    {
        // 遍历 QTKey 类中的所有常量
        var qtKeys = typeof(QTKey).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(fi => (string)fi.GetValue(null))
            .ToList();

        // 查找匹配的 key（忽略大小写）
        foreach (var qtKey in qtKeys)
        {
            if (string.Equals(qtKey, keyPart, StringComparison.OrdinalIgnoreCase))
            {
                return qtKey;
            }
        }

        return null; // 未找到匹配的 QTKey
    }

    private void ToggleQtSetting(string qtKey)
    {
        bool currentValue = BardRotationEntry.QT.GetQt(qtKey);
        BardRotationEntry.QT.SetQt(qtKey, !currentValue);

        LogHelper.Print($"QT \"{qtKey}\" 已设置为 {(!currentValue).ToString().ToLower()}。");
    }
    
    public void OnExitRotation()
    {
    }

    public void OnTerritoryChanged()
    {
        BardRotationEntry.UpdateWardensPaeanPanel();
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
}