using System.Numerics;
using Dalamud.Bindings.ImGui;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Utility;

public class BardCommandWindow
{
    private static (string CommandType, string ChineseCommand, string EnglishCommand)[]? commandList;

    private static void BuildCommandList()
    {
        var rows = new List<(string, string, string)>();

        // ---- Hotkey 行（/Wotou_BRD <alias>_hk）----
        foreach (var hk in BardQtHotkeyRegistry.Hotkeys)
        {
            var zh = $"/Wotou_BRD {hk.LabelZh}_hk";                 // 中文：显示名
            var en = $"/Wotou_BRD {hk.Id.ToLowerInvariant()}_hk";   // 英文：id 小写
            rows.Add(("Hotkey", zh, en));
        }

        // ---- QT 行（/Wotou_BRD <alias>_qt）----
        foreach (var q in BardQtHotkeyRegistry.Qts)
        {
            var zh = $"/Wotou_BRD {q.Key}_qt";                      // 中文：QTKey 中文值
            var en = $"/Wotou_BRD {q.EnglishAlias}_qt";             // 英文：变量名小写
            rows.Add(("QT", zh, en));
        }

        // 去重 & 排序（防止未来重复项）
        commandList = rows
            .GroupBy(r => (r.Item1, r.Item2, r.Item3))
            .Select(g => g.Key)
            .OrderBy(r => r.Item1)
            .ThenBy(r => r.Item3, System.StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool HasChinese(string s) => s.Any(ch => ch > 127);

    public static void Draw()
    {
        if (!BardSettings.Instance.IsOpenCommandWindow)
            return;

        if (commandList is null) // 只在首次打开时构建；如需每次刷新，改回直接调用 BuildCommandList();
            BuildCommandList();

        var mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowSize(new Vector2(mainViewport.Size.X / 2f, mainViewport.Size.Y / 1.5f), ImGuiCond.Always);

        ImGui.Begin("Bard Command Help", ref BardSettings.Instance.IsOpenCommandWindow);
        ImGui.TextWrapped("本 ACR 提供 /Wotou_BRD 指令：可与游戏宏搭配使用，适合手柄快速操作。");
        ImGui.Separator();

        ImGui.Columns(3, "CommandColumns", true);
        ImGui.SetColumnWidth(0, mainViewport.Size.X / 10f);
        ImGui.SetColumnWidth(1, mainViewport.Size.X / 5f);
        ImGui.SetColumnWidth(2, mainViewport.Size.X / 5f);

        ImGui.Text("指令类型"); ImGui.NextColumn();
        ImGui.Text("中文指令"); ImGui.NextColumn();
        ImGui.Text("英文指令"); ImGui.NextColumn();
        ImGui.Separator();

        foreach (var (commandType, chineseCommand, englishCommand) in commandList!)
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

        if (ImGui.Button("刷新"))
        {
            BuildCommandList(); // 如果动态增删，点刷新更新
        }
        ImGui.SameLine();
        if (ImGui.Button("关闭"))
        {
            BardSettings.Instance.IsOpenCommandWindow = false;
            BardSettings.Instance.Save();
        }

        ImGui.End();
    }
}
