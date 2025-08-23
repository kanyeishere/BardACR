using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardBattleVoiceAndRadiantFinaleAbility: ISlotResolver
{
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() >= BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs)
            return -1;
        if (BardRotationEntry.QT.GetQt("对齐旅神") && 
            Core.Resolve<JobApi_Bard>().ActiveSong != Song.Wanderer)
            return -1;
        if (BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 2000)
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (!BattleVoice.GetSpell().IsReadyWithCanCast())
            return -1;
        // 第一个120s技能是BattleVoice，且剩下的两个120s技能中有一个技能的CD大于当前GCDDuration + UseBattleVoiceBeforeGcdTimeInMs - RagingStrikeBeforeGcdTime
        if (BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            (BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds >
             GCDHelper.GetGCDDuration()
             + BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs
             - BardSettings.Instance.RagingStrikeBeforeGcdTime || 
             BardBattleData.Instance.Second120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds >
             GCDHelper.GetGCDDuration() 
             + BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs
             - BardSettings.Instance.RagingStrikeBeforeGcdTime)
            )
            return -1;
        BardUtil.LogDebug("GCDDuration 1: ",GCDHelper.GetGCDDuration().ToString());
        BardUtil.LogDebug("ElapsedGCD 1: ",GCDHelper.GetElapsedGCD().ToString());
        BardUtil.LogDebug("GcdCooldown 1: ",GCDHelper.GetGCDCooldown().ToString());
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BattleVoice.GetSpell());
        if (RadiantFinale.IsUnlock())
            slot.Add(RadiantFinale.GetSpell());
        if (Core.Me.Level < 90 && !BardSettings.Instance.IsDailyMode)
            slot.Add(RagingStrikes.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 0;
        BardUtil.LogDebug("GcdCooldown 2: ",GCDHelper.GetGCDCooldown().ToString());
    }
}