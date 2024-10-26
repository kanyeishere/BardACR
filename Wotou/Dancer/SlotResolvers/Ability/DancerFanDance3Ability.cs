using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.Ability;

public class DancerFanDance3Ability : ISlotResolver
{
    private const uint FanDance3 = DancerDefinesData.Spells.FanDance3;
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    private const uint ThreeFoldFanDance = DancerDefinesData.Buffs.ThreeFoldFanDance;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!DancerDefinesData.Spells.FanDance3.IsReady())
            return -2;
        if (DancerRotationEntry.QT.GetQt(QTKey.FinalBurst))
            return 100;
        if (Core.Me.HasLocalPlayerAura(ThreeFoldFanDance) && !Core.Me.HasMyAuraWithTimeleft(ThreeFoldFanDance, 3500))
            return 2;
        if (Core.Resolve<JobApi_Dancer>().FourFoldFeathers > 3 &&
            (Core.Me.HasLocalPlayerAura(FlourishingSymmetry) ||
             Core.Me.HasLocalPlayerAura(FlourishingFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenSymmetry)))
            return 1;
        if (Core.Me.HasLocalPlayerAura(Devilment))
            return 1;
        return -3;
    }

    public void Build(Slot slot)
    {
        slot.Add(FanDance3.GetSpell());
    }
}