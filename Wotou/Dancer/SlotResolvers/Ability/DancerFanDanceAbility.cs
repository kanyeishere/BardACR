using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.Ability;

public class DancerFanDanceAbility : ISlotResolver
{
    private const uint FanDance = DancerDefinesData.Spells.FanDance;
    private const uint FanDance2 = DancerDefinesData.Spells.FanDance2;
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!FanDance.IsReady() && !CanUseFanDance2())
            return -2;
        if (DancerRotationEntry.QT.GetQt(QTKey.FinalBurst))
            return 100;
        if (Core.Resolve<JobApi_Dancer>().FourFoldFeathers > 3 &&
            (Core.Me.HasLocalPlayerAura(FlourishingSymmetry) ||
             Core.Me.HasLocalPlayerAura(FlourishingFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenSymmetry)))
            return 0;
        if (Core.Me.HasLocalPlayerAura(Devilment))
            return 1;
        return -4;
    }

    public void Build(Slot slot)
    {
        if (CanUseFanDance2())
        {
            slot.Add(FanDance2.GetSpell());
            return;
        }
        slot.Add(FanDance.GetSpell());
    }
    
    private static bool CanUseFanDance2()
    {
        return FanDance2.IsReady() &&
               TargetHelper.GetNearbyEnemyCount(5) > 2 && 
               DancerRotationEntry.QT.GetQt(QTKey.Aoe);
    }
}