using AEAssist.GUI;
using ImGuiNET;

namespace Wotou.Bard.Setting;

public class BardSettingUI
{
    public static BardSettingUI Instance = new();
    public BardSettings BardSettings => BardSettings.Instance;
    
    public void Draw()
    {
        ImGui.Checkbox("使用速行", ref BardSettings.Instance.UsePeloton);
        ImGuiHelper.LeftInputInt("非爆发期Apex值达到多少时才使用", ref BardSettings.ApexArrowValue);
        
        if (ImGui.Button("Save"))
        {
            BardSettings.Instance.Save();
        }
    }
}