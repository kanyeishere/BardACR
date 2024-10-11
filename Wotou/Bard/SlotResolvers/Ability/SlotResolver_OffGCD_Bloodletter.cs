using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.Ability;

public class SlotResolver_OffGCD_Bloodletter : ISlotResolver
{
    public int Check()
    {
        // 如果没学习 充能数<1 等等情况 就返回 否则就使用
        if (!BardDefinesData.Spells.HeartBreak.IsReady())
        {
            // 建议check方法内每个返回的负数都不一样 方便定位问题
            return -1;
        }

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(BardDefinesData.Spells.HeartBreak.GetSpell(SpellTargetType.Target));
    }
}