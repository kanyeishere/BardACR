using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerTechnicalStepGcd : ISlotResolver
{
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    private const uint Devilment = DancerDefinesData.Spells.Devilment;
    public int Check()
    {
        if (!TechnicalStep.IsUnlock())
            return -1;
        if (!DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return -1;
        if (Core.Resolve<JobApi_Dancer>().IsDancing)
            return -1;
        // 如果探戈的cd - 大舞的cd > 8000 
        if (Core.Resolve<MemApiSpell>().GetCooldown(Devilment).TotalMilliseconds - Core.Resolve<MemApiSpell>().GetCooldown(TechnicalStep).TotalMilliseconds > 7500)
            return -2;
        if (TechnicalStep.IsUnlockWithCDCheck())
            return 1;
        if (Core.Resolve<MemApiSpell>().GetCooldown(TechnicalStep).TotalMilliseconds <= 1500 &&
            DancerRotationEntry.QT.GetQt(QTKey.StrongAlign))
            return 2;
        return -10;
    }

    public void Build(Slot slot)
    {
        slot.Add(TechnicalStep.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 1;
    }
}