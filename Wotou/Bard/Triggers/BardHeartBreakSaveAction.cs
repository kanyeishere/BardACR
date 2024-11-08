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
        ImGui.Text("注意，此数值修改会在团辅爆发期也生效" +
                   "\n但是会在以下两种情况下无效：" +
                   "\n - 当你打开攒碎心箭QT后，会自动在团辅技能CD还剩28秒以后，不再用碎心箭，此处修改会在这段时间内失效" +
                   "\n - 当碎心箭层数满了之后，为了防止溢出，会自动使用碎心箭");
        ImGuiHelper.LeftInputFloat("碎心箭保留层数", ref this.HeartBreakSaveStack, 0f, 3f);
        return true;
    }

    public bool Handle()
    {
        BardSettings.Instance.HeartBreakSaveStack = this.HeartBreakSaveStack;
        return true;
    }
}