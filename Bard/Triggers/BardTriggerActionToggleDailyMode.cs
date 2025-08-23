using AEAssist.CombatRoutine.Trigger;
using ImGuiNET;
using System.Numerics;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Triggers;

public class BardTriggerActionToggleDailyMode : ITriggerAction
{
    public string DisplayName { get; } = "Bard/切换模式（日随高难）";
    public string Remark { get; set; }

    // 内部状态记录当前模式（从设置中同步）
    public bool IsDailyMode = false;

    public bool Draw()
    {
        ImGui.NewLine();
        ImGui.Separator();
        ImGui.Text("选择当前战斗模式：");
        ImGui.NewLine();
        
        Vector4 color = IsDailyMode
            ? new Vector4(0.9f, 0.75f, 0.2f, 1.0f) // 黄色 - 日随
            : new Vector4(0.2f, 0.6f, 1.0f, 1.0f); // 蓝色 - 高难

        ImGui.TextColored(color, IsDailyMode ? "当前：日常模式" : "当前：高难模式");        
        ImGui.NewLine();

        // 切换按钮
        if (ImGui.Button("切换模式"))
        {
            IsDailyMode = !IsDailyMode;
        }

        ImGui.Separator();
        return true;
    }

    public bool Handle()
    {
        // 应用切换结果
        BardSettings.Instance.IsDailyMode = IsDailyMode;
        return true;
    }
}