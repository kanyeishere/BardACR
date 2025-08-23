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
    private const uint FanDance3 = DancerDefinesData.Spells.FanDance3;
    
    private const uint ThreeFoldFanDance = DancerDefinesData.Buffs.ThreeFoldFanDance;
    public int Check()
    {
        if (!Flourish.GetSpell().IsReadyWithCanCast())
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -4;
        if (!DancerRotationEntry.QT.GetQt(QTKey.Flourish))
            return -1;
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds < 10000 && DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return -2;
        if (QuadrupleTechnicalFinish.RecentlyUsed(1500))
            return -3;
        if (Core.Me.HasLocalPlayerAura(ThreeFoldFanDance))
            return -5;
        if (FanDance3.GetSpell().IsReadyWithCanCast())
            return -40;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(Flourish.GetSpell());
    }
}