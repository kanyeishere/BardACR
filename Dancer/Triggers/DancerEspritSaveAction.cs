using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using Dalamud.Bindings.ImGui;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Triggers;

public class DancerEspritSaveAction : ITriggerAction, ITriggerBase
{
    public int SaberDanceEspritThreshold;

    public string DisplayName { get; } = "Dancer/非团辅期剑舞的伶俐阈值";

    public string Remark { get; set; }

    public DancerEspritSaveAction()
    {
        this.SaberDanceEspritThreshold = DancerSettings.Instance.SaberDanceEspritThreshold;
    }

    public bool Draw()
    {
        ImGui.Text("这个数值修改之后，战斗重新开始时不会自动重置");
        ImGui.Text("这个数值设置在以下两种情况下无法改变剑舞使用的逻辑：\n- 在小舞前你的伶俐量谱已经满80了，为了防止溢出，ACR会自动使用剑舞\n- 团辅期的剑舞使用逻辑较为复杂，这个阈值设置不会影响团辅期的剑舞使用");
        ImGuiHelper.LeftInputInt("非团辅期使用剑舞的伶俐阈值", ref this.SaberDanceEspritThreshold, 50, 100);
        return true;
    }

    public bool Handle()
    {
        DancerSettings.Instance.SaberDanceEspritThreshold = this.SaberDanceEspritThreshold;
        return true;
    }
}