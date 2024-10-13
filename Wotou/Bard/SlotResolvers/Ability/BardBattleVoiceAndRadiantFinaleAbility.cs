using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardBattleVoiceAndRadiantFinaleAbility: ISlotResolver
{
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() >= 1350)
            return -1;
        if(RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 2000)
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (BattleVoice.RecentlyUsed())
            return -1;
        if (!BattleVoice.IsReady())
            return -1;
        return 1;
    }

    public void Build(Slot slot)
    {
        
        slot.Add(BattleVoice.GetSpell());
        if (RadiantFinale.IsUnlock())
            slot.Add(RadiantFinale.GetSpell());
    }
}