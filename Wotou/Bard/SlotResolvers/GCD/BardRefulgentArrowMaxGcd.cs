using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardRefulgentArrowMaxGcd : ISlotResolver
{
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;
    
    private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
    private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
    
    public int Check()
    {
        if (Core.Me.HasLocalPlayerAura(HawkEyeBuff) && (!Core.Me.HasMyAuraWithTimeleft(HawkEyeBuff, 3000)))
            return 1;
        if (Core.Me.HasLocalPlayerAura(BarrageBuff) && (!Core.Me.HasMyAuraWithTimeleft(BarrageBuff, 3000)))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetGcd());
    }

    private static Spell GetGcd()
    {
        if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25,5) > 1  && BardRotationEntry.QT.GetQt("AOE"))
            return Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).GetSpell();
        return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
    }
}