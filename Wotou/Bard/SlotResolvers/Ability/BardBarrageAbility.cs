using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardBarrageAbility : ISlotResolver
{
    private const uint Barrage = BardDefinesData.Spells.Barrage;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -1;
        if (!Barrage.GetSpell().IsReadyWithCanCast())
            return -1;
        if (BardUtil.HasAllPartyBuff())
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Barrage.GetSpell());
    }
}