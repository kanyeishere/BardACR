using AEAssist.GUI;
using ImGuiNET;

namespace Wotou.Bard.Setting;

public class BardSettingUI
{
    public static BardSettingUI Instance = new();
    public BardSettings BardSettings => BardSettings.Instance;
    
    public void Draw()
    {
        /*ImGuiHelper.LeftInputFloat("旅神歌持续时间", ref BardSettings.WandererSongDuration);
        ImGuiHelper.LeftInputFloat("贤者歌持续时间", ref BardSettings.MageSongDuration);
        ImGuiHelper.LeftInputFloat("军神歌持续时间", ref BardSettings.ArmySongDuration);
        ImGui.Checkbox("是否赌两分钟三次绝峰", ref BardSettings.GambleTripleApex);
        if (ImGui.Button("Save"))
        {
            BardSettings.Instance.Save();
        }*/
    }
}