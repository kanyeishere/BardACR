using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerTechnicalStepDancingGcd: ISlotResolver
{
    private const uint StandardStep = DancerDefinesData.Buffs.StandardStep;
    private const uint TechnicalStep = DancerDefinesData.Buffs.TechnicalStep;
    private const uint QuadrupleTechnicalFinish = DancerDefinesData.Spells.QuadrupleTechnicalFinish;
    
    public int Check()
    {
        if (!Core.Me.HasAura(TechnicalStep))
            return -1;
        if (Core.Me.HasAura(StandardStep))
            return -2;
        if (!Core.Resolve<JobApi_Dancer>().IsDancing)
            return -3;
        return 1;
    }

    public void Build(Slot slot)
    {
        if (Core.Me.HasAura(TechnicalStep) && Core.Resolve<JobApi_Dancer>().CompleteSteps == 4)
        {
            slot.Add(QuadrupleTechnicalFinish.GetSpell());
            return;
        }
        slot.Add(Core.Resolve<JobApi_Dancer>().NextStep.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 1;
    }
}