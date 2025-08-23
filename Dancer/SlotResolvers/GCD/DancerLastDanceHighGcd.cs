using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.GCD;

public class DancerLastDanceHighGcd : ISlotResolver
{
    private const uint LastDance = DancerDefinesData.Spells.LastDance;
    private const uint LastDanceReady = DancerDefinesData.Buffs.LastDanceReady;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    public int Check()
    {
        if (!LastDance.GetSpell().IsReadyWithCanCast())
            return -1; 
        if (!Core.Me.HasAura(LastDanceReady))
            return -2;
        if (Core.Resolve<MemApiSpell>().CheckActionChange(StandardStep).GetSpell().Cooldown.TotalMilliseconds > 3500)
            return -1;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.GetSmartAoeSpell(LastDance));
        // slot.Add(LastDance.GetSpell());
    }
}