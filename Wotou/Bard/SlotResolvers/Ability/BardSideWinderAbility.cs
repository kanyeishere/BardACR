using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardSideWinderAbility: ISlotResolver
{
    private const uint SideWinder = BardDefinesData.Spells.Sidewinder;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt(QTKey.Sidewinder))
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -1;
        if (!SideWinder.IsReady())
            return -1;
        return 1;
    }

    public void Build(Slot slot) => slot.Add(SideWinder.GetSpell());
}