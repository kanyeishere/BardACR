using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.GCD;

public class DancerTechnicalStepDancingGcd: ISlotResolver
{
    private const uint StandardStep = DancerDefinesData.Buffs.StandardStep;
    private const uint TechnicalStep = DancerDefinesData.Buffs.TechnicalStep;
    
    private const uint QuadrupleTechnicalFinish = DancerDefinesData.Spells.QuadrupleTechnicalFinish;
    private const uint Devilment = DancerDefinesData.Spells.Devilment;

    
    public int Check()
    {
        if (!Core.Me.HasAura(TechnicalStep))
            return -1;
        if (Core.Me.HasAura(StandardStep))
            return -2;
        if (!Core.Resolve<JobApi_Dancer>().IsDancing)
            return -3;
        return 1;
    }

    public void Build(Slot slot)
    {
        if (Core.Me.HasAura(TechnicalStep) && Core.Resolve<JobApi_Dancer>().CompleteSteps == 4 && QuadrupleTechnicalFinish.IsUnlockWithCDCheck() && Core.Resolve<MemApiSpell>().GetCooldown(Devilment).TotalMilliseconds < 500)
        {
            AI.Instance.BattleData.CurrGcdAbilityCount = 0;
            slot.Add(QuadrupleTechnicalFinish.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 0;
            slot.Add(Devilment.GetSpell());
            return;
        }
        if (Core.Me.HasAura(TechnicalStep) && Core.Resolve<JobApi_Dancer>().CompleteSteps == 4 && QuadrupleTechnicalFinish.IsUnlockWithCDCheck())
        {
            AI.Instance.BattleData.CurrGcdAbilityCount = 0;
            slot.Add(QuadrupleTechnicalFinish.GetSpell());
            AI.Instance.BattleData.CurrGcdAbilityCount = 0;
            // slot.Add(Devilment.GetSpell());
            return;
        }
        slot.Wait2NextGcd = true;
        slot.Add(Core.Resolve<JobApi_Dancer>().NextStep.GetSpell());
        if (Core.Me.HasAura(TechnicalStep) && Core.Resolve<JobApi_Dancer>().CompleteSteps == 2)
        {
            if (DancerBattleData.Instance.TechnicalStepCount == 1 && !DancerSettings.Instance.UsePotionInOpener)
                return;
            if (DancerRotationEntry.QT.GetQt(QTKey.UsePotion) && ItemHelper.CheckCurrJobPotion())
                slot.Add(Spell.CreatePotion());
        }
        AI.Instance.BattleData.CurrGcdAbilityCount = 0;
    }
}