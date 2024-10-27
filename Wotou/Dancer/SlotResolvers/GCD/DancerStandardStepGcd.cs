using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.GCD;

public class DancerStandardStepGcd : ISlotResolver
{
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    private const uint FinishingMove = DancerDefinesData.Spells.FinishingMove;
    
    public int Check()
    {
        if (!DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -1;
        if (Core.Me.HasAura(DancerDefinesData.Buffs.FinishingMoveReady))
            return -2;
        if (Core.Resolve<JobApi_Dancer>().IsDancing)
            return -3;
        if (StandardStep.GetSpell().Cooldown.TotalMilliseconds <= DancerSettings.Instance.StandardStepCdTolerance)
            return 1;
        if (!StandardStep.IsReady())
            return -1;
        return 0;
    }

    public void Build(Slot slot)
    {
        if (StandardStep.GetSpell().Cooldown.TotalMilliseconds <= DancerSettings.Instance.StandardStepCdTolerance &&
            !FinishingMove.IsReady())
        {
            slot.Add(StandardStep.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 1;
            return;
        }

        if (FinishingMove.IsReady())
        {
            slot.Add(FinishingMove.GetSpell());
            return;
        }
        if (StandardStep.IsReady())
        {
            slot.Add(StandardStep.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 1;
            return;
        }
    }
}