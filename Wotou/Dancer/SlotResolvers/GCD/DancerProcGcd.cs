using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.GCD;

public class DancerProcGcd : ISlotResolver
{
    private const uint ReverseCascade = DancerDefinesData.Spells.ReverseCascade;
    private const uint FountainFall = DancerDefinesData.Spells.Fountainfall;
    private const uint RisingWindmill = DancerDefinesData.Spells.RisingWindmill;
    private const uint BloodShower = DancerDefinesData.Spells.Bloodshower;
    
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    
    public int Check()
    {
        if (!ReverseCascade.GetSpell().IsReadyWithCanCast() &&
            !FountainFall.GetSpell().IsReadyWithCanCast() && 
            !RisingWindmill.GetSpell().IsReadyWithCanCast() && 
            !BloodShower.GetSpell().IsReadyWithCanCast())
            return -1;

        if (!Core.Me.HasAura(FlourishingSymmetry) && 
            !Core.Me.HasAura(FlourishingFlow) && 
            !Core.Me.HasAura(SilkenFlow) && 
            !Core.Me.HasAura(SilkenSymmetry))
            return -2;
        
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.GetProcGcdCombo());
    }
}