using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardPotionAbility : ISlotResolver
{
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint Potion = BardDefinesData.Spells.Potion;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发药"))
            return -1;
        if (!ItemHelper.CheckCurrJobPotion())
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <1500)
            return 1;
        if (BardRotationEntry.QT.GetQt("好了就吃"))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Potion.GetSpell());
    }
}
