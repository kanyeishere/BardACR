using Wotou.Bard.Setting;
using Wotou.Bard.Data;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Utility;
using Wotou.Bard.Utility;

namespace Wotou.Bard;

/// <summary>
/// 事件回调处理类 参考接口里的方法注释
/// </summary>
public class BardRotationEventHandler : IRotationEventHandler
{

    private bool _originalValueForNoClipGcd3;
    private int _originalMaxAbilityTimesInGcd;
    
    public async Task OnPreCombat()
    {
        SmartUseHighPrioritySlot();
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
            
            if (BardSettings.Instance.EnableAutoPeloton && !Core.Me.InCombat())
            {
                if ((!Core.Me.HasAura(BardDefinesData.Buffs.Peloton) || 
                     !Core.Me.HasMyAuraWithTimeleft(BardDefinesData.Buffs.Peloton, 4000)) && 
                    !Core.Me.InCombat())
                {
                    await Task.Delay(new Random().Next(600, 3000));
                    if ((!Core.Me.HasAura(BardDefinesData.Buffs.Peloton) || 
                         !Core.Me.HasMyAuraWithTimeleft(BardDefinesData.Buffs.Peloton, 4000)) && 
                        !Core.Me.InCombat())
                        await BardDefinesData.Spells.Peloton.GetSpell().Cast();
                }
            }
        }
        if (PartyHelper.CastableParty.Any(characterAgent => 
                (characterAgent.HasAura(1896U) && BardSettings.Instance.NaturesMinneWithRecitation) ||  //秘策
                (characterAgent.HasAura(2611U) && BardSettings.Instance.NaturesMinneWithZoe) ||         //活化
                (characterAgent.HasAura(1892U) && BardSettings.Instance.NaturesMinneWithNeutralSect)))  //中间学派
        await BardDefinesData.Spells.NaturesMinne.GetSpell().Cast();
    }

    public void OnResetBattle()
    {
        // QT的设置重置为默认值
        BardRotationEntry.QT.Reset();
        
        // 重置战斗中缓存的数据
        BardBattleData.Instance = new BardBattleData();
        
        // 重置碎心箭保留层数
        BardSettings.Instance.HeartBreakSaveStack = 0;
        
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
        
        if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
            ChatHelper.SendMessage("/e 警告，严重错误，你开启了全局能力技能不卡GCD，可能导致本ACR产生能力技插入问题，建议关闭 <se.1>");
    }

    public void OnBattleUpdate(int currTimeInMs)
    {
        SmartUseHighPrioritySlot();
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
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

    public void OnTerritoryChanged()
    {
        
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