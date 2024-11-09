using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.GCD;

public class DancerProcFountainFallMediumGcd : ISlotResolver
{
    private const uint FountainFall = DancerDefinesData.Spells.Fountainfall;
    private const uint BloodShower = DancerDefinesData.Spells.Bloodshower;
    
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    
    public int Check()
    {
        if (!FountainFall.GetSpell().IsReadyWithCanCast())
            return -2;
        
        if (Core.Resolve<JobApi_Dancer>().FourFoldFeathers == 4)
            return -3;
        
        if (Core.Me.HasAura(SilkenFlow) && 
            Core.Me.HasLocalPlayerAura(Devilment) &&
            Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me, SilkenFlow, true) < 
            Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me, Devilment, true))
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.CanUseAoeCombo() ? BloodShower.GetSpell() : FountainFall.GetSpell());
    }
}