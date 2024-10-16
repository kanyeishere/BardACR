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
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (!Core.Me.HasLocalPlayerAura(ResonantArrowReady))
            return -1;
        if (Util.HasAllPartyBuff())
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