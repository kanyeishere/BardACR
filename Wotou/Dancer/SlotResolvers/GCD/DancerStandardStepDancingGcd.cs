using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerStandardStepDancingGcd : ISlotResolver
{
    private const uint StandardStep = DancerDefinesData.Buffs.StandardStep;
    private const uint TechnicalStep = DancerDefinesData.Buffs.TechnicalStep;
    private const uint DoubleStandardFinish = DancerDefinesData.Spells.DoubleStandardFinish;
    
    public int Check()
    {
        if (Core.Resolve<JobApi_Dancer>().CompleteSteps == 4)
            return -1;
        if (Core.Me.HasAura(TechnicalStep))
            return -2;
        if (!Core.Me.HasAura(StandardStep))
            return -3;
        if (!Core.Resolve<JobApi_Dancer>().IsDancing)
            return -4;
        return 1;
    }

    public void Build(Slot slot)
    {
        if (Core.Me.HasAura(StandardStep) && Core.Resolve<JobApi_Dancer>().CompleteSteps == 2)
        {
            slot.Add(DoubleStandardFinish.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 0;
            return;
        }
        slot.Wait2NextGcd = true;
        slot.Add(Core.Resolve<JobApi_Dancer>().NextStep.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 0;
    }
}