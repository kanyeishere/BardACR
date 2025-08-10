using System.Numerics;
using ImGuiNET;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Utility;

public class DancerCommandWindow
{
    private static (string Type, string ChineseCommand, string EnglishCommand)[]? commandList;

    private static bool HasChinese(string s) => s.Any(ch => ch > 127);

    private static void BuildCommandList()
    {
        // ⚠️ 重要：
        // 这里必须先 new 一个 DancerRotationEventHandler 并调用 BulidQtKeyDictionary()，
        // 否则 qtKeyDictionary 不会被初始化，所有 /xxx_qt 宏指令将无法识别。
        // 因为 qtKeyDictionary 是根据 QTKey 常量反射生成的，
        // 初始化后才能提供“中/英别名 → 规范键”的映射。
        var handler = new DancerRotationEventHandler();
        handler.BulidQtKeyDictionary();

        var rows = new List<(string, string, string)>();

        // Hotkey 行：按“规范名”分组，挑一个中文别名 + 一个英文别名
        foreach (var g in handler.hotkeyDictionary
                     .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                     .GroupBy(kv => kv.Value!, StringComparer.OrdinalIgnoreCase))
        {
            var zh = g.Select(kv => kv.Key).FirstOrDefault(HasChinese) ?? g.First().Key;
            var en = g.Select(kv => kv.Key).FirstOrDefault(k => !HasChinese(k)) ?? g.First().Key;
            rows.Add(("Hotkey", $"/Wotou_DNC {zh}_hk", $"/Wotou_DNC {en}_hk"));
        }

        // QT 行：按“规范键”分组，挑一个中文别名 + 一个英文别名
        foreach (var g in handler.qtKeyDictionary
                     .GroupBy(kv => kv.Value, StringComparer.OrdinalIgnoreCase))
        {
            var zh = g.Select(kv => kv.Key).FirstOrDefault(HasChinese) ?? g.First().Key;
            var en = g.Select(kv => kv.Key).FirstOrDefault(k => !HasChinese(k)) ?? g.First().Key;
            rows.Add(("QT", $"/Wotou_DNC {zh}_qt", $"/Wotou_DNC {en}_qt"));
        }

        // 排序：先按类型，再按英文命令
        commandList = rows
            .OrderBy(r => r.Item1, StringComparer.OrdinalIgnoreCase)
            .ThenBy(r => r.Item3, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static void Draw()
    {
        if (!DancerSettings.Instance.IsOpenCommandWindow)
            return;
        
        BuildCommandList();
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        //ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(mainViewport.Size.X / 2f, mainViewport.Size.Y / 1.5f), ImGuiCond.Always);
        //ImGui.SetNextWindowFocus();

        ImGui.Begin("Dancer Command Help", ref DancerSettings.Instance.IsOpenCommandWindow, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize);

        ImGui.TextWrapped("本ACR提供了完整的指令支持，可以通过/Wotou_DNC 使用快捷指令。\n这些指令可以结合游戏内宏功能使用，非常适合手柄用户快速操作。");
        ImGui.Separator();

        ImGui.Columns(3, "CommandColumns", true);
        ImGui.SetColumnWidth(0, mainViewport.Size.X / 10f);
        ImGui.SetColumnWidth(1, mainViewport.Size.X / 5f);
        ImGui.SetColumnWidth(2, mainViewport.Size.X / 5f);

        ImGui.Text("指令类型");
        ImGui.NextColumn();
        ImGui.Text("中文指令");
        ImGui.NextColumn();
        ImGui.Text("英文指令");
        ImGui.NextColumn();
        ImGui.Separator();

        foreach (var (explanation, chineseCommand, englishCommand) in commandList)
        {
            ImGui.Text(explanation);
            ImGui.NextColumn();

            if (ImGui.Button($"复制##{chineseCommand}"))
            {
                ImGui.SetClipboardText(chineseCommand);
            }
            ImGui.SameLine();
            ImGui.Text(chineseCommand);
            ImGui.NextColumn();

            if (ImGui.Button($"复制##{englishCommand}"))
            {
                ImGui.SetClipboardText(englishCommand);
            }
            ImGui.SameLine();
            ImGui.Text(englishCommand);
            ImGui.NextColumn();
        }

        ImGui.Columns(1);
        ImGui.Separator();

        if (ImGui.Button("关闭"))
        {
            DancerSettings.Instance.IsOpenCommandWindow = false;
            DancerSettings.Instance.Save();
        }

        ImGui.End();
    }
}
