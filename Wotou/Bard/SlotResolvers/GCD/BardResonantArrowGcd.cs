using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardResonantArrowGcd : ISlotResolver
{
    private const uint ResonantArrow = BardDefinesData.Spells.ResonantArrow;
    
    private const uint ResonantArrowReady = BardDefinesData.Buffs.ResonantArrowReady;
    
    public int Check()
    {
        if (!Core.Me.HasLocalPlayerAura(ResonantArrowReady))
            return -1;
        if (BardUtil.HasAllPartyBuff())
            return 1;
        if (Core.Me.HasLocalPlayerAura(ResonantArrowReady) && !Core.Me.HasMyAuraWithTimeleft(ResonantArrowReady, 3000))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(ResonantArrow.GetSpell());
    }
}