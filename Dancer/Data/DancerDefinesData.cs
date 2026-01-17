using System.Reflection;

namespace Wotou.Dancer.Data;

public class DancerDefinesData
{
    public static class Buffs
    {
        public const ushort
            FlourshingCascade = 1814,
            FlourshingFountain = 1815,
            FlourshingWindmill = 1816,
            FlourshingShower = 1817,
            StandardStep = 1818,
            TechnicalStep = 1819,
            ThreeFoldFanDance = 1820,
            StandardFinish = 1821,
            TechnicalFinish = 1822,
            ClosedPosition = 1823,
            DancePartner = 1824,
            Devilment = 1825,
            ShieldSamba = 1826,
            Improvisation = 1827,
            FlourishingSymmetry = 3017,
            FlourshingFlow = 3018,
            SilkenFlow = 2694,
            SilkenSymmetry = 2693,
            FlourishingFinish = 2698,
            FourfoldFanDance = 2699,
            FlourishingStarfall = 2700,
            LastDanceReady = 3867,
            FinishingMoveReady = 3868,
            DanceOfTheDawnReady = 3869,
            Medicated = 49, 
            Troubadour = 1934,
            Tactician = 1951,
            Peloton = 1199;
    }

    public static class Spells
    {
        public const uint 
            HeadGraze = 7551,
            ArmsLength = 7548,
            Cascade = 15989,
            Fountain = 15990,
            ReverseCascade = 15991,
            Fountainfall = 15992,
            Windmill = 15993,
            Bladeshower = 15994,
            RisingWindmill = 15995,
            Bloodshower = 15996,
            StandardStep = 15997,
            Emboite = 15999,
            Entrechat = 16000,
            Jete = 16001,
            Pirouette = 16002,
            StandardFinish = 16003,
            SaberDance = 16005,
            ClosedPosition = 16006,
            FanDance = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            FanDance4 = 25791,
            EnAvant = 16010,
            Devilment = 16011,
            ShieldSamba = 16012,
            Flourish = 16013,
            Improvisation = 16014,
            ImprovisationFinish = 25789,
            CuringWaltz = 16015,
            SingleStandardFinish = 16191,
            DoubleStandardFinish = 16192,
            Ending = 18073,
            TechnicalStep = 15998,
            SingleTechnicalFinish = 16193,
            DoubleTechnicalFinish = 16194,
            TripleTechnicalFinish = 16195,
            QuadrupleTechnicalFinish = 16196,
            StarfallDance = 25792,
            Tillana = 25790,
            LastDance = 36983,
            FinishingMove = 36984,
            DanceOfTheDawn = 36985,
            Peloton = 7557,
            SecondWind = 7541;
    }
    
    public static class 舞者技能
    {
        public const uint 亲疏自行 = 7548;
        public const uint 伤头 = 7551;
        public const uint 瀑泻 = 15989;
        public const uint 喷泉 = 15990;
        public const uint 逆瀑泻 = 15991;
        public const uint 坠喷泉 = 15992;
        public const uint 风车 = 15993;
        public const uint 落刃雨 = 15994;
        public const uint 升风车 = 15995;
        public const uint 落血雨 = 15996;
        public const uint 标准舞步 = 15997;
        public const uint 技巧舞步 = 15998;
        public const uint 单色标准舞步结束 = 16191;
        public const uint 双色标准舞步结束 = 16192;
        public const uint 单色技巧舞步结束 = 16193;
        public const uint 双色技巧舞步结束 = 16194;
        public const uint 三色技巧舞步结束 = 16195;
        public const uint 四色技巧舞步结束 = 16196;
        public const uint 剑舞 = 16005;
        public const uint 闭式舞姿 = 16006;
        public const uint 解除闭式舞姿 = 18073;
        public const uint 扇舞序 = 16007;
        public const uint 扇舞破 = 16008;
        public const uint 扇舞急 = 16009;
        public const uint 扇舞终 = 25791;
        public const uint 前冲步 = 16010;
        public const uint 进攻之探戈 = 16011;
        public const uint 防守之桑巴 = 16012;
        public const uint 百花争艳 = 16013;
        public const uint 即兴表演 = 16014;
        public const uint 即兴表演结束 = 25789;
        public const uint 治疗之华尔兹 = 16015;
        public const uint 流星舞 = 25792;
        public const uint 提拉纳 = 25790;
        public const uint 落幕舞 = 36983;
        public const uint 结束动作 = 36984;
        public const uint 拂晓舞 = 36985;
        public const uint 内丹 = 7541;
    }
    
    private static Dictionary<string, uint> skillDictionary;
    
    public static void InitializeDictionary()
    {
        if (skillDictionary == null)
        {
            skillDictionary = new Dictionary<string, uint>();
            skillDictionary.Add("4级巧力之宝药", 49235);
            skillDictionary.Add("3级巧力之宝药", 45996);
            skillDictionary.Add("2级巧力之宝药", 44163);
            skillDictionary.Add("1级巧力之宝药", 44158);
            skillDictionary.Add("8级巧力之幻药", 39728);
            skillDictionary.Add("7级巧力之幻药", 37841);
            // 加入你的中文技能类 
            foreach (var field in typeof(舞者技能).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(uint))
                {
                    skillDictionary.Add(field.Name, (uint)field.GetValue(null));
                }
            }
            // 加入你的英文技能类 （可选）
            foreach (var field in typeof(Spells).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(uint))
                {
                    skillDictionary.Add(field.Name, (uint)field.GetValue(null));
                }
            }
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