using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;

namespace Wotou.Bard.Sequence;

// 为了解决玩家死亡后复活，重新进入战斗时，九天连箭与旅神歌同时CD预备，此时无法正确判断技能释放顺序的问题
public class BurstingAfterDeathSequence: ISlotSequence
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
    private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
    private const uint BurstShot = BardDefinesData.Spells.BurstShot;
    private const uint Ladonsbite = BardDefinesData.Spells.Ladonsbite;
    
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    
    public int StartCheck()
    {
        if (EmpyrealArrow.IsUnlockWithCDCheck() && 
            WanderersMinuet.IsUnlockWithCDCheck() &&
            BardRotationEntry.QT.GetQt(QTKey.StrongAlign) &&
            BardRotationEntry.QT.GetQt(QTKey.BurstWithWanderer) &&
            BardRotationEntry.QT.GetQt(QTKey.Song))
            return 1;
        return -1;
    }

    public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>
    {
        Step0,
    };
    
    private static Spell GetBaseGcd()
    {
        if (Core.Me.HasAura(HawkEyeBuff))
        {
            if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25,5) > 1 && BardRotationEntry.QT.GetQt("AOE"))
                return Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).GetSpell();
            return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
        }
      
        if (TargetHelper.GetEnemyCountInsideSector(Core.Me, Core.Me.GetCurrTarget(), 12, 90) > 1  && BardRotationEntry.QT.GetQt("AOE"))
            return Core.Resolve<MemApiSpell>().CheckActionChange(Ladonsbite).GetSpell();
        return Core.Resolve<MemApiSpell>().CheckActionChange(BurstShot).GetSpell();
    }
    
    private static void Step0(Slot slot)
    {
        slot.Add(WanderersMinuet.GetSpell());
        slot.Add(GetBaseGcd());
    }
}