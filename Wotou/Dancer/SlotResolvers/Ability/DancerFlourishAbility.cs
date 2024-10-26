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
    public int Check()
    {
        if (!Flourish.IsReady())
            return -1;
        if (!DancerRotationEntry.QT.GetQt(QTKey.Flourish))
            return -1;
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds < 10000)
            return -2;
        if (TechnicalStep.RecentlyUsed(1500))
            return -3;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(Flourish.GetSpell());
    }
}