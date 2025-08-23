using System.Reflection;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;
using Wotou.Common;

namespace Wotou.Bard;

public static class BardQtHotkeyRegistry
{
    public record QtDef(
        string Key,              // 中文值（UI/存档Key）
        bool Default,            // 默认值
        string Description,      // 描述
        Action<bool>? Callback,  // 回调
        string EnglishAlias,     // 英文别名（变量名小写）
        string ChineseAlias      // 中文别名（变量值）
    );

    public static readonly QtDef[] Qts;

    public static readonly Dictionary<string,string> QtAliasToKey;

    public record HotkeyDef(
        string Id, 
        string LabelZh, 
        Func<IHotkeyResolver> Factory,
        string EnglishAlias, 
        string ChineseAlias
    );

    public static readonly HotkeyDef[] Hotkeys;

    public static readonly Dictionary<string,string> HotkeyAliasToId;

    static BardQtHotkeyRegistry()
    {
        Qts = new[]
        {
            NewQt(nameof(QTKey.Aoe), true, "是否使用AOE", null),
            NewQt(nameof(QTKey.Burst), true, "控制猛者，双团辅及纷乱箭的使用", _ => BardRotationEntry.OnClickBurstQT()),
            NewQt(nameof(QTKey.BurstWithWanderer), true, "爆发是否对齐旅神", _ => BardRotationEntry.OnClickBurstWithWandererQT()),
            NewQt(nameof(QTKey.StrongAlign), true, "不会因GCD时间变化而延后爆发，绝本有上天的阶段建议关闭", BardRotationEntry.OnClickStrongAlign),
            NewQt(nameof(QTKey.Apex), true, "是否使用绝峰箭", null),
            NewQt(nameof(QTKey.HeartBreak), true, "是否攒碎心箭进团辅", null),
            NewQt(nameof(QTKey.DOT), true, "是否使用DOT", null),
            NewQt(nameof(QTKey.Song), true, "是否使用歌曲", null),
            NewQt(nameof(QTKey.NatureMinne), true, "大地神自动对齐秘策/活化/中间学派", null),
            NewQt(nameof(QTKey.UsePotion), false, "是否使用爆发药水", null),
            NewQt(nameof(QTKey.Debug), false, "是否打印调试信息", null),
            NewQt(nameof(QTKey.EmpyrealArrow), true, "是否使用九天", null),
            NewQt(nameof(QTKey.Sidewinder), true, "是否使用侧风", null),
            NewQt(nameof(QTKey.EmpyrealArrowBeforeGcd), false, "Boss上天后，落地第一个技能是否使用九天", null),
            NewQt(nameof(QTKey.ClearHawkEyesBuffBeforeDots), true, "在上毒前是否清空鹰眼Buff", null),
            NewQt(nameof(QTKey.AutoWardensPaean), true, "是否开启自动驱散", null),
            NewQt(nameof(QTKey.SmartAoeTarget), false, "是否智能选择AOE目标", null),
        };

        QtAliasToKey = Qts
            .SelectMany(q => new[]
            {
                (alias: q.EnglishAlias, key: q.Key),
                (alias: q.ChineseAlias, key: q.Key)
            })
            .GroupBy(x => x.alias, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Last().key, StringComparer.OrdinalIgnoreCase);
        
        Hotkeys = new[]
        {
            NewHotkey("ArmsLength", "防击退", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.ArmsLength, SpellTargetType.Target)),
            NewHotkey("IronJaws", "续毒", () => new IronJawsHotkeyResolver(BardDefinesData.Spells.IronJaws, SpellTargetType.Target)),
            NewHotkey("SecondWind", "内丹", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.SecondWind, SpellTargetType.Target)),
            NewHotkey("Troubadour", "行吟", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.Troubadour, SpellTargetType.Target)),
            NewHotkey("NaturesMinne", "大地神", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.NaturesMinne, SpellTargetType.Target)),
            NewHotkey("Run", "疾跑", () => new HotKeyResolver_疾跑()),
            NewHotkey("RepellingShot", "后跳", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.RepellingShot, SpellTargetType.Target)),
            NewHotkey("Potion", "爆发药", () => new HotKeyResolver_Potion()),
            NewHotkey("LimitBreak", "极限技", () => new HotKeyResolver_LB()),
            NewHotkey("StopMove", "停止自动移动", () => new StopMoveHotkeyResolver()),
            NewHotkey("ApexArrow", "绝峰箭", () => new ApexArrowHotkeyResolver(BardDefinesData.Spells.ApexArrow, SpellTargetType.Target)),
            NewHotkey("HeadGraze", "伤头", () => new MyNormalSpellHotKeyResolver(BardDefinesData.Spells.HeadGraze, SpellTargetType.Target)),
        };

        HotkeyAliasToId = Hotkeys
            .SelectMany(h => new[]
            {
                (alias: h.EnglishAlias, id: h.Id),
                (alias: h.ChineseAlias, id: h.Id)
            })
            .ToDictionary(x => x.alias, x => x.id, StringComparer.OrdinalIgnoreCase);
    }

    // 工厂：自动取 QTKey 变量名转小写作英文别名，变量值作中文别名
    private static QtDef NewQt(string qtFieldName, bool def, string desc, Action<bool>? cb)
    {
        var field = typeof(QTKey).GetField(qtFieldName, BindingFlags.Public | BindingFlags.Static);
        var cnValue = field?.GetValue(null)?.ToString() ?? qtFieldName;
        return new QtDef(cnValue, def, desc, cb, qtFieldName.ToLowerInvariant(), cnValue);
    }

    // 工厂：自动取 Id 转小写作英文别名，LabelZh 作中文别名
    private static HotkeyDef NewHotkey(string id, string labelZh, Func<IHotkeyResolver> factory)
    {
        return new HotkeyDef(id, labelZh, factory, id.ToLowerInvariant(), labelZh);
    }

    // ---------- 辅助方法 ----------
    public static IHotkeyResolver? CreateResolver(string id)
        => Hotkeys.FirstOrDefault(h => string.Equals(h.Id, id, StringComparison.OrdinalIgnoreCase))?.Factory();

    public static string? ParseQtKey(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        return QtAliasToKey.TryGetValue(input.Trim().ToLowerInvariant(), out var key) ? key : null;
    }
}
