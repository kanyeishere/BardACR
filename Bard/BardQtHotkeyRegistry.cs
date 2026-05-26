using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;
using Wotou.Common;

namespace Wotou.Bard;

public static class BardQtHotkeyRegistry
{
    public static readonly QtDef[] Qts;

    public static readonly Dictionary<string,string> QtAliasToKey;

    public static readonly HotkeyDef[] Hotkeys;

    public static readonly Dictionary<string,string> HotkeyAliasToId;

    static BardQtHotkeyRegistry()
    {
        Qts = new[]
        {
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Aoe), true, "是否使用AOE", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Burst), true, "控制猛者，双团辅及纷乱箭的使用", _ => BardRotationEntry.OnClickBurstQT()),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.BurstWithWanderer), true, "爆发是否对齐旅神", _ => BardRotationEntry.OnClickBurstWithWandererQT()),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.StrongAlign), true, "不会因GCD时间变化而延后爆发，绝本有上天的阶段建议关闭", BardRotationEntry.OnClickStrongAlign),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Apex), true, "是否使用绝峰箭", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.HeartBreak), true, "是否攒碎心箭进团辅", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.DOT), true, "是否使用DOT", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Song), true, "是否使用歌曲", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.NatureMinne), true, "大地神自动对齐秘策/活化/中间学派", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.UsePotion), false, "是否使用爆发药水", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Debug), false, "是否打印调试信息", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.EmpyrealArrow), true, "是否使用九天", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Sidewinder), true, "是否使用侧风", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.EmpyrealArrowBeforeGcd), false, "Boss上天后，落地第一个技能是否使用九天", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.ClearHawkEyesBuffBeforeDots), true, "在上毒前是否清空鹰眼Buff", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.AutoWardensPaean), true, "是否开启自动驱散", null),
            QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.SmartAoeTarget), false, "是否智能选择AOE目标", null),
        };

        QtAliasToKey = QtHotkeyRegistryBase.BuildQtAliasMap(Qts);
        
        Hotkeys = new[]
        {
            QtHotkeyRegistryBase.NewHotkey("ArmsLength", "防击退", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.ArmsLength, SpellTargetType.Target)),
            QtHotkeyRegistryBase.NewHotkey("IronJaws", "续毒", () => new IronJawsHotkeyResolver(BardDefinesData.Spells.IronJaws, SpellTargetType.Target)),
            QtHotkeyRegistryBase.NewHotkey("SecondWind", "内丹", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.SecondWind, SpellTargetType.Target)),
            QtHotkeyRegistryBase.NewHotkey("Troubadour", "行吟", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.Troubadour, SpellTargetType.Target)),
            QtHotkeyRegistryBase.NewHotkey("NaturesMinne", "大地神", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.NaturesMinne, SpellTargetType.Target)),
            QtHotkeyRegistryBase.NewHotkey("Run", "疾跑", () => new HotKeyResolver_疾跑()),
            QtHotkeyRegistryBase.NewHotkey("RepellingShot", "后跳", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.RepellingShot, SpellTargetType.Target)),
            QtHotkeyRegistryBase.NewHotkey("Potion", "爆发药", () => new HotKeyResolver_Potion()),
            QtHotkeyRegistryBase.NewHotkey("LimitBreak", "极限技", () => new HotKeyResolver_LB()),
            QtHotkeyRegistryBase.NewHotkey("StopMove", "停止自动移动", () => new StopMoveHotkeyResolver()),
            QtHotkeyRegistryBase.NewHotkey("ApexArrow", "绝峰箭", () => new ApexArrowHotkeyResolver(BardDefinesData.Spells.ApexArrow, SpellTargetType.Target)),
            QtHotkeyRegistryBase.NewHotkey("HeadGraze", "伤头", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.HeadGraze, SpellTargetType.Target)),
        };

        HotkeyAliasToId = QtHotkeyRegistryBase.BuildHotkeyAliasMap(Hotkeys);
    }

    // ---------- 辅助方法 ----------
    public static IHotkeyResolver? CreateResolver(string id)
        => QtHotkeyRegistryBase.CreateResolver(Hotkeys, id);

    public static string? ParseQtKey(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        return QtHotkeyRegistryBase.ParseQtKey(QtAliasToKey, input);
    }
}
