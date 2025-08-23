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

public class DancerProcReverseCascadeHighGcd : ISlotResolver
{
    private const uint ReverseCascade = DancerDefinesData.Spells.ReverseCascade;
    private const uint FountainFall = DancerDefinesData.Spells.Fountainfall;
    private const uint RisingWindmill = DancerDefinesData.Spells.RisingWindmill;
    private const uint BloodShower = DancerDefinesData.Spells.Bloodshower;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    private const uint FinishingMoveReady = DancerDefinesData.Buffs.FinishingMoveReady;
    
    public int Check()
    {
        if (!ReverseCascade.GetSpell().IsReadyWithCanCast() &&
            !FountainFall.GetSpell().IsReadyWithCanCast() && 
            !RisingWindmill.GetSpell().IsReadyWithCanCast() && 
            !BloodShower.GetSpell().IsReadyWithCanCast())
            return -2;
        
        if (Core.Resolve<JobApi_Dancer>().FourFoldFeathers == 4)
            return -3;
        
        if (Core.Me.HasAura(FlourishingSymmetry) && !Core.Me.HasMyAuraWithTimeleft(FlourishingSymmetry,3500))
            return 1;
        if (Core.Me.HasAura(SilkenSymmetry) && !Core.Me.HasMyAuraWithTimeleft(SilkenSymmetry, 3500))
            return 1;
        
        if (Core.Me.HasAura(FlourishingSymmetry) && 
            !Core.Me.HasMyAuraWithTimeleft(FlourishingSymmetry,8000) &&
            StandardStep.GetSpell().Cooldown.TotalMilliseconds < 5500 &&
            !Core.Me.HasLocalPlayerAura(FinishingMoveReady))
            return 1;
        
        if (Core.Me.HasAura(SilkenSymmetry) && 
            !Core.Me.HasMyAuraWithTimeleft(SilkenSymmetry,8000) && 
            StandardStep.GetSpell().Cooldown.TotalMilliseconds < 5500 &&
            !Core.Me.HasLocalPlayerAura(FinishingMoveReady))
            return 1;
        
        // 同时拥有两个百花触发
        if (Core.Me.HasAura(FlourishingFlow) && 
            Core.Me.HasAura(FlourishingSymmetry) &&
            !Core.Me.HasMyAuraWithTimeleft(FlourishingFlow,6000) &&
            !Core.Me.HasMyAuraWithTimeleft(FlourishingSymmetry,6000))
            return 1;
        
        // 同时两个百花触发又碰到小舞
        if (Core.Me.HasAura(FlourishingFlow) && 
            Core.Me.HasAura(FlourishingSymmetry) &&
            !Core.Me.HasMyAuraWithTimeleft(FlourishingFlow,11000) &&
            !Core.Me.HasMyAuraWithTimeleft(FlourishingSymmetry,11000) && 
            StandardStep.GetSpell().Cooldown.TotalMilliseconds < 5500 &&
            !Core.Me.HasLocalPlayerAura(FinishingMoveReady))
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.CanUseAoeCombo() ? RisingWindmill.GetSpell() : ReverseCascade.GetSpell());
    }
}