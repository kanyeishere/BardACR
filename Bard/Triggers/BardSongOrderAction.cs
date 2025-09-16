using System.Numerics;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using AEAssist.Helper;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Bindings.ImGui;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.Triggers;

public class BardSongOrderAction : ITriggerAction, ITriggerBase
{
    public int FirstSong;
    public int SecondSong;
    public int ThirdSong;

    public string DisplayName { get; } = "Bard/歌曲顺序";

    public string Remark { get; set; }

    private readonly string[] label = new string[4]
    {
        "请选择",
        "旅神",
        "贤者",
        "军神"
    };

    public bool Draw()
    {
        ImGui.Text("这个数值修改之后不会自动重置");
        ImGui.Text("如果要改的话建议战斗开始时加一个重置回默认的触发器");
        
        if (FirstSong == 0 || SecondSong == 0 || ThirdSong == 0)
            ImGui.TextColored(new Vector4(1, 0.7f, 0, 1), "请选择歌曲顺序");
        else if (FirstSong == SecondSong || FirstSong == ThirdSong || SecondSong == ThirdSong)
            ImGui.TextColored(new Vector4(1, 0.7f, 0, 1), "歌曲不能重复");
        else
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "歌曲顺序已选择");
        // 特殊歌曲顺序提示
        if (FirstSong != 1 || SecondSong != 2 || ThirdSong != 3)
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "特殊歌曲顺序，将禁用强对齐和对齐旅神");
        
        ImGuiHelper.LeftCombo("第一首歌", ref this.FirstSong, this.label);
        ImGuiHelper.LeftCombo("第二首歌", ref this.SecondSong, this.label);
        ImGuiHelper.LeftCombo("第三首歌", ref this.ThirdSong, this.label);
        return true;
    }

    public bool Handle()
    {
        if (FirstSong == 0 || SecondSong == 0 || ThirdSong == 0)
            return false;
        if (FirstSong == SecondSong || FirstSong == ThirdSong || SecondSong == ThirdSong)
            return false;

        BardSettings.Instance.FirstSong = FirstSong switch
        {
            1 => Song.Wanderer,
            2 => Song.Mage,
            3 => Song.Army,
            _ => BardSettings.Instance.FirstSong
        };

        BardSettings.Instance.SecondSong = SecondSong switch
        {
            1 => Song.Wanderer,
            2 => Song.Mage,
            3 => Song.Army,
            _ => BardSettings.Instance.SecondSong
        };

        BardSettings.Instance.ThirdSong = ThirdSong switch
        {
            1 => Song.Wanderer,
            2 => Song.Mage,
            3 => Song.Army,
            _ => BardSettings.Instance.ThirdSong
        };
        BardSongSettingsManager.Instance.InitializeSongSettings();
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
            LogHelper.Print("时间轴设置","特殊歌轴顺序，将禁用强对齐和对齐旅神");
        }
        return true;
    }
}