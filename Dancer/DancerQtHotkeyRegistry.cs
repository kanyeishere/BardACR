using System.Reflection;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;
using Wotou.Common;

namespace Wotou.Dancer
{
    public static class DancerQtHotkeyRegistry
    {
        public record QtDef(
            string Key,              // 中文值（UI/存档Key）
            bool Default,            // 默认值
            string Description,      // 描述
            Action<bool>? Callback,  // 回调（如需的话，可填入触发逻辑）
            string EnglishAlias,     // 英文别名（变量名小写）
            string ChineseAlias      // 中文别名（变量值）
        );

        public static readonly QtDef[] Qts;
        public static readonly Dictionary<string, string> QtAliasToKey;

        public record HotkeyDef(
            string Id,
            string LabelZh,
            Func<IHotkeyResolver> Factory,
            string EnglishAlias,
            string ChineseAlias
        );

        public static readonly HotkeyDef[] Hotkeys;
        public static readonly Dictionary<string, string> HotkeyAliasToId;

        static DancerQtHotkeyRegistry()
        {
            // === QT 列表（与 DancerRotationEntry.DefaultQtValues 对齐） ===
            Qts = new[]
            {
                NewQt(nameof(QTKey.UsePotion),        false, "是否使用爆发药", null),
                NewQt(nameof(QTKey.Aoe),               true,  "是否使用AOE", null),
                NewQt(nameof(QTKey.StrongAlign),       true,  "不会因多打GCD而延后大舞，绝本有上天的阶段建议关闭", null),
                NewQt(nameof(QTKey.TechnicalStep),     true,  "是否使用技巧舞步与进攻之探戈", null),
                NewQt(nameof(QTKey.StandardStep),      true,  "是否使用标准舞步与结束动作", null),
                NewQt(nameof(QTKey.Flourish),          true,  "是否使用百花争艳", null),
                NewQt(nameof(QTKey.SaberDance),        true,  "是否使用剑舞与拂晓舞", null),
                NewQt(nameof(QTKey.FanDance),          true,  "是否使用扇舞", null),
                NewQt(nameof(QTKey.AutoCuringWaltz),   true,  "是否自动使用治疗之华尔兹", null),
                NewQt(nameof(QTKey.FinalBurst),        false, "是否倾泻资源", null),
                NewQt(nameof(QTKey.SmartAoeTarget),    false, "是否智能选择AOE目标", null),
            };

            // 英文/中文别名 => 存档Key
            QtAliasToKey = Qts
                .SelectMany(q => new[]
                {
                    (alias: q.EnglishAlias, key: q.Key),
                    (alias: q.ChineseAlias, key: q.Key)
                })
                .GroupBy(x => x.alias, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Last().key, StringComparer.OrdinalIgnoreCase);

            // === 热键清单（与 DancerRotationEntry.BuildQt 中 AddHotkey 对齐） ===
            Hotkeys = new[]
            {
                NewHotkey("ArmsLength", "防击退",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.ArmsLength, SpellTargetType.Target)),

                NewHotkey("SecondWind", "内丹",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.SecondWind, SpellTargetType.Self)),

                NewHotkey("ShieldSamba", "桑巴",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.ShieldSamba, SpellTargetType.Self)),

                NewHotkey("CuringWaltz", "华尔兹",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.CuringWaltz, SpellTargetType.Self)),

                NewHotkey("ImprovisationToggle", "秒开关即兴",
                    () => new ImprovisationHotkeyResolver()),

                NewHotkey("Run", "疾跑",
                    () => new HotKeyResolver_疾跑()),

                NewHotkey("EnAvant", "前冲步",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.EnAvant, SpellTargetType.Self)),

                NewHotkey("Potion", "爆发药",
                    () => new HotKeyResolver_Potion()),

                NewHotkey("LimitBreak", "极限技",
                    () => new HotKeyResolver_LB()),

                NewHotkey("StopMove", "停止自动移动",
                    () => new StopMoveHotkeyResolver()),

                NewHotkey("HeadGraze", "伤头",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.HeadGraze, SpellTargetType.Target)),
            };

            // 英文/中文别名 => 热键 Id
            HotkeyAliasToId = Hotkeys
                .SelectMany(h => new[]
                {
                    (alias: h.EnglishAlias, id: h.Id),
                    (alias: h.ChineseAlias, id: h.Id)
                })
                .ToDictionary(x => x.alias, x => x.id, StringComparer.OrdinalIgnoreCase);
        }

        // ---------- 工厂：QT ----------
        // 自动取 QTKey 变量名转小写作英文别名，变量值作中文别名
        private static QtDef NewQt(string qtFieldName, bool def, string desc, Action<bool>? cb)
        {
            var field = typeof(QTKey).GetField(qtFieldName, BindingFlags.Public | BindingFlags.Static);
            var cnValue = field?.GetValue(null)?.ToString() ?? qtFieldName;
            return new QtDef(
                Key: cnValue,
                Default: def,
                Description: desc,
                Callback: cb,
                EnglishAlias: qtFieldName.ToLowerInvariant(),
                ChineseAlias: cnValue
            );
        }

        // ---------- 工厂：Hotkey ----------
        // 自动取 Id 转小写作英文别名，LabelZh 作中文别名
        private static HotkeyDef NewHotkey(string id, string labelZh, Func<IHotkeyResolver> factory)
        {
            return new HotkeyDef(
                Id: id,
                LabelZh: labelZh,
                Factory: factory,
                EnglishAlias: id.ToLowerInvariant(),
                ChineseAlias: labelZh
            );
        }

        // ---------- 辅助方法 ----------
        public static IHotkeyResolver? CreateResolver(string id)
            => Hotkeys.FirstOrDefault(h => string.Equals(h.Id, id, StringComparison.OrdinalIgnoreCase))?.Factory();

        /// <summary>
        /// 将用户输入（英文别名 / 中文别名）解析为真正的 QT 存档 Key（中文值）
        /// </summary>
        public static string? ParseQtKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            return QtAliasToKey.TryGetValue(input.Trim().ToLowerInvariant(), out var key) ? key : null;
        }
    }
}
