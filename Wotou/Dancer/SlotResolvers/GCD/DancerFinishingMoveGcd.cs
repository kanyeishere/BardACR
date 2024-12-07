using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.GCD;

public class DancerFinishingMoveGcd : ISlotResolver
{
    private const uint FinishingMove = DancerDefinesData.Spells.FinishingMove;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint FinishingMoveReady = DancerDefinesData.Buffs.FinishingMoveReady;

    public int Check()
    {
        if (!DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -1;
        if (!Core.Me.HasAura(FinishingMoveReady))
            return -20;
        if (!FinishingMove.GetSpell().IsUnlock())
            return -101;
        if (FinishingMove.GetSpell().Cooldown.TotalMilliseconds <= DancerSettings.Instance.StandardStepCdTolerance)
            return 1;
        if (FinishingMove.GetSpell().IsReadyWithCanCast())
            return 10;
        return -10;
    }

    public void Build(Slot slot)
    {
        if (FinishingMove.GetSpell().IsReadyWithCanCast() || Core.Me.HasAura(FinishingMoveReady))
        {
            slot.Add(FinishingMove.GetSpell());
            return;
        }
        if (FinishingMove.GetSpell().Cooldown.TotalMilliseconds <= DancerSettings.Instance.StandardStepCdTolerance)
        {
            slot.Add(FinishingMove.GetSpell());
            return;
        }
        if (StandardStep.GetSpell().IsReadyWithCanCast() || !Core.Me.HasAura(FinishingMoveReady))
        {
            slot.Add(StandardStep.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 1;
        }
    }
}