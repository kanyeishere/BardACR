using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.Ability;

public class DancerFlourishAbility : ISlotResolver
{
    private const uint Flourish = DancerDefinesData.Spells.Flourish;
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    private const uint QuadrupleTechnicalFinish = DancerDefinesData.Spells.QuadrupleTechnicalFinish;
    public int Check()
    {
        if (!Flourish.GetSpell().IsReadyWithCanCast())
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -4;
        if (!DancerRotationEntry.QT.GetQt(QTKey.Flourish))
            return -1;
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds < 15000)
            return -2;
        if (QuadrupleTechnicalFinish.RecentlyUsed(1500))
            return -3;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(Flourish.GetSpell());
    }
}