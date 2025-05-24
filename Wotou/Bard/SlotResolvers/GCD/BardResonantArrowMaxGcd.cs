using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardResonantArrowMaxGcd : ISlotResolver
{
    private const uint ResonantArrow = BardDefinesData.Spells.ResonantArrow;
    
    private const uint ResonantArrowReady = BardDefinesData.Buffs.ResonantArrowReady;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;

    public int Check()
    {
        if (Core.Me.HasLocalPlayerAura(BarrageBuff) && !Core.Me.HasMyAuraWithTimeleft(BarrageBuff, 3000))
            return -1;
        if (Core.Me.HasLocalPlayerAura(ResonantArrowReady) && (!Core.Me.HasMyAuraWithTimeleft(ResonantArrowReady, 6000)))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BardUtil.GetSmartAoeSpell(ResonantArrow, 1));
    }
}