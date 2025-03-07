using System.Numerics;
using ImGuiNET;
using Wotou.Bard.Setting;
using Wotou.Dancer.Setting;

namespace Wotou.Bard.Utility;

public class BardCommandWindow
{
    // 完整的指令列表
    private static readonly (string CommandType, string ChineseCommand, string EnglishCommand)[] commandList =
    {
        ("Hotkey", "/Wotou_BRD 防击退_hk", "/Wotou_BRD armslength_hk"),
        ("Hotkey", "/Wotou_BRD 续毒_hk", "/Wotou_BRD ironjaws_hk"),
        ("Hotkey", "/Wotou_BRD 内丹_hk", "/Wotou_BRD secondwind_hk"),
        ("Hotkey", "/Wotou_BRD 行吟_hk", "/Wotou_BRD troubadour_hk"),
        ("Hotkey", "/Wotou_BRD 大地神_hk", "/Wotou_BRD naturesminne_hk"),
        ("Hotkey", "/Wotou_BRD 疾跑_hk", "/Wotou_BRD run_hk"),
        ("Hotkey", "/Wotou_BRD 后跳_hk", "/Wotou_BRD repellingshot_hk"),
        ("Hotkey", "/Wotou_BRD 爆发药_hk", "/Wotou_BRD potion_hk"),
        ("Hotkey", "/Wotou_BRD 极限技_hk", "/Wotou_BRD limitbreak_hk"),
        ("Hotkey", "/Wotou_BRD 停止自动移动_hk", "/Wotou_BRD stopmove_hk"),
        ("Hotkey", "/Wotou_BRD 绝峰箭_hk", "/Wotou_BRD apexarrow_hk"),

        ("QT", "/Wotou_BRD AOE_qt", "/Wotou_BRD aoe_qt"),
        ("QT", "/Wotou_BRD 爆发_qt", "/Wotou_BRD burst_qt"),
        ("QT", "/Wotou_BRD 绝峰箭_qt", "/Wotou_BRD apex_qt"),
        ("QT", "/Wotou_BRD DOT_qt", "/Wotou_BRD dot_qt"),
        ("QT", "/Wotou_BRD 唱歌_qt", "/Wotou_BRD song_qt"),
        ("QT", "/Wotou_BRD 攒碎心箭_qt", "/Wotou_BRD heartbreak_qt"),
        ("QT", "/Wotou_BRD 爆发药_qt", "/Wotou_BRD usepotion_qt"),
        ("QT", "/Wotou_BRD 大地神_qt", "/Wotou_BRD natureminne_qt"),
        ("QT", "/Wotou_BRD 强对齐_qt", "/Wotou_BRD strongalign_qt"),
        ("QT", "/Wotou_BRD 九天连箭_qt", "/Wotou_BRD empyrrealarrow_qt"),
        ("QT", "/Wotou_BRD 侧风诱导箭_qt", "/Wotou_BRD sidewinder_qt"),
        ("QT", "/Wotou_BRD 九天前置_qt", "/Wotou_BRD empyrrealarrowbeforegcd_qt"),
        ("QT", "/Wotou_BRD 对齐旅神_qt", "/Wotou_BRD burstwithwanderer_qt"),
        ("QT", "/Wotou_BRD 清空鹰眼_qt", "/Wotou_BRD clearhawkeyesbuffbeforedots_qt")
    };


    public static void Draw()
    {
        if (!BardSettings.Instance.IsOpenCommandWindow)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        //ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(mainViewport.Size.X / 2f, mainViewport.Size.Y / 1.5f), ImGuiCond.Always);
        //ImGui.SetNextWindowFocus();

        ImGui.Begin("Bard Command Help", ref BardSettings.Instance.IsOpenCommandWindow);
        ImGui.TextWrapped("本次更新提供了完整的指令支持，您可以通过/Wotou_BRD 使用快捷指令。" +
                          "\n这些指令可以结合游戏内宏功能使用，非常适合手柄用户快速操作。");
        ImGui.Separator();
        //ImGui.Text("\u6307\u4ee4\u5e2e\u52a9\u7a97\u53e3\uff1a"); // 显示标题
        //ImGui.Separator();

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
            BardSettings.Instance.IsOpenCommandWindow = false;
            BardSettings.Instance.Save();
        }

        ImGui.End();
    }
}
