using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardRadiantEncoreGcd : ISlotResolver
{
    private const uint RadiantEncore = BardDefinesData.Spells.RadiantEncore;
    
    private const uint RadiantEncoreReady = BardDefinesData.Buffs.RadiantEncoreReady;
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (RadiantEncoreReady.IsReady())
            return 1;
        if (Core.Me.HasLocalPlayerAura(RadiantEncoreReady))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(RadiantEncore.GetSpell());
    }
}