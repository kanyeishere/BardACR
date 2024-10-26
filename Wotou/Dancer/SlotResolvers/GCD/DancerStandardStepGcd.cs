using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerStandardStepGcd : ISlotResolver
{
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    public int Check()
    {
        if (!StandardStep.IsReady())
            return -1;
        if (!DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -1;
        if (Core.Me.HasAura(DancerDefinesData.Buffs.FinishingMoveReady))
            return -2;
        if (Core.Resolve<JobApi_Dancer>().IsDancing)
            return -3;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(StandardStep.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 1;
    }
}