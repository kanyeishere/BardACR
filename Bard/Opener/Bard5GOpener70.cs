

using System.Numerics;
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

#nullable enable
#nullable enable
namespace Wotou.Bard.Opener;

/// <summary>
/// 表示Bard第三阶段量规的开启序列，专门用于70到80级。
/// 这个类定义了执行开启的步骤和条件。
/// </summary>
/// 
/// 作者: Nag0mi
public class Bard5GOpener70 : IOpener
{
  // 定义Bard关键技能和法术的常量
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
  
  // 定义与Bard技能相关的关键增益和减益效果的常量
  private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
  private const uint CausticBiteDot = BardDefinesData.Buffs.CausticBite;
  private const uint VenomousBiteDot = BardDefinesData.Buffs.VenomousBite;
  private const uint WindBiteDot = BardDefinesData.Buffs.Windbite;
  private const uint StormBiteDot = BardDefinesData.Buffs.Stormbite;
  private const uint RadiantEncoreReady = BardDefinesData.Buffs.RadiantEncoreReady;
  
  /// <summary>
  /// 检查是否应该开始开启基于各种条件。
  /// </summary>
  /// <returns>一个整数，指示是否应该开始开启以及原因。</returns>
  public int StartCheck()
  {
    // 检查由于每日模式启用是否应跳过开启
    if (BardSettings.Instance.IsDailyMode)
      return -1;
    // 检查当前战斗时间是否超过开始开启的阈值
    if (AI.Instance.BattleData.CurrBattleTimeInMs > 3000L)
      return -9;
    // 检查关键技能是否在冷却中，这将阻止开启的开始
    if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds > 1000 || 
        BattleVoice.GetSpell().Cooldown.TotalMilliseconds > 1000 ||
        Core.Resolve<MemApiSpell>().GetCooldown(BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong)).TotalSeconds > 0.0)
      return -4;
    // 如果上述条件都不满足，则可以开始开启
    return 0;
  }

  /// <summary>
  /// 检查是否应该停止开启的方法存根。
  /// </summary>
  /// <returns>始终返回-1，表示不应停止开启。</returns>
  public int StopCheck() => -1;

  // 定义开启的动作（步骤）序列
  public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>
  {
    Step0,
    Step1,
    Step2,
    Step3,
    Step4
  };

  /// <summary>
  /// 初始化开启的倒计时，目前未使用。
  /// </summary>
  /// <param name="countDownHandler">用于管理倒计时的倒计时处理器。</param>
  public void InitCountDown(CountDownHandler countDownHandler)
  {
    /*BardBattleData.Instance.UseCountdown = true;
    countDownHandler.AddAction(BardSettings.Instance.OpenerTime, Core.Resolve<MemApiSpell>().CheckActionChange(WindBite));*/
  }

  // 获取Bard基础gcd技能的实用方法
  private static Spell GetBaseGcd()
  {
    return BardUtil.GetBaseGcd();
  }
  
  // 根据战斗条件确定适当的Heart Break技能
  private static Spell GetHeartBreakSpell()
  {
    if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1  && BardRotationEntry.QT.GetQt("AOE"))
      return Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).GetSpell();
    return Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell();
  }

  // 开启序列的第0步：准备战斗开始时使用特定技能并进行检查
  private static void Step0(Slot slot)
  {
    // 如果目标上没有Wind Bite或Storm Bite减益效果，并且它们最近没有被使用过，则应用Wind Bite
    if (!Core.Me.GetCurrTarget().HasLocalPlayerAura(WindBiteDot) &&
        !Core.Me.GetCurrTarget().HasLocalPlayerAura(StormBiteDot) && 
        !Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).RecentlyUsed(3000) &&
        !Core.Resolve<MemApiSpell>().CheckActionChange(StormBite).RecentlyUsed(3000))
      slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).GetSpell());
    // 尝试使用设置中的第一个歌曲
    if (BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsUnlockWithCDCheck())
      slot.Add(BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell());
    else if (BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsUnlockWithCDCheck())
      slot.Add(BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell());
    else if (BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsUnlockWithCDCheck())
      slot.Add(BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell());
    
  }

  // 开启序列的第一步
  private static void Step1(Slot slot)
  {
    // 如果目标上有Caustic Bite或VenomousBite减益效果，则使用基础gcd
    if (Core.Me.GetCurrTarget().HasLocalPlayerAura(CausticBiteDot) || 
        Core.Me.GetCurrTarget().HasLocalPlayerAura(VenomousBiteDot))
    {
      slot.Add(GetBaseGcd());
    }
    else
      slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(VenomousBite).GetSpell());
    // 如果EmpyrealArrow解锁并且不在冷却中，则使用它
    if (EmpyrealArrow.IsUnlockWithCDCheck())
      slot.Add(EmpyrealArrow.GetSpell());
    // 使用根据情况选择的心碎技能
    slot.Add(GetHeartBreakSpell());
  }

  // 开启序列的第二步
  private static void Step2(Slot slot)
  {
    // 使用基础gcd
    slot.Add(GetBaseGcd());
    // 如果设置了爆发药选项并且允许在开启中使用药水，则添加药水
    if (BardRotationEntry.QT.GetQt("爆发药") && BardSettings.Instance.UsePotionInOpener)
      slot.Add(Spell.CreatePotion());
    // 使用BattleVoice
    
  }
  private static void Step3(Slot slot)
  {
    // 使用基础gcd
    slot.Add(GetBaseGcd());
    // 使用BattleVoice
    
  }

  // 开启序列的第三步
  private static void Step4(Slot slot)
  {
    // 使用基础gcd
    slot.Add(GetBaseGcd());
    slot.Add(BattleVoice.GetSpell());
    // 使用RagingStrikes
    slot.Add(RagingStrikes.GetSpell());
  }
}
