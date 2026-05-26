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
        public static readonly QtDef[] Qts;
        public static readonly Dictionary<string, string> QtAliasToKey;

        public static readonly HotkeyDef[] Hotkeys;
        public static readonly Dictionary<string, string> HotkeyAliasToId;

        static DancerQtHotkeyRegistry()
        {
            // === QT 列表（与 DancerRotationEntry.DefaultQtValues 对齐） ===
            Qts = new[]
            {
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.UsePotion),        false, "是否使用爆发药", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Aoe),               true,  "是否使用AOE", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.StrongAlign),       true,  "不会因多打GCD而延后大舞，绝本有上天的阶段建议关闭", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.TechnicalStep),     true,  "是否使用技巧舞步与进攻之探戈", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.StandardStep),      true,  "是否使用标准舞步与结束动作", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.Flourish),          true,  "是否使用百花争艳", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.SaberDance),        true,  "是否使用剑舞与拂晓舞", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.FanDance),          true,  "是否使用扇舞", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.AutoCuringWaltz),   true,  "是否自动使用治疗之华尔兹", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.FinalBurst),        false, "是否倾泻资源", null),
                QtHotkeyRegistryBase.NewQt(typeof(QTKey), nameof(QTKey.SmartAoeTarget),    false, "是否智能选择AOE目标", null),
            };

            // 英文/中文别名 => 存档Key
            QtAliasToKey = QtHotkeyRegistryBase.BuildQtAliasMap(Qts);

            // === 热键清单（与 DancerRotationEntry.BuildQt 中 AddHotkey 对齐） ===
            Hotkeys = new[]
            {
                QtHotkeyRegistryBase.NewHotkey("ArmsLength", "防击退",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.ArmsLength, SpellTargetType.Target)),

                QtHotkeyRegistryBase.NewHotkey("SecondWind", "内丹",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.SecondWind, SpellTargetType.Self)),

                QtHotkeyRegistryBase.NewHotkey("ShieldSamba", "桑巴",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.ShieldSamba, SpellTargetType.Self)),

                QtHotkeyRegistryBase.NewHotkey("CuringWaltz", "华尔兹",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.CuringWaltz, SpellTargetType.Self)),

                QtHotkeyRegistryBase.NewHotkey("ImprovisationToggle", "秒开关即兴",
                    () => new ImprovisationHotkeyResolver()),

                QtHotkeyRegistryBase.NewHotkey("Run", "疾跑",
                    () => new HotKeyResolver_疾跑()),

                QtHotkeyRegistryBase.NewHotkey("EnAvant", "前冲步",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.EnAvant, SpellTargetType.Self)),

                QtHotkeyRegistryBase.NewHotkey("Potion", "爆发药",
                    () => new HotKeyResolver_Potion()),

                QtHotkeyRegistryBase.NewHotkey("LimitBreak", "极限技",
                    () => new HotKeyResolver_LB()),

                QtHotkeyRegistryBase.NewHotkey("StopMove", "停止自动移动",
                    () => new StopMoveHotkeyResolver()),

                QtHotkeyRegistryBase.NewHotkey("HeadGraze", "伤头",
                    () => new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.HeadGraze, SpellTargetType.Target)),
            };

            // 英文/中文别名 => 热键 Id
            HotkeyAliasToId = QtHotkeyRegistryBase.BuildHotkeyAliasMap(Hotkeys);
        }

    // ---------- 辅助方法 ----------
        public static IHotkeyResolver? CreateResolver(string id)
            => QtHotkeyRegistryBase.CreateResolver(Hotkeys, id);

        /// <summary>
        /// 将用户输入（英文别名 / 中文别名）解析为真正的 QT 存档 Key（中文值）
        /// </summary>
        public static string? ParseQtKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            return QtHotkeyRegistryBase.ParseQtKey(QtAliasToKey, input);
        }
    }
}
