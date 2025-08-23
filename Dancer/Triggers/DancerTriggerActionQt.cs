using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using ImGuiNET;

namespace Wotou.Dancer.Trigger;

/// <summary>
/// 配置文件适合放一些一般不会在战斗中随时调整的开关数据
/// 如果一些开关需要在战斗中调整 或者提供给时间轴操作 那就用QT
/// 非开关类型的配置都放配置里 比如诗人绝峰能量配置
/// </summary>
public class DancerTriggerActionQt : ITriggerAction
{
    public string DisplayName { get; } = "Dancer/QT";
    public string Remark { get; set; }

    public string Key = "";
    public bool Value;

    // 辅助数据 因为是private 所以不存档
    private int _selectIndex;
    private string[] _qtArray;

    public DancerTriggerActionQt()
    {
        _qtArray = DancerRotationEntry.QT.GetQtArray();
    }

    public bool Draw()
    {
        _selectIndex = Array.IndexOf(_qtArray, Key);
        if (_selectIndex == -1)
        {
            _selectIndex = 0;
        }
        ImGuiHelper.LeftCombo("选择QT", ref this._selectIndex, this._qtArray);
        this.Key = this._qtArray[this._selectIndex];
        ImGui.Text("勾选为开启QT，不勾选则关闭");
        ImGui.Text("开/关  ");
        ImGui.SameLine();
        using (new GroupWrapper())
            ImGui.Checkbox("", ref this.Value);
        return true;
    }

    public bool Handle()
    {
        DancerRotationEntry.QT.SetQt(Key, Value);
        return true;
    }

}