﻿using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerLastDanceGcd : ISlotResolver
{
    private const uint LastDanceReady = DancerDefinesData.Buffs.LastDanceReady;
    private const uint Devilment = DancerDefinesData.Spells.Devilment;
    private const uint LastDance = DancerDefinesData.Spells.LastDance;

    public int Check()
    {
        if (!LastDance.IsReady())
            return -1; 
        if (!Core.Me.HasAura(LastDanceReady))
            return -2;
        if (Core.Me.HasMyAuraWithTimeleft(LastDanceReady, (int)Devilment.GetSpell().Cooldown.TotalMilliseconds + 2500) && 
            DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return -3;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(LastDance.GetSpell());
    }
}