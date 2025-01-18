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
        if (BardSettings.Instance.IsReadInfoWindow04)
            return;
        if (DancerSettings.Instance.IsReadInfoWindow04)
            return;
        return;
        ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(mainViewport.Pos.X + mainViewport.Size.X / 2f, mainViewport.Pos.Y + mainViewport.Size.Y / 2f), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(800f, 800f), ImGuiCond.Always);
        ImGui.SetNextWindowFocus();
        ImGui.Begin("", ref InfoWindow.isWindowOpen);
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "特别说明");
        ImGui.Separator();
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "本次更新了M4S诗人6分钟爆发-落地诗心的精修轴v1125.01（是的，又又又又更新了）");
        ImGui.TextWrapped("修复了转场后半爆发期不打双九天的问题（后半的爆发也因此延后2s）");
        ImGui.TextWrapped("同时，再次提醒您上次更新了M3S与M4S的舞者小舞无损轴 v1123.01");
        ImGui.TextWrapped("如果您的时间轴版本过低，请你及时前往云时间轴，选择对应职业搜索，并更新");
        
        
        ImGui.Text("");
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "伪装绿玩手打：全新功能强势上线");
        ImGui.TextWrapped("本次更新为诗人ACR引入了全新且致命的功能——模仿绿玩手打循环！");
        ImGui.TextWrapped("新功能的效果是：在您释放旅神歌与猛者强击前，会模仿绿玩手动操作，插入伤腿与伤足");
        ImGui.TextWrapped("开启这个功能，让小警察们在鉴挂的路上感受来自艺术的震撼，让他们的怀疑流于空中，徒增混乱");
        ImGui.TextWrapped("想体验这种极致的搅局快感吗？赶紧去诗人ACR面板->高级设置开启此功能");
        ImGui.TextWrapped("让你在绿玩和红玩之间自由切换，成为鉴挂局中最具谜团的存在！");
        ImGui.TextWrapped("让鉴挂的猎巫行动陷入迷雾，分不清真相，更分不清你！来吧，搅乱这个无聊的世界！");
        
        ImGui.Text("");
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "新版“强对齐”功能使用说明");
        ImGui.TextWrapped("本次更新重写了诗人“强对齐”功能的实现逻辑");
        ImGui.TextWrapped("新版不仅保留了旧版的精准团辅释放功能");
        ImGui.TextWrapped("最关键的是：新版无停手！");
        ImGui.TextWrapped("操作丝滑流畅，宛如绿玩亲手操刀！让鉴挂小警察怀疑人生");
        
        ImGui.Text("");
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "关于与绿玩配合与调整团辅节奏的建议");
        ImGui.TextWrapped("有网络评论反馈，诗人ACR的团辅释放过于整齐，容易被认出是自动循环");
        ImGui.TextWrapped("而与绿玩队友配合时，由于绿玩团辅多为手动操作，释放时间可能略有延后。为此，建议您：");
        ImGui.TextWrapped("请灵活使用“强对齐”功能");
        ImGui.TextWrapped("如果发现绿玩队友的团辅释放时间比自己的要晚，可以关闭“强对齐”功能，让自己的团辅自然延后，与绿玩队友保持节奏一致。");
        ImGui.TextWrapped("与之相反，如果发现绿玩队友的团辅释放时间比自己的要早，可以再次开启“强对齐”功能，让自己的团辅不延后");
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "注意，使用M4S诗人的精修轴时，请在转场前全程开启强对齐，确保爆发能在上天前打完");
        
        ImGui.Text("");
        
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.0f, 1f), "关于舞者使用中小舞被动延后的建议");
        ImGui.TextWrapped("如果您在使用舞者的过程中发现有小舞被动延后现象，请尝试以下方法：");
        ImGui.TextWrapped("1.请您在设置-全局设置中勾上优化Gcd偏移");
        ImGui.TextWrapped("    并手动填写网络延迟3/5/10（请从小到大进行尝试，直到小舞不延后为止）");
        ImGui.TextWrapped("2.或者改用2.49技速，这样可以减少小舞延后的可能性");

        
        ImGui.Separator();
        if (ImGui.Button("已知悉"))
        {
            InfoWindow.isWindowOpen = false;
            BardSettings.Instance.IsReadInfoWindow04 = true;
            DancerSettings.Instance.IsReadInfoWindow04 = true;
            BardSettings.Instance.Save();
            DancerSettings.Instance.Save();
        }
        ImGui.End();
    }
}