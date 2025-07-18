using AEAssist.CombatRoutine.Trigger;
using ImGuiNET;
using System.Numerics;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Triggers
{
    public class DancerTriggerActionPotionMode : ITriggerAction
    {
        public string DisplayName => "Dancer/起手or2分钟爆发药";
        public string Remark { get; set; } = "";

        public bool UsePotionInOpener = true;

        public bool Draw()
        {
            ImGui.Separator();
            ImGui.Text("爆发药设置：");

            ImGui.TextColored(new Vector4(0.3f, 0.9f, 1.0f, 1f),
                UsePotionInOpener ? "→ 起手吃爆发药" : "→ 两分钟爆发吃爆发药");

            ImGui.TextColored(new Vector4(0.9f, 0.6f, 0.2f, 1f),
                "请在 QT 面板中开启“爆发药”开关，否则不会使用药水");

            ImGui.Checkbox("起手吃爆发药", ref UsePotionInOpener);
            return true;
        }

        public bool Handle()
        {
            DancerSettings.Instance.UsePotionInOpener = UsePotionInOpener;
            return true;
        }
    }
}