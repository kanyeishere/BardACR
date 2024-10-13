using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.IO;

namespace Wotou.Bard.Setting;

/// <summary>
/// 配置文件适合放一些一般不会在战斗中随时调整的开关数据
/// 如果一些开关需要在战斗中调整 或者提供给时间轴操作 那就用QT
/// 非开关类型的配置都放配置里 比如诗人绝峰能量配置
/// </summary>
public class BardSettings
{
    public static BardSettings Instance;

    #region 标准模板代码 可以直接复制后改掉类名即可
    private static string path;
    public static void Build(string settingPath)
    {
        path = Path.Combine(settingPath,nameof(BardSettings) + ".json");
        if (!File.Exists(path))
        {
            Instance = new BardSettings();
            Instance.Save();
            return;
        }
        try
        {
            Instance = JsonHelper.FromJson<BardSettings>(File.ReadAllText(path));
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
    
    public float WandererSongDuration = (float)43; // 第一首歌的持续时间
    public float MageSongDuration = (float)39.2; // 第二首歌的持续时间
    public float ArmySongDuration = (float)39; // 第三首歌的持续时间
    public bool GambleTripleApex = false; // 是否赌两分钟三次绝峰
    public int Opener = 0; // 起手选择
    
    public JobViewSave JobViewSave = new(); // QT设置存档
    
}