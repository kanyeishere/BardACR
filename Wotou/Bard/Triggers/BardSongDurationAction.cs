using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using ImGuiNET;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Triggers;

public class BardSongDurationAction : ITriggerAction, ITriggerBase
{
    private float _wandererSongDuration;
    private float _mageSongDuration;
    private float _armySongDuration;

    public string DisplayName { get; } = "Bard/歌曲时长";

    public string Remark { get; set; }

    public BardSongDurationAction()
    {
        this._wandererSongDuration = BardSettings.Instance.WandererSongDuration;
        this._mageSongDuration = BardSettings.Instance.MageSongDuration;
        this._armySongDuration = BardSettings.Instance.ArmySongDuration;
    }

    public bool Draw()
    {
        ImGui.Text("这个数值修改之后不会自动重置");
        ImGui.Text("如果要改的话建议战斗开始时加一个重置回默认的触发器");
        
        ImGuiHelper.LeftInputFloat("旅神歌时长", ref this._wandererSongDuration, 3f, 45f);
        ImGuiHelper.LeftInputFloat("贤者歌时长", ref this._mageSongDuration, 3f, 45f);
        ImGuiHelper.LeftInputFloat("军神歌时长", ref this._armySongDuration, 3f, 45f);
        return true;
    }

    public bool Handle()
    {
        BardSettings.Instance.WandererSongDuration = this._wandererSongDuration;
        BardSettings.Instance.MageSongDuration = this._mageSongDuration;
        BardSettings.Instance.ArmySongDuration = this._armySongDuration;
        return true;
    }
}