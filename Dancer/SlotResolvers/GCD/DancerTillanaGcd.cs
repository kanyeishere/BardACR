using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.GCD;

public class DancerTillanaGcd : ISlotResolver
{
    private const uint Tillana = DancerDefinesData.Spells.Tillana;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint FlourishingFinish = DancerDefinesData.Buffs.FlourishingFinish;
    private const uint LastDanceReady = DancerDefinesData.Buffs.LastDanceReady;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;

    public int Check()
    {
        if (!Tillana.GetSpell().IsReadyWithCanCast())
            return -1;
        if (!Tillana.IsUnlockWithCDCheck())
            return -10;
        if (Core.Me.HasLocalPlayerAura(FlourishingFinish) && !Core.Me.HasMyAuraWithTimeleft(FlourishingFinish, 3000))
            return 4;
        if (TargetHelper.GetNearbyEnemyCount(15) == 0)
            return -20;
        // 不在小舞前1G使用 
        if  (Core.Resolve<MemApiSpell>().CheckActionChange(StandardStep).GetSpell().Cooldown.TotalMilliseconds < 
             2 * GCDHelper.GetGCDDuration() - 1000 && 
             DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -2;
        // 不在小舞前2G且有落幕舞buff时使用
        if (Core.Resolve<MemApiSpell>().CheckActionChange(StandardStep).GetSpell().Cooldown.TotalMilliseconds < 
             3 * GCDHelper.GetGCDDuration() - 1000 && 
             Core.Me.HasAura(LastDanceReady) && 
             DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -3;
        if (Core.Resolve<JobApi_Dancer>().Esprit <= DancerSettings.Instance.TillanaEspritThreshold && Core.Me.HasLocalPlayerAura(Devilment))
            return 1;
        if (Core.Resolve<JobApi_Dancer>().Esprit <= 40 && !Core.Me.HasLocalPlayerAura(Devilment))
            return 2;
        if (Core.Me.HasLocalPlayerAura(Devilment) && 
            !Core.Me.HasMyAuraWithTimeleft(Devilment, 2600) && 
            Core.Resolve<JobApi_Dancer>().Esprit <= DancerSettings.Instance.TillanaLastGcdEspritThreshold && 
            !Core.Me.HasAura(SilkenFlow) &&
            !Core.Me.HasAura(SilkenSymmetry))
            return 3;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Tillana.GetSpell());
    }
}