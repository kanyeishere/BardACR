
using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.GUI;
using AEAssist.GUI.Tree;
using AEAssist.JobApi;
using Dalamud.Bindings.ImGui;

#nullable enable
namespace Wotou.Bard.Triggers;

public class BardRepertoireCondition : ITriggerBase, ITriggerCond, ITriggerlineCheck
{
    public string DisplayName { get; } = "Bard/判断诗心层数";
    
    public int OperatorIndex = 0;
    
    private readonly string[] _label = new string[5]
    {
        "<",
        ">",
        "<=",
        ">=",
        "=="
    };
    public int Repertoire; 

    public string Remark { get; set; }

    public bool Draw()
    {
        ImGui.Text("诗心层数满足设定条件时，为True");
        ImGui.Text("数值应为0-4之间");
        ImGuiHelper.LeftCombo("条件", ref this.OperatorIndex, this._label);
        ImGuiHelper.LeftInputInt("层数", ref Repertoire, 0, 4);
        return false;
    }

    public bool Handle(ITriggerCondParams condParams)
    {
        switch (this.OperatorIndex)
        {
            case 0:
                return Core.Resolve<JobApi_Bard>().Repertoire < (long) this.Repertoire;
            case 1:
                return Core.Resolve<JobApi_Bard>().Repertoire > (long) this.Repertoire;
            case 2:
                return Core.Resolve<JobApi_Bard>().Repertoire <= (long) this.Repertoire;
            case 3:
                return Core.Resolve<JobApi_Bard>().Repertoire >= (long) this.Repertoire;
            case 4:
                return Core.Resolve<JobApi_Bard>().Repertoire == (long) this.Repertoire;
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
        if (this.Repertoire >= 0 && this.Repertoire <= 4)
            return;
        checkResult.AddError(currNode, "数值应为0-4之间");
    }
}
