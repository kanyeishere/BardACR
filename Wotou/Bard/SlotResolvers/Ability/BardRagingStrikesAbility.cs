using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
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
    private const uint Peloton = BardDefinesData.Spells.Peloton;
    private const uint Potion = BardDefinesData.Spells.Potion;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (BardRotationEntry.QT.GetQt("对齐旅神") && Core.Resolve<JobApi_Bard>().ActiveSong != Song.WANDERER)
            return -1;
        
        // 第一个120s技能是RagingStrikes，且剩下的两个120s技能中有一个技能的CD大于当前GCDDuration
        if (BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            (BardBattleData.Instance.Second120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds > GCDHelper.GetGCDDuration() ||
             BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds > GCDHelper.GetGCDDuration())
            )
            return -1;
        if (BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 2000)
            return -1;
        
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <= BardSettings.Instance.PotionBeforeGcdTime && 
            BardRotationEntry.QT.GetQt("爆发药") &&
            ItemHelper.CheckCurrJobPotion() &&
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.RagingStrikeBeforeGcdTime + BardSettings.Instance.PotionBeforeGcdTime
           )
        {
            if (BardRotationEntry.QT.GetQt("Debug"))
            {
                LogHelper.Print("爆发药", "准备使用爆发药");
                LogHelper.Print("当前时间", GCDHelper.GetGCDCooldown().ToString());
                LogHelper.Print("RagingStrikeBeforeGcdTime", BardSettings.Instance.RagingStrikeBeforeGcdTime.ToString());
                LogHelper.Print("PotionBeforeGcdTime", BardSettings.Instance.PotionBeforeGcdTime.ToString());
            }
            return 1; 
        }
        
        if (RagingStrikes.IsReady() && GCDHelper.GetGCDCooldown() <= BardSettings.Instance.RagingStrikeBeforeGcdTime)
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        if (BardRotationEntry.QT.GetQt("爆发药") && ItemHelper.CheckCurrJobPotion())
            slot.Add(Spell.CreatePotion());
        slot.Add(RagingStrikes.GetSpell());
    }
}