

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
    public string DisplayName { get; } = "Bard/检测歌曲剩余时间";
    
    private int _operatorIndex = 0;
    
    private readonly string[] _label = new string[5]
    {
        "<",
        ">",
        "<=",
        ">=",
        "=="
    };

    private int Time { get; set; } = 0;

    public string Remark { get; set; }

    public bool Draw()
    {
        ImGui.Text("歌曲剩余时间小于等于设定值时 通过");
        ImGui.Text("数值应为0-45000之间");
        ImGui.Text("单位为毫秒");
        ImGuiHelper.LeftCombo("歌", ref this._operatorIndex, this._label);
        return false;
    }

    public bool Handle(ITriggerCondParams condParams)
    {
        switch (this._operatorIndex)
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
        checkResult.AddError(currNode, "时长设置未在0-45000之间");
    }
}
