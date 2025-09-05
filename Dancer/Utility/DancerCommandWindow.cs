using System.Numerics;
using ImGuiNET;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Utility
{
    public class DancerCommandWindow
    {
        private static (string CommandType, string ChineseCommand, string EnglishCommand)[]? commandList;

        private static void BuildCommandList()
        {
            var rows = new List<(string, string, string)>();

            // ---- Hotkey 行（/Wotou_DNC <alias>_hk）----
            foreach (var hk in DancerQtHotkeyRegistry.Hotkeys)
            {
                var zh = $"/Wotou_DNC {hk.ChineseAlias}_hk";          // 中文：显示名（LabelZh）
                var en = $"/Wotou_DNC {hk.EnglishAlias}_hk";         // 英文：id/别名小写
                rows.Add(("Hotkey", zh, en));
            }

            // ---- QT 行（/Wotou_DNC <alias>_qt）----
            foreach (var q in DancerQtHotkeyRegistry.Qts)
            {
                var zh = $"/Wotou_DNC {q.ChineseAlias}_qt";           // 中文：QTKey 中文值
                var en = $"/Wotou_DNC {q.EnglishAlias}_qt";           // 英文：变量名小写
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

        public static void Draw()
        {
            if (!DancerSettings.Instance.IsOpenCommandWindow)
                return;
            
            if (commandList is null)
                BuildCommandList();

            var mainViewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowSize(new Vector2(mainViewport.Size.X / 2f, mainViewport.Size.Y / 1.5f), ImGuiCond.Always);

            ImGui.Begin("Dancer Command Help", ref DancerSettings.Instance.IsOpenCommandWindow, ImGuiWindowFlags.None);

            ImGui.TextWrapped("本 ACR 提供 /Wotou_DNC 指令：可与游戏宏搭配使用，适合手柄快速操作。");
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
                BuildCommandList();
            ImGui.SameLine();
            if (ImGui.Button("关闭"))
            {
                DancerSettings.Instance.IsOpenCommandWindow = false;
                DancerSettings.Instance.Save();
            }

            ImGui.End();
        }
    }
}
