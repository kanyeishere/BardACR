using System.Numerics;
using ImGuiNET;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Utility;

public class BardCommandWindow
{
    private static (string CommandType, string ChineseCommand, string EnglishCommand)[]? commandList;

    private static void BuildCommandList()
    {
        // ⚠️ 重要：
        // 这里必须先 new 一个 BardRotationEventHandler 并调用 BulidQtKeyDictionary()，
        // 否则 qtKeyDictionary 不会被初始化，所有 /xxx_qt 宏指令将无法识别。
        // 因为 qtKeyDictionary 是根据 QTKey 常量反射生成的，
        // 初始化后才能提供中英别名 → 规范键 的映射。
        var handler = new BardRotationEventHandler(); 
        handler.BulidQtKeyDictionary();

        var rows = new List<(string, string, string)>();

        // Hotkey 行
        foreach (var g in handler.hotkeyDictionary
                     .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                     .GroupBy(kv => kv.Value!, StringComparer.OrdinalIgnoreCase))
        {
            var zh = g.Select(kv => kv.Key).FirstOrDefault(HasChinese) ?? g.First().Key;
            var en = g.Select(kv => kv.Key).FirstOrDefault(k => !HasChinese(k)) ?? g.First().Key;
            rows.Add(("Hotkey", $"/Wotou_BRD {zh}_hk", $"/Wotou_BRD {en}_hk"));
        }

        // QT 行
        foreach (var g in handler.qtKeyDictionary
                     .GroupBy(kv => kv.Value, StringComparer.OrdinalIgnoreCase))
        {
            var zh = g.Select(kv => kv.Key).FirstOrDefault(HasChinese) ?? g.First().Key;
            var en = g.Select(kv => kv.Key).FirstOrDefault(k => !HasChinese(k)) ?? g.First().Key;
            rows.Add(("QT", $"/Wotou_BRD {zh}_qt", $"/Wotou_BRD {en}_qt"));
        }
        

        commandList = rows
            .OrderBy(r => r.Item1, StringComparer.OrdinalIgnoreCase)
            .ThenBy(r => r.Item3, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }


    private static bool HasChinese(string s) => s.Any(ch => ch > 127);

    public static void Draw()
    {
        if (!BardSettings.Instance.IsOpenCommandWindow)
            return;
        BuildCommandList();
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowSize(new Vector2(mainViewport.Size.X / 2f, mainViewport.Size.Y / 1.5f), ImGuiCond.Always);

        ImGui.Begin("Bard Command Help", ref BardSettings.Instance.IsOpenCommandWindow);
        ImGui.TextWrapped("本ACR提供了完整的指令支持，可以通过/Wotou_BRD 使用快捷指令。" +
                          "\n这些指令可以结合游戏内宏功能使用，非常适合手柄用户快速操作。");
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

        foreach (var (commandType, chineseCommand, englishCommand) in commandList)
        {
            ImGui.Text(commandType);
            ImGui.NextColumn();

            if (ImGui.Button($"复制##{chineseCommand}"))
                ImGui.SetClipboardText(chineseCommand);
            ImGui.SameLine();
            ImGui.Text(chineseCommand);
            ImGui.NextColumn();

            if (ImGui.Button($"复制##{englishCommand}"))
                ImGui.SetClipboardText(englishCommand);
            ImGui.SameLine();
            ImGui.Text(englishCommand);
            ImGui.NextColumn();
        }

        ImGui.Columns(1);
        ImGui.Separator();

        if (ImGui.Button("关闭"))
        {
            BardSettings.Instance.IsOpenCommandWindow = false;
            BardSettings.Instance.Save();
        }

        ImGui.End();
    }
}
