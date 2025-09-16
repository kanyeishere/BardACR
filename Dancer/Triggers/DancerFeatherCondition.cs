using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.GUI;
using AEAssist.GUI.Tree;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Bindings.ImGui;

#nullable enable
namespace Wotou.Dancer.Triggers;

public class DancerFeatherCondition : ITriggerBase, ITriggerCond, ITriggerlineCheck
{
    public string DisplayName { get; } = "Dancer/判断扇舞层数";
    
    public int OperatorIndex = 0;
    
    private readonly string[] _label = new string[5]
    {
        "<",
        ">",
        "<=",
        ">=",
        "=="
    };

    public int Feather = 0;

    public string Remark { get; set; }

    public bool Draw()
    {
        ImGui.Text("扇舞量谱满足设定条件时，为True");
        ImGui.Text("数值应为0-4之间");
        ImGuiHelper.LeftCombo("条件", ref this.OperatorIndex, this._label);
        ImGuiHelper.LeftInputInt("数值", ref this.Feather, 0, 4);

        return false;
    }

    public bool Handle(ITriggerCondParams condParams)
    {
        switch (this.OperatorIndex)
        {
            case 0:
                return Core.Resolve<JobApi_Dancer>().FourFoldFeathers <  this.Feather;
            case 1:
                return Core.Resolve<JobApi_Dancer>().FourFoldFeathers > this.Feather;
            case 2:
                return Core.Resolve<JobApi_Dancer>().FourFoldFeathers <= this.Feather;
            case 3:
                return Core.Resolve<JobApi_Dancer>().FourFoldFeathers >=  this.Feather;
            case 4:
                return Core.Resolve<JobApi_Dancer>().FourFoldFeathers == this.Feather;
            default:
                return true;
        }
    }

    public void Check(
        TreeCompBase parent,
        TreeNodeBase currNode,
        TriggerLine triggerLine,
        Env env,
        TriggerlineCheckResult checkResult)
    {
        if (this.Feather >= 0 && this.Feather <= 4)
            return;
        checkResult.AddError(currNode, "扇舞应在0-4之间");
    }
}