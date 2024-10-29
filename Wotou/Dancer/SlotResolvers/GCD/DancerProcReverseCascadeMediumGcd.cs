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

public class DancerProcReverseCascadeMediumGcd : ISlotResolver
{
    private const uint ReverseCascade = DancerDefinesData.Spells.ReverseCascade;
    private const uint RisingWindmill = DancerDefinesData.Spells.RisingWindmill;

    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    
    public int Check()
    {
        if (!ReverseCascade.IsReady())
            return -2;
        
        if (Core.Me.HasAura(SilkenSymmetry) && 
            Core.Me.HasLocalPlayerAura(Devilment) &&
            Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me, SilkenSymmetry, true) < 
            Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me, Devilment, true))
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.CanUseAoeCombo() ? RisingWindmill.GetSpell() : ReverseCascade.GetSpell());
    }
}