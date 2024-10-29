using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using ImGuiNET;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Triggers;

public class DancerFeatherSaveAction : ITriggerAction, ITriggerBase
{
    public int FanDanceSaveStack;

    public string DisplayName { get; } = "Dancer/扇舞保留层数";

    public string Remark { get; set; }

    public DancerFeatherSaveAction()
    {
        this.FanDanceSaveStack = DancerSettings.Instance.FanDanceSaveStack;
    }

    public bool Draw()
    {
        ImGui.Text("这个数值修改之后，战斗重新开始时会自动重置为3");
        ImGuiHelper.LeftInputInt("扇舞保留层数", ref this.FanDanceSaveStack, 0, 4);
        return true;
    }

    public bool Handle()
    {
        DancerSettings.Instance.FanDanceSaveStack = this.FanDanceSaveStack;
        return true;
    }
}