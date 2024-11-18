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
        if (BardSettings.Instance.IsReadInfoWindow2)
            return;
        if (DancerSettings.Instance.IsReadInfoWindow2)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f + 100), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(600f, 400f), ImGuiCond.Always);
        ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "特别说明");
        ImGui.Separator();
        ImGui.TextWrapped("本次更新修复了因队友抢开导致的起手问题");
        ImGui.TextWrapped("因此之前带有起手脚本的M1S-M4S时间轴已不再适用");
        ImGui.TextWrapped("请大家前往云时间轴更新至最新版本 v1118.01，以确保正常使用。\n");
        
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "请在AE悬浮图标->时间轴->取消勾选“时间轴的高优先级技能作为强制使用处理”");
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "否则4层的6分钟爆发与8分钟爆发会三插卡GCD！");


        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
            BardSettings.Instance.IsReadInfoWindow2 = true;
            DancerSettings.Instance.IsReadInfoWindow2 = true;
            BardSettings.Instance.Save();
            DancerSettings.Instance.Save();
        }
        ImGui.End();
    }
}