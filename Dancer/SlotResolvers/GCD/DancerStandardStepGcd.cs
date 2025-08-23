using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.GCD;

public class DancerStandardStepGcd : ISlotResolver
{
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    private const uint FinishingMove = DancerDefinesData.Spells.FinishingMove;
    
    private const uint FinishingMoveReady = DancerDefinesData.Buffs.FinishingMoveReady;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint TechnicalFinish = DancerDefinesData.Buffs.TechnicalFinish;
    
    public int Check()
    {
        if (!DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -1;
        if (Core.Me.HasAura(FinishingMoveReady))
            return -2;
        if (Core.Resolve<JobApi_Dancer>().IsDancing)
            return -3;
        if (!StandardStep.GetSpell().IsUnlock())
            return -4;
        /*if (Core.Me.HasLocalPlayerAura(Devilment) && !Core.Me.HasMyAuraWithTimeleft(Devilment, 3500))
            return -4;*/
        
        if (Core.Me.HasAura(TechnicalFinish) && 
            !Core.Me.HasMyAuraWithTimeleft(TechnicalFinish, 3700) && 
            Core.Me.Level == 90)
            return -1;
        if (Core.Me.HasAura(Devilment) &&
            !Core.Me.HasMyAuraWithTimeleft(Devilment, 3700) && 
            Core.Me.Level == 90)
            return -1;
        
        if (StandardStep.GetSpell().Cooldown.TotalMilliseconds <= DancerSettings.Instance.StandardStepCdTolerance)
            return 1;
        if (!StandardStep.GetSpell().IsReadyWithCanCast())
            return -1;
        return 0;
    }

    public void Build(Slot slot)
    {
        if (FinishingMove.GetSpell().IsReadyWithCanCast() || Core.Me.HasAura(FinishingMoveReady))
        {
            slot.Add(FinishingMove.GetSpell());
            return;
        }
        if (StandardStep.GetSpell().IsReadyWithCanCast() || !Core.Me.HasAura(FinishingMoveReady))
        {
            slot.Add(StandardStep.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 1;
        }
    }
}