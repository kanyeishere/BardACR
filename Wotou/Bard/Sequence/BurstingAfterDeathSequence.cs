using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

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
    private const uint StormBite = BardDefinesData.Spells.Stormbite;
    private const uint CausticBite = BardDefinesData.Spells.CausticBite;
    
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint WindBiteDot = BardDefinesData.Buffs.Windbite;
    private const uint StormBiteDot = BardDefinesData.Buffs.Stormbite;
    private const uint VenomousBiteDot = BardDefinesData.Buffs.VenomousBite;
    private const uint CausticBiteDot = BardDefinesData.Buffs.CausticBite;
    
    public int StartCheck()
    {
        if (AI.Instance.BattleData.CurrBattleTimeInMs < 5000)
            return -9;
        if (EmpyrealArrow.IsUnlockWithCDCheck() && 
            WanderersMinuet.IsUnlockWithCDCheck() &&
            GCDHelper.GetGCDCooldown() == 0 && 
            //BardRotationEntry.QT.GetQt(QTKey.StrongAlign) &&
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
        return BardUtil.GetBaseGcd();
    }
    
    private static void Step0(Slot slot)
    {
        slot.Add(WanderersMinuet.GetSpell());
        if (!Core.Me.GetCurrTarget().HasLocalPlayerAura(WindBiteDot) && 
            !Core.Me.GetCurrTarget().HasLocalPlayerAura(StormBiteDot) &&
            BardRotationEntry.QT.GetQt(QTKey.DOT))
            slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(StormBite).GetSpell());
        else if (BardBattleData.Instance.DotBlackList.Contains(Core.Me.GetCurrTarget().DataId) ||
                 DotBlacklistHelper.IsBlackList(Core.Me.GetCurrTarget()))
            slot.Add(GetBaseGcd());
        else if (!Core.Me.GetCurrTarget().HasLocalPlayerAura(VenomousBiteDot) &&
                 !Core.Me.GetCurrTarget().HasLocalPlayerAura(CausticBiteDot) &&
                 BardRotationEntry.QT.GetQt(QTKey.DOT))
            slot.Add(CausticBite.GetSpell());
        else
            slot.Add(GetBaseGcd());
    }
}