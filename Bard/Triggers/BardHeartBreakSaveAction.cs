using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using Dalamud.Bindings.ImGui;
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
        ImGui.Text("注意，此数值修改会在以下三种情况下无效：" +
                   "\n - 当你打开攒碎心箭QT，在团辅技能CD小于28秒后，将不再用碎心箭" +
                   "\n - 当碎心箭层数满了之后，为了防止溢出，会自动使用碎心箭" +
                   "\n - 当你进入团辅爆发期，碎心箭会自动使用至0层");
        ImGuiHelper.LeftInputFloat("碎心箭保留层数", ref this.HeartBreakSaveStack, 0f, 3f);
        return true;
    }

    public bool Handle()
    {
        BardSettings.Instance.HeartBreakSaveStack = this.HeartBreakSaveStack;
        return true;
    }
}