using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.Ability;

public class DancerFanDance3Ability : ISlotResolver
{
    private const uint FanDance3 = DancerDefinesData.Spells.FanDance3;
    private const uint QuadrupleTechnicalFinish = DancerDefinesData.Spells.QuadrupleTechnicalFinish;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    private const uint DevilmentSpell = DancerDefinesData.Spells.Devilment;
    private const uint Flourish = DancerDefinesData.Spells.Flourish;
    
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    private const uint ThreeFoldFanDance = DancerDefinesData.Buffs.ThreeFoldFanDance;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint FinishingMoveReady = DancerDefinesData.Buffs.FinishingMoveReady;
    private const uint Medicated = DancerDefinesData.Buffs.Medicated;
    
    public int Check()
    {
        if (!DancerRotationEntry.QT.GetQt(QTKey.FanDance))
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!FanDance3.GetSpell().IsReadyWithCanCast())
            return -2;
        if (QuadrupleTechnicalFinish.RecentlyUsed(1500))
            return -3;
        
        if (DancerRotationEntry.QT.GetQt(QTKey.FinalBurst))
            return 100;
        
        if (Core.Me.HasLocalPlayerAura(ThreeFoldFanDance) && !Core.Me.HasMyAuraWithTimeleft(ThreeFoldFanDance, 3500))
            return 2;
        
        if (Flourish.GetSpell().Cooldown.TotalMilliseconds <= 5000 && Flourish.IsUnlock())
            return 20;
        
        if (Core.Me.HasLocalPlayerAura(ThreeFoldFanDance) && 
            !Core.Me.HasMyAuraWithTimeleft(ThreeFoldFanDance, 7500) &&
            StandardStep.GetSpell().Cooldown.TotalMilliseconds < 2500 &&
            !Core.Me.HasLocalPlayerAura(FinishingMoveReady))
            return 3;
        
        if (Core.Me.HasLocalPlayerAura(ThreeFoldFanDance) &&
            Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me, ThreeFoldFanDance, true) < DevilmentSpell.GetSpell().Cooldown.TotalMilliseconds + 1600 &&
            DevilmentSpell.IsUnlock())
            return 4;

        if (!DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return 5;
        
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds <= 3500 && 
            DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep) &&
            TechnicalStep.IsUnlock())
            return -4;
        
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds <= 6000 && 
            Core.Me.HasLocalPlayerAura(SilkenSymmetry) && 
            DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep) &&
            TechnicalStep.IsUnlock())
            return -5;

        if (Core.Resolve<JobApi_Dancer>().FourFoldFeathers > DancerSettings.Instance.FanDanceSaveStack + 1)
            return 10;
        
        if (Core.Resolve<JobApi_Dancer>().FourFoldFeathers > DancerSettings.Instance.FanDanceSaveStack &&
            (Core.Me.HasLocalPlayerAura(FlourishingSymmetry) ||
             Core.Me.HasLocalPlayerAura(FlourishingFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenFlow) || 
             Core.Me.HasLocalPlayerAura(SilkenSymmetry)))
            return 11;
        if (Core.Me.HasLocalPlayerAura(Devilment))
            return 12;
        if (Core.Me.HasLocalPlayerAura(Medicated))
            return 13;
        if (!DevilmentSpell.IsUnlock())
            return 22;
        return -3;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.GetSmartAoeSpell(FanDance3));
        // slot.Add(FanDance3.GetSpell());
    }
}