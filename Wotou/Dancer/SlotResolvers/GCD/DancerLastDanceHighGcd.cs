using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerLastDanceHighGcd : ISlotResolver
{
    private const uint LastDance = DancerDefinesData.Spells.LastDance;
    private const uint LastDanceReady = DancerDefinesData.Buffs.LastDanceReady;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    public int Check()
    {
        if (!LastDance.IsReady())
            return -1; 
        if (!Core.Me.HasAura(LastDanceReady))
            return -2;
        if ( Core.Resolve<MemApiSpell>().CheckActionChange(StandardStep).GetSpell().Cooldown.TotalMilliseconds >= GCDHelper.GetGCDDuration() * 2)
            return -1;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(LastDance.GetSpell());
    }
}