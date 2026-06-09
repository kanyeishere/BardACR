using AEAssist.CombatRoutine.Trigger;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Triggers;

public class BardTriggerActionTripleWeaveForDM : ITriggerAction
{
    public string DisplayName { get; } = "Bard/DM三插";
    public string Remark { get; set; } = "";

    public bool EnableTripleWeaveForDM;

    public BardTriggerActionTripleWeaveForDM()
    {
        EnableTripleWeaveForDM = BardSettings.Instance.EnableTripleWeaveForDM;
    }

    public bool Draw()
    {
        ImGui.Separator();
        ImGui.Text("DM三插设置：");
        ImGui.TextColored(new Vector4(0.3f, 0.9f, 1.0f, 1f),
            EnableTripleWeaveForDM ? "当前：已开启" : "当前：已关闭");
        ImGui.Checkbox("启用DM三插", ref EnableTripleWeaveForDM);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("控制 EnableTripleWeaveForDM，用于开关旅神歌/战斗之声/光明神三插。需要 FuckAnimationLock 支持三插。");
        ImGui.Separator();
        return true;
    }

    public bool Handle()
    {
        BardSettings.Instance.EnableTripleWeaveForDM = EnableTripleWeaveForDM;
        return true;
    }
}