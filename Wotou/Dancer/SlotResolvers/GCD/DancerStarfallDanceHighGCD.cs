using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerStarfallDanceHighGCD : ISlotResolver
{
    private const uint StarfallDance = DancerDefinesData.Spells.StarfallDance;
    private const uint FlourishingStarfall = DancerDefinesData.Buffs.FlourishingStarfall;
    
    public int Check()
    {
        if (!StarfallDance.IsReady())
            return -1;
        // 快过期的流星舞 
        if (Core.Me.HasAura(FlourishingStarfall) && 
            !Core.Me.HasMyAuraWithTimeleft(FlourishingStarfall,3500))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerDefinesData.Spells.StarfallDance.GetSpell());
    }
}