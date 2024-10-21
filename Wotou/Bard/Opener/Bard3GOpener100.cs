

using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

#nullable enable
namespace Wotou.Bard.Opener;

public class Bard3GOpener100 : IOpener
{
  
  private const uint Barrage = BardDefinesData.Spells.Barrage;
  private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
  private const uint VenomousBite = BardDefinesData.Spells.VenomousBite;
  private const uint WindBite = BardDefinesData.Spells.Windbite;
  private const uint StormBite = BardDefinesData.Spells.Stormbite;
  private const uint HeavyShot = BardDefinesData.Spells.HeavyShot;
  private const uint StraightShot = BardDefinesData.Spells.StraightShot;
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
  
  
  private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
  
  private const uint StormBiteDot = BardDefinesData.Buffs.Stormbite;
  private const uint WindBiteDot = BardDefinesData.Buffs.Windbite;
  private const uint CausticBiteDot = BardDefinesData.Buffs.CausticBite;
  private const uint VenomousBiteDot = BardDefinesData.Buffs.VenomousBite;
  
  private static uint GetSpellBySong(Song song)
  {
    return song switch
    {
      Song.WANDERER => BardDefinesData.Spells.TheWanderersMinuet,
      Song.MAGE => BardDefinesData.Spells.MagesBallad,
      Song.ARMY => BardDefinesData.Spells.ArmysPaeon,
      _ => throw new ArgumentOutOfRangeException(nameof(song), song, null)
    };
  }
  
  public int StartCheck()
  {
    if (AI.Instance.BattleData.CurrBattleTimeInMs > 3000L)
      return -9;
    if (!Barrage.IsReady() || 
        !RagingStrikes.IsReady() || 
        !BattleVoice.IsReady() ||
        (RadiantFinale.IsUnlock() && Core.Resolve<MemApiSpell>().GetCooldown(RadiantFinale).TotalSeconds > 0.0) ||
        Core.Resolve<MemApiSpell>().GetCooldown(GetSpellBySong(BardSettings.Instance.FirstSong)).TotalSeconds > 0.0)
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
    countDownHandler.AddAction(BardSettings.Instance.OpenerTime, PreCastSpell);
  }

  private Spell PreCastSpell()
  {
    return Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).GetSpell();
  }

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
  
  private static Spell GetHeartBreakSpell()
  {
    if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1  && BardRotationEntry.QT.GetQt("AOE"))
      return Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).GetSpell();
    return Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell();
  }

  private static void Step0(Slot slot)
  {
    if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() != Core.Resolve<MemApiSpell>().CheckActionChange(WindBite))
      slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).GetSpell());
    slot.Add(GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell());
    slot.Add(GetHeartBreakSpell());
  }

  private static void Step1(Slot slot)
  {
    if (Core.Me.GetCurrTarget().HasLocalPlayerAura(CausticBiteDot) || 
        Core.Me.GetCurrTarget().HasLocalPlayerAura(VenomousBiteDot))
    {
      slot.Add(GetBaseGcd());
    }
    else
      slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(VenomousBite).GetSpell());
    
    if (BardRotationEntry.QT.GetQt("爆发药") && BardSettings.Instance.UsePotionInOpener)
    {
      slot.Add(Spell.CreatePotion());
      slot.Add(RagingStrikes.GetSpell());
      return;
    }
    
    slot.Add2NdWindowAbility(RagingStrikes.GetSpell());
  }

  private static void Step2(Slot slot)
  {
    slot.Add(GetBaseGcd());
    slot.Add(BattleVoice.GetSpell());
    if (!RadiantFinale.IsUnlock())
      return;
    slot.Add(RadiantFinale.GetSpell());
  }

  private static void Step3(Slot slot)
  {
    slot.Add(GetBaseGcd());
    slot.Add(EmpyrealArrow.GetSpell());
    // slot.Add(Barrage.GetSpell());
  }
}

