using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardRagingStrikesAbility : ISlotResolver
{
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint Peloton = BardDefinesData.Spells.Peloton;
    private const uint Potion = BardDefinesData.Spells.Potion;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (RagingStrikes.RecentlyUsed())
            return -1;
        if (RagingStrikes.IsReady() && GCDHelper.GetGCDCooldown() <= BardSettings.Instance.RagingStrikeGcdTime)
            return 1;
        if (RagingStrikes.IsReady() && BardRotationEntry.QT.GetQt("爆发药") && Potion.IsReady() && GCDHelper.GetGCDCooldown() <= 1400)
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        if (BardRotationEntry.QT.GetQt("爆发药") && Potion.IsReady())
            slot.Add(Potion.GetSpell());
        slot.Add(RagingStrikes.GetSpell());
    }
}