﻿using AEAssist;
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
    public int Check()
    {
        if (!TechnicalStep.IsUnlock())
            return -1;
        if (!DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return -1;
        if (Core.Resolve<MemApiSpell>().GetCooldown(TechnicalStep).TotalMilliseconds > 1000)
            return -1;
        if (Core.Resolve<JobApi_Dancer>().IsDancing)
            return -1;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(TechnicalStep.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 1;
    }
}