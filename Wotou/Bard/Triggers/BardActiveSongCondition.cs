
using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.GUI;
using AEAssist.GUI.Tree;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using ImGuiNET;

#nullable enable
namespace Wotou.Bard.Trigger
{
    public class BardActiveSongCondition : ITriggerBase, ITriggerCond, ITriggerlineCheck
    {
        private int _songType = 0;
        private readonly string[] label = new string[4]
        {
            "无歌曲",
            "旅神歌",
            "贤者歌",
            "军神歌"
        };

        public string DisplayName { get; } = "Bard/判断歌曲类型";

        public string Remark { get; set; }

        public bool Draw()
        {
            ImGui.Text("正在唱的歌曲与所选歌曲相同时，为Ture");
            ImGuiHelper.LeftCombo("歌", ref this._songType, this.label);
            return false;
        }

        public bool Handle(ITriggerCondParams condParams)
        {
            return Core.Resolve<JobApi_Bard>().ActiveSong == BardActiveSongCondition.GetSong(this._songType);
        }

        private static Song GetSong(int index)
        {
            switch (index)
            {
                case 0:
                    return Song.NONE;
                case 1:
                    return Song.WANDERER;
                case 2:
                    return Song.MAGE;
                case 3:
                    return Song.ARMY;
                default:
                    return Song.NONE;
            }
        }

        public void Check(
            TreeCompBase parent,
            TreeNodeBase currNode,
            TriggerLine triggerLine,
            Env env,
            TriggerlineCheckResult checkResult)
        {
            if (this._songType is >= 0 and <= 3)
                return;
            checkResult.AddError(currNode, "选择的歌有误");
        }
    }
}
