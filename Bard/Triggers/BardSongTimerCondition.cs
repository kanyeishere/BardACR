

using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.GUI;
using AEAssist.GUI.Tree;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using ImGuiNET;

#nullable enable
namespace Wotou.Bard.Triggers;

public class BardSongTimerCondition : ITriggerBase, ITriggerCond, ITriggerlineCheck
{
    public string DisplayName { get; } = "Bard/判断歌曲剩余时间";
    
    public int OperatorIndex = 0;
    
    private readonly string[] _label = new string[5]
    {
        "<",
        ">",
        "<=",
        ">=",
        "=="
    };

    public int Time = 0;

    public string Remark { get; set; }

    public bool Draw()
    {
        ImGui.Text("歌曲剩余时间满足设定条件时，为True");
        ImGui.Text("数值应为0-45000之间");
        ImGui.Text("单位为毫秒");
        ImGuiHelper.LeftCombo("条件", ref this.OperatorIndex, this._label);
        ImGuiHelper.LeftInputInt("时长", ref this.Time, 0, 45000);
        return false;
    }

    public bool Handle(ITriggerCondParams condParams)
    {
        switch (this.OperatorIndex)
        {
            case 0:
                return Core.Resolve<JobApi_Bard>().SongTimer < (long) this.Time;
            case 1:
                return Core.Resolve<JobApi_Bard>().SongTimer > (long) this.Time;
            case 2:
                return Core.Resolve<JobApi_Bard>().SongTimer <= (long) this.Time;
            case 3:
                return Core.Resolve<JobApi_Bard>().SongTimer >= (long) this.Time;
            case 4:
                return Core.Resolve<JobApi_Bard>().SongTimer == (long) this.Time;
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
        if (this.Time >= 0 && this.Time <= 45000)
            return;
        checkResult.AddError(currNode, "歌曲时长应在在0-45000之间");
    }
}
