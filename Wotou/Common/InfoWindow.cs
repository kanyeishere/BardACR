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
        if (BardSettings.Instance.IsReadInfoWindow049)
            return;
        if (DancerSettings.Instance.IsReadInfoWindow049)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f));
        ImGui.SetNextWindowSize(new Vector2(600, 500));
        //ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen);
        ImGui.Begin("0319.05 时间轴更新日志", ref isWindowOpen, ImGuiWindowFlags.NoCollapse);

        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "诗人更新");
        ImGui.Separator();
        ImGui.BulletText("P2 第一次行吟略微延后，以保证钢铁的情况下，能覆盖全员。");
        ImGui.BulletText("P2 开头的光阴神默认开启。");
        ImGui.BulletText("P2.5 光水晶现在会根据玩家在时间轴界面选择的职能，自动切换目标（旧版是根据距离判断）。");
        ImGui.BulletText("P3 一运修复p3中火无损处理:");
        ImGui.BulletText(" - 需要 DR测试码 / 猫盒 / I-Ching 三选一，并开启移速功能，否则仅会自动开疾跑，时间较紧张");
        ImGui.BulletText("P4 取消血量差强制双目标切换，仅保留双目标续毒功能。");
        ImGui.BulletText("P4 如果需要血量差强制切换目标，请进入309节点修改");

        ImGui.Text("");
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "舞者更新");
        ImGui.Separator();
        ImGui.BulletText("P2 第一次桑巴略微延后，以保证钢铁的情况下，能覆盖全员。");
        ImGui.BulletText("P2.5 光水晶现在会根据玩家在时间轴界面选择的职能，自动切换目标（旧版是根据距离判断）。");
        ImGui.BulletText("P3 一运添加中火无损移速修改功能:");
        ImGui.BulletText(" - 需要 DR测试码 / 猫盒 / I-Ching 三选一，并开启移速功能，否则仅会自动开疾跑，时间较紧张");
        ImGui.BulletText("P4 团辅延后 1 秒，保证大舞能打到双目标。");

        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
            BardSettings.Instance.IsReadInfoWindow049 = true;
            DancerSettings.Instance.IsReadInfoWindow049 = true;
            BardSettings.Instance.Save();
            DancerSettings.Instance.Save();
        }
        ImGui.End();
    }
}