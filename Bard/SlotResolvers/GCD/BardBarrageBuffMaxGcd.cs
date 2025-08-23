using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardBarrageBuffMaxGcd : ISlotResolver
{
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;
    
    public int Check()
    {
        if (Core.Me.HasLocalPlayerAura(BarrageBuff) && !Core.Me.HasMyAuraWithTimeleft(BarrageBuff, 3000))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BardUtil.GetBaseGcd());
    }
}