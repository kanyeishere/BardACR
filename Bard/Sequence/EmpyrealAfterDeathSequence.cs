using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.Sequence;

// 为了解决玩家死亡后复活/释放lb后，重新进入战斗时，九天连箭与GCD技能同时预备，此时无法正确判断技能释放顺序的问题
// 暂时弃用此seq，用always队列解决
public class EmpyrealAfterDeathSequence: ISlotSequence
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
    private const uint BurstShot = BardDefinesData.Spells.BurstShot;
    private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
    private const uint Ladonsbite = BardDefinesData.Spells.Ladonsbite;
    private const uint WindBite = BardDefinesData.Spells.Windbite;
    private const uint VenomousBite = BardDefinesData.Spells.VenomousBite;
    private const uint PitchPerfect = BardDefinesData.Spells.PitchPerfect;
    private const uint ApexArrow = BardDefinesData.Spells.ApexArrow;
    private const uint IronJaws = BardDefinesData.Spells.IronJaws;
    
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;
    private const uint WindBiteDot = BardDefinesData.Buffs.Windbite;
    private const uint StormBiteDot = BardDefinesData.Buffs.Stormbite;
    private const uint VenomousBiteDot = BardDefinesData.Buffs.VenomousBite;
    private const uint CausticBiteDot = BardDefinesData.Buffs.CausticBite;
    private const Song Wanderer = Song.Wanderer;
    
    public int StartCheck()
    {
        if (AI.Instance.BattleData.CurrBattleTimeInMs < 5000)
            return -9;
        if (EmpyrealArrow.IsUnlockWithCDCheck() &&
            BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) &&
            GCDHelper.GetGCDCooldown() == 0 && 
            BardUtil.HasNoPartyBuff())
            return 1;
        return -1;
    }
    
    public int StopCheck(int index)
    { 
        if (BardUtil.HasAnyPartyBuff())
            return 1;
        return -1;
    }

    public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>
    {
        Step0,
        Step1,
        Step2,
        Step3
    };
    
    private static void Step0(Slot slot)
    {
        if (BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrowBeforeGcd))
        {
            if (Core.Resolve<JobApi_Bard>().Repertoire >= 2 &&
                Core.Resolve<JobApi_Bard>().ActiveSong == Wanderer)
                slot.Add(PitchPerfect.GetSpell());
            slot.Add(EmpyrealArrow.GetSpell());
        } 
    }
    
    private static void Step1(Slot slot)
    {
        if (BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrowBeforeGcd))
        {
            if (Core.Resolve<JobApi_Bard>().Repertoire == 3 &&
                Core.Resolve<JobApi_Bard>().ActiveSong == Wanderer)
                slot.Add(PitchPerfect.GetSpell());
        }
    }
    
    private static void Step2(Slot slot)
    {
        var partyBuffCountdown  = BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalSeconds;
        if (!BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod &&
            Core.Me.Level >= 80 &&
            Core.Resolve<JobApi_Bard>().SoulVoice >= 95 &&
            BardRotationEntry.QT.GetQt(QTKey.Apex) &&
            BardUtil.HasNoPartyBuff() &&
            partyBuffCountdown >= 39)
            slot.Add(ApexArrow.GetSpell());
        
        // 绝伊甸 p1 & p2 特化
        else if (!BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod && 
                 Core.Me.Level >= 80 && 
                 Core.Resolve<MemApiZoneInfo>().GetCurrTerrId() == 1238 && 
                 Core.Resolve<MemApiZoneInfo>().GetWeatherId() <= 3 && 
                 Core.Resolve<JobApi_Bard>().SoulVoice >= 80 && 
                 BardRotationEntry.QT.GetQt(QTKey.Apex) && 
                 BardUtil.HasNoPartyBuff() && 
                 partyBuffCountdown >= 39)
            slot.Add(ApexArrow.GetSpell());
        
        else if (!BardBattleData.Instance.DotBlackList.Contains(Core.Me.GetCurrTarget().DataId) && 
                 !DotBlacklistHelper.IsBlackList(Core.Me.GetCurrTarget()))
        {
            if (!Core.Me.GetCurrTarget().HasLocalPlayerAura(WindBiteDot) &&
                !Core.Me.GetCurrTarget().HasLocalPlayerAura(StormBiteDot) &&
                BardRotationEntry.QT.GetQt(QTKey.DOT))
                slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).GetSpell());
            else if (!Core.Me.GetCurrTarget().HasLocalPlayerAura(CausticBiteDot) &&
                     !Core.Me.GetCurrTarget().HasLocalPlayerAura(VenomousBiteDot) &&
                     BardRotationEntry.QT.GetQt(QTKey.DOT))
                slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(VenomousBite).GetSpell());
            else if (Core.Me.GetCurrTarget() != null &&
                     ((Core.Me.GetCurrTarget().HasLocalPlayerAura(CausticBiteDot) &&
                       !Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(CausticBiteDot, 5500)) ||
                      (Core.Me.GetCurrTarget().HasLocalPlayerAura(StormBiteDot) &&
                       !Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(StormBiteDot, 5500)) ||
                      (Core.Me.GetCurrTarget().HasLocalPlayerAura(VenomousBiteDot) &&
                       !Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(VenomousBiteDot, 5500)) ||
                      (Core.Me.GetCurrTarget().HasLocalPlayerAura(WindBiteDot) &&
                       !Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(WindBiteDot, 5500))))
                slot.Add(IronJaws.GetSpell());
            else
                slot.Add(GetBaseGcd());
        }
        else
            slot.Add(GetBaseGcd());
    }

    private static void Step3(Slot slot)
    {
        if (Core.Resolve<JobApi_Bard>().Repertoire >= 2 && 
            Core.Resolve<JobApi_Bard>().ActiveSong == Wanderer &&
            !BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrowBeforeGcd))
            slot.Add(PitchPerfect.GetSpell());
    }
    
    private static Spell GetBaseGcd()
    {
        return BardUtil.GetBaseGcd();
    }
}