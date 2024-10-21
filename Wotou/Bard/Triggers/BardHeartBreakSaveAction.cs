using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using ImGuiNET;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Triggers;

public class BardHeartBreakSaveAction : ITriggerAction, ITriggerBase
{
    public float HeartBreakSaveStack;

    public string DisplayName { get; } = "Bard/碎心箭保留层数";

    public string Remark { get; set; }

    public BardHeartBreakSaveAction()
    {
        this.HeartBreakSaveStack = BardSettings.Instance.HeartBreakSaveStack;
    }

    public bool Draw()
    {
        ImGui.Text("这个数值修改之后，战斗重新开始时会自动重置为0");
        ImGuiHelper.LeftInputFloat("碎心箭保留层数", ref this.HeartBreakSaveStack, 0f, 3f);
        return true;
    }

    public bool Handle()
    {
        BardSettings.Instance.HeartBreakSaveStack = this.HeartBreakSaveStack;
        return true;
    }
}