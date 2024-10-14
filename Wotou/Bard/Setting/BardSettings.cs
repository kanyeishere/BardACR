using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.IO;
using Dalamud.Interface.Utility.Raii;

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
    
    public float WandererSongDuration = (float)42.6; // 第一首歌的持续时间
    public float MageSongDuration = (float)39.2; // 第二首歌的持续时间
    public float ArmySongDuration = (float)39; // 第三首歌的持续时间
    
    public float WandererSongDefaultDuration = (float)42.6; // 第一首歌的持续时间
    public float MageSongDefaultDuration = (float)39.2; // 第二首歌的持续时间
    public float ArmySongDefaultDuration = (float)39; // 第三首歌的持续时间
    
    
    public bool GambleTripleApex = false; // 是否赌两分钟三次绝峰
    public bool UsePotionInOpener = false; // 是否在开场使用药水
    public int Opener = 0; // 起手选择

    public int BattleVoiceGcdTime = 1280; //战斗之声和光明神在下个GCD前多久使用（毫秒）
    public int RagingStrikeGcdTime = 530; //猛者强击在下个GCD前多久使用（毫秒）
    public int WandererGcdTime = 530; //非起手的贤者歌在下个GCD有多久使用（毫秒）
    public int PotionGcdTime = 870; // 爆发药水的动画持续时间（毫秒）
    
    public JobViewSave JobViewSave = new JobViewSave()
    {
        MainColor = new(0f, 0.3012f, 0.2306f, 1f),
        QtLineCount = 2,
        QtWindowBgAlpha = 0.1f,
    }; // QT设置存档
    
    
}