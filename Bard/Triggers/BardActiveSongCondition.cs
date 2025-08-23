
using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.GUI;
using AEAssist.GUI.Tree;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using ImGuiNET;

#nullable enable
namespace Wotou.Bard.Triggers
{
    public class BardActiveSongCondition : ITriggerBase, ITriggerCond, ITriggerlineCheck
    {
        
        private readonly string[] label = new string[4]
        {
            "无歌曲",
            "旅神歌",
            "贤者歌",
            "军神歌"
        };

        public string DisplayName { get; } = "Bard/判断歌曲类型";

        public string Remark { get; set; }
        
        public int SongType = 0;

        public bool Draw()
        {
            ImGui.Text("正在唱的歌曲与所选歌曲相同时，为Ture");
            ImGuiHelper.LeftCombo("歌", ref this.SongType, this.label);
            return false;
        }

        public bool Handle(ITriggerCondParams condParams)
        {
            if (SongType == 0)
                return Core.Resolve<JobApi_Bard>().ActiveSong == Song.None;
            if (SongType == 1)
                return Core.Resolve<JobApi_Bard>().ActiveSong == Song.Wanderer;
            if (SongType == 2)
                return Core.Resolve<JobApi_Bard>().ActiveSong == Song.Mage;
            if (SongType == 3)
                return Core.Resolve<JobApi_Bard>().ActiveSong == Song.Army;
            return false;
        }
        

        public void Check(
            TreeCompBase parent,
            TreeNodeBase currNode,
            TriggerLine triggerLine,
            Env env,
            TriggerlineCheckResult checkResult)
        {
            if (this.SongType is >= 0 and <= 3)
                return;
            checkResult.AddError(currNode, "选择的歌有误");
        }
    }
}
