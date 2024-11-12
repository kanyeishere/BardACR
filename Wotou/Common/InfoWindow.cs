using System.Numerics;
using ImGuiNET;

namespace Wotou.Common;

public class InfoWindow
{
    private static bool isWindowOpen = true;

    public static void Draw()
    {
        if (!InfoWindow.isWindowOpen)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f + 100), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(600f, 400f), ImGuiCond.Always);
        ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "特别说明");
        ImGui.Separator();
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f),"经过高强度的零式开荒测试，确认在当前版本下几乎不存在玩家死亡后技能卡住（“电”）的情况\n请大家放心使用");
        ImGui.TextWrapped("但万一您遇到这种情况，请尝试以下方法恢复循环：");
        ImGui.TextWrapped("1. 手动释放一次技能，例如爆发射击或瀑泻");
        ImGui.TextWrapped("2. 切换到日随模式，通常能帮助恢复正常循环");
        ImGui.TextWrapped("3. 若上述方法无效，请提前下载并切换到其他 ACR 进行使用");

        ImGui.TextWrapped("如有任何问题，欢迎随时反馈\n");
        ImGui.TextWrapped("此外，M1S-M4S的诗人与舞者时间轴已完成并上传至云时间轴。请前往时间轴-时间轴编辑器-云时间轴下载使用（记得取消勾选 “只查询已验证轴” 选项）");
        ImGui.TextWrapped("感谢您的理解与支持，希望这些提示能帮助您顺利开荒零式！");
        

        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
        }
        ImGui.End();
    }
}