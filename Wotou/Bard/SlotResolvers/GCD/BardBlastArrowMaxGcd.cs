using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardBlastArrowMaxGcd : ISlotResolver
{
    private const uint BlastArrow = BardDefinesData.Spells.BlastArrow;
    
    private const uint BlastArrowReady = BardDefinesData.Buffs.BlastArrowReady;


    public int Check()
    {
        // 有爆炸箭Buff,且Buff剩余时间不足3秒时，使用爆炸箭
        if (Core.Me.HasLocalPlayerAura(BlastArrowReady) && (!Core.Me.HasMyAuraWithTimeleft(BlastArrowReady, 3000)))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BardUtil.GetSmartAoeSpell(BlastArrow, 1));
    }
}
