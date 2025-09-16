using Dalamud.Bindings.ImGui;

namespace Wotou.Dancer.Setting;

/// <summary>
/// 配置文件适合放一些一般不会在战斗中随时调整的开关数据
/// 如果一些开关需要在战斗中调整 或者提供给时间轴操作 那就用QT
/// 非开关类型的配置都放配置里 比如诗人绝峰能量配置
/// </summary>
public class DancerSettingsUI
{
    public static DancerSettingsUI Instance = new();
    public DancerSettings DancerSettings => DancerSettings.Instance;

    public void Draw()
    {
    }

}