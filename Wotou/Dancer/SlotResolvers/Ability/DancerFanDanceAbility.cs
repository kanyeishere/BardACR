using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Ability;

public class DancerFanDanceAbility : ISlotResolver
{
    private const uint FanDance = DancerDefinesData.Spells.FanDance;
    private const uint FanDance2 = DancerDefinesData.Spells.FanDance2;
    private const uint QuadrupleTechnicalFinish = DancerDefinesData.Spells.QuadrupleTechnicalFinish;
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint Medicated = DancerDefinesData.Buffs.Medicated;
    
    public int Check()
    {
        if (!DancerRotationEntry.QT.GetQt(QTKey.FanDance))
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!FanDance.GetSpell().IsReadyWithCanCast() && !CanUseFanDance2())
            return -2;
        if (QuadrupleTechnicalFinish.RecentlyUsed(1500))
            return -3;
        if (DancerRotationEntry.QT.GetQt(QTKey.FinalBurst))
            return 100;
        if (Core.Resolve<JobApi_Dancer>().FourFoldFeathers > DancerSettings.Instance.FanDanceSaveStack  &&
            (Core.Me.HasLocalPlayerAura(FlourishingSymmetry) ||
             Core.Me.HasLocalPlayerAura(FlourishingFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenSymmetry)))
            return 0;
        if (Core.Me.HasLocalPlayerAura(Devilment))
            return 1;
        if (Core.Me.HasLocalPlayerAura(Medicated))
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
        return FanDance2.GetSpell().IsReadyWithCanCast() &&
               TargetHelper.GetNearbyEnemyCount(5) > 2 && 
               DancerRotationEntry.QT.GetQt(QTKey.Aoe);
    }
}