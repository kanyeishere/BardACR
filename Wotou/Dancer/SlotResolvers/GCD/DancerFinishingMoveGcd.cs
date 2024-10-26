using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerFinishingMoveGcd : ISlotResolver
{
    private const uint FinishingMove = DancerDefinesData.Spells.FinishingMove;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint FinishingMoveReady = DancerDefinesData.Buffs.FinishingMoveReady;

    public int Check()
    {
        if (!DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -1;
        if (Core.Me.HasLocalPlayerAura(Devilment) && FinishingMove.GetSpell().Cooldown.TotalMilliseconds <= 1000)
            return 1;
        if (!FinishingMove.IsReady())
            return -10;
        if (!Core.Me.HasAura(FinishingMoveReady))
            return -20;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(FinishingMove.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 1;
    }
}