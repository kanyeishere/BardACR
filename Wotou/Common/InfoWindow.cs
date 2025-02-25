using System.Numerics;
using ImGuiNET;
using Wotou.Bard.Setting;
using Wotou.Dancer.Setting;

namespace Wotou.Common;

public class InfoWindow
{
    private static bool isWindowOpen = true;

    public static void Draw()
    {
        if (!InfoWindow.isWindowOpen)
            return;
        if (BardSettings.Instance.IsReadInfoWindow05)
            return;
        if (DancerSettings.Instance.IsReadInfoWindow077)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(400, 400), ImGuiCond.Always);
        ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen);
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "重要提醒");
        ImGui.Separator();
        ImGui.TextWrapped("首次进入光暗未来绝境战（绝伊甸）后，系统将自动记录当前队伍成员信息");
        ImGui.TextWrapped("目前，Lv < 1 的用户，仅允许与已记录的固定队队员组队时使用本ACR");
        ImGui.TextWrapped("所以请确保首次进入副本时，与你的固定队队员一同进本！");
        ImGui.TextWrapped("此纪录一旦保存，将无法更改！请谨慎操作！");
        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
            BardSettings.Instance.IsReadInfoWindow05 = true;
            DancerSettings.Instance.IsReadInfoWindow077 = true;
            BardSettings.Instance.Save();
            DancerSettings.Instance.Save();
        }
        ImGui.End();
    }
}