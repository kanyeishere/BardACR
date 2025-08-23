
#nullable disable
using System.Reflection;

namespace Wotou.Bard.Data;

public class BardDefinesData
{
  public static class Buffs
  {
    public const ushort HawksEye = 3861;
    public const ushort RagingStrikes = 125;
    public const ushort VenomousBite = 124;
    public const ushort Windbite = 129;
    public const ushort Barrage = 128;
    public const ushort Peloton = 1199;
    public const ushort CausticBite = 1200;
    public const ushort Stormbite = 1201;
    public const ushort TheWanderersMinuet = 2216;
    public const ushort MagesBallad = 2217;
    public const ushort ArmysPaeon = 2218;
    public const ushort BattleVoice = 141;
    public const ushort BlastArrowReady = 2692;
    public const ushort ResonantArrowReady = 3862;
    public const ushort RadiantFinale = 2964;
    public const ushort PlayingRadiantFinale = 2722;
    public const ushort RadiantEncoreReady = 3863;
    public const ushort Troubadour = 1934;
    public const ushort ShieldSamba = 1826;
    public const ushort Tactician = 1951;
  }

  public static class Spells
  {
    public const uint HeavyShot = 97;
    public const uint ArmsLength = 7548;
    public const uint Peloton = 7557;
    public const uint StraightShot = 98;
    public const uint Bloodletter = 110;
    public const uint HeadGraze = 7551;
    public const uint Potion = 846;
    public const uint PitchPerfect = 7404;
    public const uint EmpyrealArrow = 3558;
    public const uint Sidewinder = 3562;
    public const uint RefulgentArrow = 7409;
    public const uint BurstShot = 16495;
    public const uint QuickNock = 106;
    public const uint RainofDeath = 117;
    public const uint Shadowbite = 16494;
    public const uint ApexArrow = 16496;
    public const uint Ladonsbite = 25783;
    public const uint BlastArrow = 25784;
    public const uint VenomousBite = 100;
    public const uint Windbite = 113;
    public const uint IronJaws = 3560;
    public const uint CausticBite = 7406;
    public const uint Stormbite = 7407;
    public const uint RagingStrikes = 101;
    public const uint Barrage = 107;
    public const uint BattleVoice = 118;
    public const uint RadiantFinale = 25785;
    public const uint MagesBallad = 114;
    public const uint ArmysPaeon = 116;
    public const uint TheWanderersMinuet = 3559;
    public const uint RepellingShot = 112;
    public const uint TheWardensPaean = 3561;
    public const uint NaturesMinne = 7408;
    public const uint SecondWind = 7541;
    public const uint Troubadour = 7405;
    public const uint HeartBreak = 36975;
    public const uint ResonantArrow = 36976;
    public const uint RadiantEncore = 36977;
    public const uint WideVolley = 36974;
    public const uint FootGraze = 7553;
    public const uint LegGraze = 7554;
  }
  
  public static class 诗人技能
  {
    public const uint 强力射击 = 97;
    public const uint 亲疏自行 = 7548;
    public const uint 速行 = 7557;
    public const uint 直线射击 = 98;
    public const uint 失血箭 = 110;
    public const uint 伤头 = 7551;
    public const uint 完美音调 = 7404;
    public const uint 九天连箭 = 3558;
    public const uint 侧风诱导箭 = 3562;
    public const uint 辉煌箭 = 7409;
    public const uint 爆发射击 = 16495;
    public const uint 连珠箭 = 106;
    public const uint 死亡箭雨 = 117;
    public const uint 影噬箭 = 16494;
    public const uint 绝峰箭 = 16496;
    public const uint 百首龙牙箭 = 25783;
    public const uint 爆破箭 = 25784;
    public const uint 毒咬箭 = 100;
    public const uint 风蚀箭 = 113;
    public const uint 伶牙俐齿 = 3560;
    public const uint 烈毒咬箭 = 7406;
    public const uint 狂风蚀箭 = 7407;
    public const uint 猛者强击 = 101;
    public const uint 纷乱箭 = 107;
    public const uint 战斗之声 = 118;
    public const uint 光明神的最终乐章 = 25785;
    public const uint 贤者的叙事谣 = 114;
    public const uint 军神的赞美歌 = 116;
    public const uint 放浪神的小步舞曲 = 3559;
    public const uint 后跃射击 = 112;
    public const uint 光阴神的礼赞凯歌 = 3561;
    public const uint 大地神的抒情恋歌 = 7408;
    public const uint 内丹 = 7541;
    public const uint 行吟 = 7405;
    public const uint 碎心箭 = 36975;
    public const uint 共鸣箭 = 36976;
    public const uint 光明神的返场余音 = 36977;
    public const uint 广域射击 = 36974;
    public const uint 伤足 = 7553;
    public const uint 伤腿 = 7554;
  }
  
  private static Dictionary<string, uint> skillDictionary;
    
  public static void InitializeDictionary()
  {
    if (skillDictionary == null)
    {
      skillDictionary = new Dictionary<string, uint>();
      foreach (var field in typeof(诗人技能).GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        if (field.FieldType == typeof(uint))
        {
          skillDictionary.Add(field.Name, (uint)field.GetValue(null));
        }
      }
      foreach (var field in typeof(Spells).GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        if (field.FieldType == typeof(uint))
        {
          skillDictionary.Add(field.Name, (uint)field.GetValue(null));
        }
      }
      skillDictionary.Add("3级巧力之宝药", 45996);
      skillDictionary.Add("2级巧力之宝药", 44163);
      skillDictionary.Add("1级巧力之宝药", 44158);
      skillDictionary.Add("8级巧力之幻药", 39728);
      skillDictionary.Add("7级巧力之幻药", 37841);
    }
  }
    
  public static Dictionary<string, uint> GetMatchingSkills(string searchQuery)
  {
    // 将查询和技能名称转换为小写，避免大小写影响匹配
    var lowerCaseQuery = searchQuery.ToLower();
    return skillDictionary.Where(skill => skill.Key.ToLower().Contains(lowerCaseQuery))
      .ToDictionary(skill => skill.Key, skill => skill.Value);
  }
}

