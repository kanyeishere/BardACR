﻿using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerStarfallDanceGCD : ISlotResolver
{
    private const uint StarfallDance = DancerDefinesData.Spells.StarfallDance;
    private const uint FlourishingStarfall = DancerDefinesData.Buffs.FlourishingStarfall;
    
    public int Check()
    {
        if (!StarfallDance.IsReady())
            return -1;
        if (!Core.Me.HasAura(FlourishingStarfall))
            return -2;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerDefinesData.Spells.StarfallDance.GetSpell());
    }
}