using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardBarrageAbility : ISlotResolver
{
    private const uint Barrage = BardDefinesData.Spells.Barrage;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (!Barrage.IsReady())
            return -1;
        if (Util.HasAllPartyBuff())
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Barrage.GetSpell());
    }
}