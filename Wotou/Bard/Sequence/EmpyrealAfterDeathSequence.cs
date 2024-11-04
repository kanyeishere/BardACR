using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.Sequence;

// 为了解决玩家死亡后复活/释放lb后，重新进入战斗时，九天连箭与GCD技能同时预备，此时无法正确判断技能释放顺序的问题
// 暂时弃用此seq，用always队列解决
public class EmpyrealAfterDeathSequence: ISlotSequence
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    //private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    
    public int StartCheck()
    {
        if (EmpyrealArrow.IsUnlockWithCDCheck() &&
            //!WanderersMinuet.IsUnlockWithCDCheck() &&
            GCDHelper.GetGCDCooldown() == 0)
            return 1;
        return -1;
    }

    public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>
    {
        Step0,
    };
    
    private static void Step0(Slot slot)
    {
        slot.Add(EmpyrealArrow.GetSpell());
    }
}