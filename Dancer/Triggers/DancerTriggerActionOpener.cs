using AEAssist.CombatRoutine.Trigger;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using Wotou.Common;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Triggers;

public class DancerTriggerActionOpener : ITriggerAction
{
    public string DisplayName { get; } = "Dancer/起手设置";
    public string Remark { get; set; } = "";

    public DancerOpenerType OpenerType = DancerOpenerType.StandardStep;
    public int OpenerTime = 300;
    public int OpenerStandardStepTime = 15000;
    public int OpenerTechnicalStepTime = 5800;

    private static readonly string[] OpenerOptions = ["小舞起手", "大舞起手"];
    private int _openerIndex;

    public DancerTriggerActionOpener()
    {
        if (DancerSettings.Instance == null) return;

        OpenerType = DancerSettings.Instance.OpenerType;
        OpenerTime = DancerSettings.Instance.OpenerTime;
        OpenerStandardStepTime = DancerSettings.Instance.OpenerStandardStepTime;
        OpenerTechnicalStepTime = DancerSettings.Instance.OpenerTechnicalStepTime;
    }

    public bool Draw()
    {
        ImGui.Separator();
        ImGui.Text("起手设置：");
        ImGui.TextColored(new Vector4(0.3f, 0.9f, 1.0f, 1f),
            OpenerType == DancerOpenerType.TechnicalStep ? "当前：大舞起手" : "当前：小舞起手");

        _openerIndex = OpenerType == DancerOpenerType.TechnicalStep ? 1 : 0;
        if (ImGui.Combo("起手选择", ref _openerIndex, OpenerOptions, OpenerOptions.Length))
            OpenerType = _openerIndex == 1 ? DancerOpenerType.TechnicalStep : DancerOpenerType.StandardStep;

        ImGui.Separator();
        if (OpenerType == DancerOpenerType.StandardStep)
            UiHelper.RightInputInt("倒计时提前使用小舞", ref OpenerStandardStepTime, 5500, 15000, "(毫秒)");
        else
            UiHelper.RightInputInt("倒计时提前使用大舞", ref OpenerTechnicalStepTime, 5500, 15000, "(毫秒)");

        ImGui.Separator();
        UiHelper.RightInputInt("开战前使用舞步结束", ref OpenerTime, 0, 1000, "(毫秒)");
        ImGui.Separator();

        return true;
    }

    public bool Handle()
    {
        DancerSettings.Instance.OpenerType = OpenerType;
        DancerSettings.Instance.OpenerTime = Math.Clamp(OpenerTime, 0, 1000);
        DancerSettings.Instance.OpenerStandardStepTime = Math.Clamp(OpenerStandardStepTime, 5500, 15000);
        DancerSettings.Instance.OpenerTechnicalStepTime = Math.Clamp(OpenerTechnicalStepTime, 5500, 15000);
        DancerSettings.Instance.Save();
        return true;
    }
}