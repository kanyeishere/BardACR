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
        if (BardSettings.Instance.IsReadInfoWindow047 && 
            DancerSettings.Instance.IsReadInfoWindow047)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f));
        ImGui.SetNextWindowSize(new Vector2(600, 500));
        //ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen);
        ImGui.Begin("更新日志", ref isWindowOpen, ImGuiWindowFlags.NoCollapse);

        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "更新日志");
        ImGui.Separator();
        ImGui.BulletText("从此版本起，移除了本ACR在野队时的使用限制");

        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
            BardSettings.Instance.IsReadInfoWindow047 = true;
            DancerSettings.Instance.IsReadInfoWindow047 = true;
            BardSettings.Instance.Save();
            DancerSettings.Instance.Save();
        }
        ImGui.End();
    }
}