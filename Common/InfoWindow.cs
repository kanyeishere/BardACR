using System.Numerics;
using Dalamud.Bindings.ImGui;
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
        if (BardSettings.Instance.IsReadInfoWindow08 && 
            DancerSettings.Instance.IsReadInfoWindow08)
            return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f));
        ImGui.SetNextWindowSize(new Vector2(800, 600));
        //ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen);
        ImGui.Begin("更新日志", ref isWindowOpen, ImGuiWindowFlags.NoCollapse);

        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "更新日志");
        ImGui.Separator();
        ImGui.TextWrapped("使用诗人 ACR 进入绝伊甸副本时，推荐搭配可达鸭脚本，以支持 P3 一运 和 P5 的全自动功能。");
        ImGui.TextWrapped("请务必在可达鸭中正确设置全队职能，P5 分摊将自动跟随 MT。");
        ImGui.TextWrapped("此功能仅限诗人使用，舞者暂不支持。");

        ImGui.Separator();
        ImGui.Text("脚本链接：");
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 0.7f, 1f, 1f));
        var scriptUrl = "https://raw.githubusercontent.com/kanyeishere/kodakku-script/refs/heads/master/ScriptMaster.json";
        ImGui.InputText("##ScriptLink", ref scriptUrl, 512, ImGuiInputTextFlags.ReadOnly);
        if (ImGui.Button("复制地址"))
        {
            ImGui.SetClipboardText(scriptUrl);
        }
        ImGui.PopStyleColor();

        ImGui.Separator();
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "致谢");
        ImGui.TextWrapped("感谢原作者 天生有灵视 与 Karlin 的辛勤制作与无私分享。");
        ImGui.TextWrapped("本功能仅在其原脚本基础上实现坐标联动，用于 ACR 控制角色移动。");


        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
            BardSettings.Instance.IsReadInfoWindow08 = true;
            DancerSettings.Instance.IsReadInfoWindow08 = true;
            BardSettings.Instance.Save();
            DancerSettings.Instance.Save();
        }
        ImGui.End();
    }
}