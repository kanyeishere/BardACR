using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.IO;

namespace Wotou.Dancer.Setting;

/// <summary>
/// 配置文件适合放一些一般不会在战斗中随时调整的开关数据
/// 如果一些开关需要在战斗中调整 或者提供给时间轴操作 那就用QT
/// 非开关类型的配置都放配置里 比如诗人绝峰能量配置
/// </summary>
public class DancerSettings
{
    public static DancerSettings Instance;

    #region 标准模板代码 可以直接复制后改掉类名即可
    private static string path;
    public static void Build(string settingPath)
    {
        path = Path.Combine(settingPath,nameof(DancerSettings)+ ".json");
        if (!File.Exists(path))
        {
            Instance = new DancerSettings();
            Instance.Save();
            return;
        }
        try
        {
            Instance = JsonHelper.FromJson<DancerSettings>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Instance = new();
            LogHelper.Error(e.ToString());
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonHelper.ToJson(this));
    }
    #endregion
    
    public bool UsePotionInOpener = false; // 是否在开场使用药水
    public int OpenerTime = 300; // 起手提前多少时间 （毫秒）
    public int OpenerStandardStepTime = 15000; // 起手标准舞时间
    
    public JobViewSave JobViewSave = new JobViewSave()
    {
        MainColor = new(0.3f, 0.00f, 0.12f, 1f),
        QtLineCount = 2,
        QtWindowBgAlpha = 0.1f,
    }; 
}