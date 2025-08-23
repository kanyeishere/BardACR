using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.GUI;
using AEAssist.GUI.Tree;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using ImGuiNET;

#nullable enable
namespace Wotou.Dancer.Triggers;

public class DancerEspritCondition : ITriggerBase, ITriggerCond, ITriggerlineCheck
{
    public string DisplayName { get; } = "Dancer/判断伶俐";
    
    public int OperatorIndex = 0;
    
    private readonly string[] _label = new string[5]
    {
        "<",
        ">",
        "<=",
        ">=",
        "=="
    };

    public int Esprit = 0;

    public string Remark { get; set; }

    public bool Draw()
    {
        ImGui.Text("伶俐量谱满足设定条件时，为True");
        ImGui.Text("数值应为0-100之间");
        ImGuiHelper.LeftCombo("条件", ref this.OperatorIndex, this._label);
        ImGuiHelper.LeftInputInt("数值", ref this.Esprit, 0, 100);

        return false;
    }

    public bool Handle(ITriggerCondParams condParams)
    {
        switch (this.OperatorIndex)
        {
            case 0:
                return Core.Resolve<JobApi_Dancer>().Esprit <  this.Esprit;
            case 1:
                return Core.Resolve<JobApi_Dancer>().Esprit > this.Esprit;
            case 2:
                return Core.Resolve<JobApi_Dancer>().Esprit <= this.Esprit;
            case 3:
                return Core.Resolve<JobApi_Dancer>().Esprit >=  this.Esprit;
            case 4:
                return Core.Resolve<JobApi_Dancer>().Esprit == this.Esprit;
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
        if (this.Esprit >= 0 && this.Esprit <= 100)
            return;
        checkResult.AddError(currNode, "伶俐应在0-100之间");
    }
}