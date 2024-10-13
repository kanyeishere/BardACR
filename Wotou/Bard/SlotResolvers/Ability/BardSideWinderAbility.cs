using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardSideWinderAbility: ISlotResolver
{
    private const uint SideWinder = BardDefinesData.Spells.Sidewinder;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!SideWinder.IsReady())
            return -1;
        if (SideWinder.RecentlyUsed())
            return -1;
        if (Util.PartyBuffWillBeReadyIn(10000))
            return -1;
        return 1;
    }

    public void Build(Slot slot) => slot.Add(SideWinder.GetSpell());
}