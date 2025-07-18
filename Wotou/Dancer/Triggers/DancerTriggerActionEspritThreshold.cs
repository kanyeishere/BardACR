using AEAssist.CombatRoutine.Trigger;
using ImGuiNET;
using System.Numerics;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Triggers
{
    public class DancerTriggerActionEspritThreshold : ITriggerAction
    {
        public string DisplayName => "Dancer/伶俐阈值控制 New";
        public string Remark { get; set; } = "";

        public int SaberDanceEspritThreshold = 70;
        public int TillanaEspritThreshold = 20;
        public int TillanaLastGcdEspritThreshold = 30;

        public bool Draw()
        {
            ImGui.Separator();
            ImGui.Text("这些数值修改之后，战斗重新开始时不会自动重置");
            ImGui.NewLine();
            ImGui.Text("伶俐（Esprit）阈值设置：");

            ImGui.Text("非爆发期使用剑舞（≥）");
            ImGui.SliderInt("##SaberDance", ref SaberDanceEspritThreshold, 50, 100);
            ImGui.Separator();

            ImGui.Text("爆发期使用提拉纳（≤）");
            ImGui.SliderInt("##Tillana", ref TillanaEspritThreshold, 0, 50);
            ImGui.Separator();

            ImGui.Text("爆发最后1GCD使用提拉纳（≤）");
            ImGui.SliderInt("##TillanaLastGcd", ref TillanaLastGcdEspritThreshold, 0, 50);
            ImGui.Separator();

            return true;
        }

        public bool Handle()
        {
            DancerSettings.Instance.SaberDanceEspritThreshold = SaberDanceEspritThreshold;
            DancerSettings.Instance.TillanaEspritThreshold = TillanaEspritThreshold;
            DancerSettings.Instance.TillanaLastGcdEspritThreshold = TillanaLastGcdEspritThreshold;
            return true;
        }
    }
}