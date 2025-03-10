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
        if (BardSettings.Instance.IsReadInfoWindow080)
            return;
        if (DancerSettings.Instance.IsReadInfoWindow080)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(600, 400), ImGuiCond.Always);
        ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen);
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "重要提醒");
        ImGui.Separator();
        ImGui.TextWrapped("首次进入光暗未来绝境战（绝伊甸）后，ACR将自动记录当前队伍成员信息");
        ImGui.TextWrapped("绝伊甸开放后首月内，Lv < 1 的用户，仅允许与已记录的固定队队员组队时使用本ACR");
        ImGui.TextWrapped("所以请确保首次进入副本时，与你的固定队队员一同进本！");
        ImGui.TextWrapped("");
        ImGui.TextWrapped("更重要的是，如果你在国际服和国服都有角色，系统会记录你第一次进本时的队友名单。");
        ImGui.TextWrapped("这意味着，如果你在国际服先进入了副本，那么你的国服账号将无法使用 ACR，请谨慎选择！");
        ImGui.TextWrapped("此纪录一旦保存，将无法更改！请谨慎操作！");
        ImGui.TextWrapped("");
        ImGui.TextWrapped("在8人队伍中，至少有2名玩家（包括你自己）与系统记录的第一次进本的8人ID+服务器匹配，即视为固定队。");
        ImGui.TextWrapped("如果匹配人数不足，ACR将判断你处于野队，并限制使用。");
        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
            BardSettings.Instance.IsReadInfoWindow080 = true;
            DancerSettings.Instance.IsReadInfoWindow080 = true;
            BardSettings.Instance.Save();
            DancerSettings.Instance.Save();
        }
        ImGui.End();
    }
}