using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.GUI;
using AEAssist.GUI.Tree;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Bindings.ImGui;

#nullable enable
namespace Wotou.Bard.Triggers;

public class BardSoulVoiceCondition : ITriggerBase, ITriggerCond, ITriggerlineCheck
{
    public string DisplayName { get; } = "Bard/判断灵魂之声";
    
    public int OperatorIndex = 0;
    
    private readonly string[] _label = new string[5]
    {
        "<",
        ">",
        "<=",
        ">=",
        "=="
    };

    public int SoulVoice = 0;

    public string Remark { get; set; }

    public bool Draw()
    {
        ImGui.Text("灵魂之声量谱满足设定条件时，为True");
        ImGui.Text("数值应为0-100之间");
        ImGuiHelper.LeftCombo("条件", ref this.OperatorIndex, this._label);
        ImGuiHelper.LeftInputInt("数值", ref this.SoulVoice, 0, 100);

        return false;
    }

    public bool Handle(ITriggerCondParams condParams)
    {
        switch (this.OperatorIndex)
        {
            case 0:
                return Core.Resolve<JobApi_Bard>().SoulVoice <  this.SoulVoice;
            case 1:
                return Core.Resolve<JobApi_Bard>().SoulVoice > this.SoulVoice;
            case 2:
                return Core.Resolve<JobApi_Bard>().SoulVoice <= this.SoulVoice;
            case 3:
                return Core.Resolve<JobApi_Bard>().SoulVoice >=  this.SoulVoice;
            case 4:
                return Core.Resolve<JobApi_Bard>().SoulVoice == this.SoulVoice;
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
        if (this.SoulVoice >= 0 && this.SoulVoice <= 100)
            return;
        checkResult.AddError(currNode, "灵魂之声应在0-100之间");
    }
}