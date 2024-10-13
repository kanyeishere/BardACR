using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardBlastArrowGcd : ISlotResolver
{
    private const uint BlastArrow = BardDefinesData.Spells.BlastArrow;
    
    private const uint BlastArrowReady = BardDefinesData.Buffs.BlastArrowReady;


    public int Check()
    {
        // 有爆炸箭Buff时，直接使用爆炸箭
        if (Core.Me.HasLocalPlayerAura(BlastArrowReady))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BlastArrow.GetSpell());
    }
}