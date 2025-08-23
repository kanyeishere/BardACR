using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardRadiantEncoreMaxGcd : ISlotResolver
{
    private const uint RadiantEncore = BardDefinesData.Spells.RadiantEncore;
    
    private const uint RadiantEncoreReady = BardDefinesData.Buffs.RadiantEncoreReady;


    public int Check()
    {
        // 有爆炸箭Buff,且Buff剩余时间不足3秒时，使用爆炸箭
        if (Core.Me.HasLocalPlayerAura(RadiantEncoreReady) && (!Core.Me.HasMyAuraWithTimeleft(RadiantEncoreReady, 6000)))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BardUtil.GetSmartAoeSpell(RadiantEncore, 1));
    }
}
