using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.GCD;

public class DancerSaberDanceGcd : ISlotResolver
{
    private const uint SaberDance = DancerDefinesData.Spells.SaberDance;
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    
    public int Check()
    {
        if (!Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).IsReady())
            return -1;
        /*if (Core.Me.HasAura(FlourishingSymmetry) || 
            Core.Me.HasAura(FlourishingFlow))
            return -4;*/
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds < 35000 && 
            StandardStep.GetSpell().Cooldown.TotalMilliseconds < 3500 &&
            Core.Resolve<JobApi_Dancer>().Esprit >= 85 &&
            DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep) &&
            DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return 1;
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds < 35000 && 
            Core.Resolve<JobApi_Dancer>().Esprit < 90 &&
            DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return -2;
        if (Core.Resolve<JobApi_Dancer>().Esprit < 50)
            return -3;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).GetSpell());
    }
}