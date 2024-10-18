using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using ImGuiNET;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Triggers;

public class BardSongDurationAction : ITriggerAction, ITriggerBase
{
    public float WandererSongDuration;
    public float MageSongDuration;
    public float ArmySongDuration;

    public string DisplayName { get; } = "Bard/歌曲时长";

    public string Remark { get; set; }

    public BardSongDurationAction()
    {
        this.WandererSongDuration = BardSettings.Instance.WandererSongDuration;
        this.MageSongDuration = BardSettings.Instance.MageSongDuration;
        this.ArmySongDuration = BardSettings.Instance.ArmySongDuration;
    }

    public bool Draw()
    {
        ImGui.Text("这个数值修改之后不会自动重置");
        ImGui.Text("如果要改的话建议战斗开始时加一个重置回默认的触发器");
        
        ImGuiHelper.LeftInputFloat("旅神歌时长", ref this.WandererSongDuration, 3f, 45f);
        ImGuiHelper.LeftInputFloat("贤者歌时长", ref this.MageSongDuration, 3f, 45f);
        ImGuiHelper.LeftInputFloat("军神歌时长", ref this.ArmySongDuration, 3f, 45f);
        return true;
    }

    public bool Handle()
    {
        BardSettings.Instance.WandererSongDuration = this.WandererSongDuration;
        BardSettings.Instance.MageSongDuration = this.MageSongDuration;
        BardSettings.Instance.ArmySongDuration = this.ArmySongDuration;
        return true;
    }
}