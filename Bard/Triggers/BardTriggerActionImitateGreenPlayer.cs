using AEAssist.CombatRoutine.Trigger;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Triggers;

public class BardTriggerActionImitateGreenPlayer : ITriggerAction
{
    public string DisplayName { get; } = "Bard/模仿绿玩手打循环";
    public string Remark { get; set; } = "";

    public bool ImitateGreenPlayer;

    public BardTriggerActionImitateGreenPlayer()
    {
        ImitateGreenPlayer = BardSettings.Instance.ImitateGreenPlayer;
    }

    public bool Draw()
    {
        ImGui.Separator();
        ImGui.Text("模仿绿玩手打循环设置：");
        ImGui.TextColored(new Vector4(0.3f, 0.9f, 1.0f, 1f),
            ImitateGreenPlayer ? "当前：已开启" : "当前：已关闭");
        ImGui.Checkbox("模仿绿玩手打循环（实验性功能）", ref ImitateGreenPlayer);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("在旅神歌与猛者强击前插入伤腿与伤足");
        ImGui.Separator();
        return true;
    }

    public bool Handle()
    {
        BardSettings.Instance.ImitateGreenPlayer = ImitateGreenPlayer;
        return true;
    }
}