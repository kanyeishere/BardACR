using System.Numerics;
using ImGuiNET;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Utility;

public class DancerCommandWindow
{
    // 完整的指令列表
    private static readonly (string Explanation, string ChineseCommand, string EnglishCommand)[] commandList =
    {
        ("防击退技能", "/Wotou_DNC 防击退_hk", "/Wotou_DNC armslength_hk"),
        ("恢复技能", "/Wotou_DNC 内丹_hk", "/Wotou_DNC secondwind_hk"),
        ("团队减伤", "/Wotou_DNC 桑巴_hk", "/Wotou_DNC shieldsamba_hk"),
        ("治疗技能", "/Wotou_DNC 华尔兹_hk", "/Wotou_DNC curingwaltz_hk"),
        ("即兴切换", "/Wotou_DNC 秒开关即兴_hk", "/Wotou_DNC improvisation_hk"),
        ("快速移动", "/Wotou_DNC 疾跑_hk", "/Wotou_DNC run_hk"),
        ("前冲步技能", "/Wotou_DNC 前冲步_hk", "/Wotou_DNC enavant_hk"),
        ("使用爆发药", "/Wotou_DNC 爆发药_hk", "/Wotou_DNC potion_hk"),
        ("释放极限技", "/Wotou_DNC 极限技_hk", "/Wotou_DNC limitbreak_hk"),
        ("停止自动移动", "/Wotou_DNC 停止自动移动_hk", "/Wotou_DNC stopmove_hk"),

        ("开启AOE技能", "/Wotou_DNC AOE_qt", "/Wotou_DNC aoe_qt"),
        ("使用技巧舞步", "/Wotou_DNC 大舞_qt", "/Wotou_DNC technicalstep_qt"),
        ("使用标准舞步", "/Wotou_DNC 小舞_qt", "/Wotou_DNC standardstep_qt"),
        ("释放百花争艳", "/Wotou_DNC 百花_qt", "/Wotou_DNC flourish_qt"),
        ("释放剑舞技能", "/Wotou_DNC 剑舞_qt", "/Wotou_DNC saberdance_qt"),
        ("使用扇舞技能", "/Wotou_DNC 扇舞_qt", "/Wotou_DNC fandance_qt"),
        ("自动治疗技能", "/Wotou_DNC 自动华尔兹_qt", "/Wotou_DNC autocuringwaltz_qt"),
        ("倾泻资源技能", "/Wotou_DNC 倾泻资源_qt", "/Wotou_DNC finalburst_qt")
    };

    public static void Draw()
    {
        if (!DancerSettings.Instance.IsOpenCommandWindow)
            return;

        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        //ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(mainViewport.Size.X / 2f, mainViewport.Size.Y / 1.5f), ImGuiCond.Always);
        //ImGui.SetNextWindowFocus();

        ImGui.Begin("Dancer Command Help", ref DancerSettings.Instance.IsOpenCommandWindow, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize);

        ImGui.TextWrapped("本次更新提供了完整的指令支持，您可以通过/Wotou_DNC 使用快捷指令。\n这些指令可以结合游戏内宏功能使用，非常适合手柄用户快速操作。");
        ImGui.Separator();

        ImGui.Columns(3, "CommandColumns", true);
        ImGui.SetColumnWidth(0, mainViewport.Size.X / 10f);
        ImGui.SetColumnWidth(1, mainViewport.Size.X / 5f);
        ImGui.SetColumnWidth(2, mainViewport.Size.X / 5f);

        ImGui.Text("说明");
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
