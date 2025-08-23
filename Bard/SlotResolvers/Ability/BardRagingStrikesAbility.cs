using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardRagingStrikesAbility : ISlotResolver
{
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint LegGraze = BardDefinesData.Spells.LegGraze;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (BardRotationEntry.QT.GetQt("对齐旅神") && 
            Core.Resolve<JobApi_Bard>().ActiveSong != Song.Wanderer)
            return -1;
        // 90级以下，高难模式下，猛者与战斗之声一起使用，不在这里处理
        if (Core.Me.Level < 90 && !BardSettings.Instance.IsDailyMode)
            return -1;
        
        // 第一个120s技能是RagingStrikes，且剩下的两个120s技能中有一个技能的CD大于当前GCDDuration
        if (BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            !BardSettings.Instance.ImitateGreenPlayer &&
            (BardBattleData.Instance.Second120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds > GCDHelper.GetGCDDuration() - 650 ||
             BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds > GCDHelper.GetGCDDuration()))
            return -1;
        
        if (BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            BardSettings.Instance.ImitateGreenPlayer &&
            (BardBattleData.Instance.Second120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds > GCDHelper.GetGCDDuration() - 650 + 650 ||
             BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds > GCDHelper.GetGCDDuration()))
            return -1;
        
        // 第一个120s技能是BattleVoice，且BattleVoice的CD小于2s，不使用RagingStrikes以免抢占BattleVoice的CD
        if (BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 2000 &&
            BardBattleData.Instance.First120SBuffSpellId.IsUnlock())
            return -1;
        
        // 使用爆发药
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <= BardSettings.Instance.PotionBeforeGcdTime && 
            BardRotationEntry.QT.GetQt("爆发药") &&
            ItemHelper.CheckCurrJobPotion() &&
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.RagingStrikeBeforeGcdTime + BardSettings.Instance.PotionBeforeGcdTime)
            return 1;
        
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <= 650 &&
            BardSettings.Instance.ImitateGreenPlayer &&
            GCDHelper.GetGCDCooldown() <= 650 + BardSettings.Instance.RagingStrikeBeforeGcdTime)
            return 1;
        
        // 不使用爆发药
        if (RagingStrikes.GetSpell().IsReadyWithCanCast() && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.RagingStrikeBeforeGcdTime)
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        if (BardRotationEntry.QT.GetQt("爆发药") && ItemHelper.CheckCurrJobPotion())
        {
            slot.Add(Spell.CreatePotion());
            slot.Add(RagingStrikes.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 0;
            return;
        }
        var potionId = SettingMgr.GetSetting<PotionSetting>().GetPotionId(Core.Me.CurrentJob());
        var cooldown = Core.Me?.GetItemCoolDown(potionId);
        if (potionId == 0 || cooldown == null || cooldown.Value.TotalMilliseconds <= (240 + 29) * 1000)
        {
            if (BardSettings.Instance.ImitateGreenPlayer)
            {
                slot.Add(LegGraze.GetSpell());
                slot.Add(RagingStrikes.GetSpell());
                AI.Instance.BattleData.CurrGcdAbilityCount = 0;
                return;
            }
        }
        
        slot.Add(RagingStrikes.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 0;
        // 单插 
    }
}