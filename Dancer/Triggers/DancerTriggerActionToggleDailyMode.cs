using AEAssist.CombatRoutine.Trigger;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using Wotou.Dancer.Setting; // 请根据实际命名空间调整

namespace Wotou.Dancer.Triggers;

public class DancerTriggerActionToggleDailyMode : ITriggerAction
{
    public string DisplayName { get; } = "Dancer/切换模式（日随高难）";
    public string Remark { get; set; }

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

        if (ImGui.Button("切换模式"))
        {
            IsDailyMode = !IsDailyMode;
        }

        ImGui.Separator();
        return true;
    }

    public bool Handle()
    {
        DancerSettings.Instance.IsDailyMode = IsDailyMode;
        return true;
    }
}