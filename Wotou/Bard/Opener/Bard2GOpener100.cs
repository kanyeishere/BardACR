using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.Opener;

public class Bard2GOpener100 : IOpener
{
  private const uint Barrage = BardDefinesData.Spells.Barrage;
  private const uint VenomousBite = BardDefinesData.Spells.VenomousBite;
  private const uint WindBite = BardDefinesData.Spells.Windbite;
  private const uint StormBite = BardDefinesData.Spells.Stormbite;
  private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
  private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
  private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
  private const uint HeartBreak = BardDefinesData.Spells.HeartBreak;
  private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
  private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
  private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
  private const uint BurstShot = BardDefinesData.Spells.BurstShot;
  private const uint Ladonsbite = BardDefinesData.Spells.Ladonsbite;
  private const uint RainOfDeath = BardDefinesData.Spells.RainofDeath;
  private const uint LegGraze = BardDefinesData.Spells.LegGraze;
  private const uint RadiantEncore = BardDefinesData.Spells.RadiantEncore;
  
  private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
  private const uint CausticBiteDot = BardDefinesData.Buffs.CausticBite;
  private const uint VenomousBiteDot = BardDefinesData.Buffs.VenomousBite;
  private const uint WindBiteDot = BardDefinesData.Buffs.Windbite;
  private const uint StormBiteDot = BardDefinesData.Buffs.Stormbite;
  private const uint RadiantEncoreReady = BardDefinesData.Buffs.RadiantEncoreReady;
  
  public int StartCheck()
  {
    if (BardSettings.Instance.IsDailyMode)
      return -1;
    if (AI.Instance.BattleData.CurrBattleTimeInMs > 3000L)
      return -9;
    if (!Barrage.GetSpell().IsReadyWithCanCast() || 
        !RagingStrikes.GetSpell().IsReadyWithCanCast() || 
        !BattleVoice.GetSpell().IsReadyWithCanCast() ||
        (RadiantFinale.IsUnlock() && Core.Resolve<MemApiSpell>().GetCooldown(RadiantFinale).TotalSeconds > 0.0) ||
        Core.Resolve<MemApiSpell>().GetCooldown(BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong)).TotalSeconds > 0.0)
      return -4;
    return !BardRotationEntry.QT.GetQt("爆发") || !BardRotationEntry.QT.GetQt("唱歌") ? -10 : 0;
  }

  public int StopCheck() => -1;

  public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>
  {
    Step0,
    Step1,
    Step2,
    Step3
  };

  public void InitCountDown(CountDownHandler countDownHandler)
  {
    /*BardBattleData.Instance.UseCountdown = true;
    countDownHandler.AddAction(BardSettings.Instance.OpenerTime, Core.Resolve<MemApiSpell>().CheckActionChange(WindBite));*/
  }

  private static Spell GetBaseGcd()
  {
    return BardUtil.GetBaseGcd();
  }
  
  private static Spell GetHeartBreakSpell()
  {
    if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1  && BardRotationEntry.QT.GetQt("AOE"))
      return Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).GetSpell();
    return Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell();
  }

  private static void Step0(Slot slot)
  {
    if (!Core.Me.GetCurrTarget().HasLocalPlayerAura(WindBiteDot) &&
        !Core.Me.GetCurrTarget().HasLocalPlayerAura(StormBiteDot) && 
        !Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).RecentlyUsed(3000) &&
        !Core.Resolve<MemApiSpell>().CheckActionChange(StormBite).RecentlyUsed(3000))
      slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).GetSpell());
    if (BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsUnlockWithCDCheck())
      slot.Add(BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell());
    else if (BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsUnlockWithCDCheck())
      slot.Add(BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell());
    else if (BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsUnlockWithCDCheck())
      slot.Add(BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell());
    slot.Add(GetHeartBreakSpell());
  }

  private static void Step1(Slot slot)
  {
    if (Core.Me.GetCurrTarget().HasLocalPlayerAura(CausticBiteDot) || 
        Core.Me.GetCurrTarget().HasLocalPlayerAura(VenomousBiteDot))
      slot.Add(GetBaseGcd());
    else
      slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(VenomousBite).GetSpell());
    
    slot.Add(BattleVoice.GetSpell());
    
    if (!RadiantFinale.IsUnlock())
      return;
    slot.Add(RadiantFinale.GetSpell());
  }

  private static void Step2(Slot slot)
  {
    slot.Add(GetBaseGcd());

    if (BardRotationEntry.QT.GetQt("爆发药") && BardSettings.Instance.UsePotionInOpener)
    {
      slot.Add(Spell.CreatePotion());
      slot.Add(RagingStrikes.GetSpell());
      return;
    }
    if (BardSettings.Instance.ImitateGreenPlayer)
    {
      slot.Add(LegGraze.GetSpell());
      slot.Add(RagingStrikes.GetSpell());
      return;
    }
    slot.Add2NdWindowAbility(RagingStrikes.GetSpell());
  }

  private static void Step3(Slot slot)
  {
    if (RadiantEncore.IsUnlockWithCDCheck() || Core.Me.HasAura(RadiantEncoreReady))
      slot.Add(RadiantEncore.GetSpell());
    else
      slot.Add(GetBaseGcd());
    if (EmpyrealArrow.IsUnlockWithCDCheck())
      slot.Add(EmpyrealArrow.GetSpell());
    // slot.Add(Barrage.GetSpell());
  }
}
