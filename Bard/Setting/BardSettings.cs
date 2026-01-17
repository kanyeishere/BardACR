using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.IO;
using Dalamud.Game.ClientState.JobGauge.Enums;

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
        Instance.InitializeQtValues();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonHelper.ToJson(this));
    }
    #endregion
    
    public float WandererSongDuration = 42.6f; // 第一首歌的持续时间
    public float MageSongDuration = 39.2f; // 第二首歌的持续时间
    public float ArmySongDuration = 39f; // 第三首歌的持续时间
    
    public Song FirstSong = Song.Wanderer; // 第一首歌
    public Song SecondSong = Song.Mage; // 第二首歌
    public Song ThirdSong = Song.Army; // 第三首歌
    
    public bool ResetSongOrder = true; // 是否重置歌曲顺序
    public List<Song> SongOrderOnReset = new() { Song.Wanderer, Song.Mage, Song.Army };

    public bool GambleTripleApex = false; // 是否赌两分钟三次绝峰
    public bool UsePotionInOpener = false; // 是否在开场使用药水
    public int Opener = 0; // 起手选择
    public int OpenerTime = 300; // 起手提前多少时间 （毫秒）
    public float HeartBreakSaveStack = 0f;     //碎心箭保留层数
    public bool WelcomeVoice = true; // 是否开启欢迎声音
    public List<uint> CustomOpenerSkills = new(); // 自定义起手技能列表

    public int UseBattleVoiceBeforeGcdTimeInMs = 1350; //战斗之声和光明神在下个GCD前多久使用（毫秒）
    public int RagingStrikeBeforeGcdTime = 600; //猛者强击在下个GCD前多久使用（毫秒）
    public int WandererBeforeGcdTime = 600; //非起手的贤者歌在下个GCD有多久使用（毫秒）
    public int PotionBeforeGcdTime = 700; // 爆发药水的动画持续时间（毫秒）
    public int GcdAnimationTime = 600; //GCD动画锁时间
    //public int EmpyrealArrowNotBeforeGcdTime = 500; // 九天连箭最晚在下个GCD前多久使用（毫秒）
    
    public bool NaturesMinneWithRecitation = true; // 大地神对齐秘策
    public bool NaturesMinneWithZoe = true; // 大地神对齐活化
    public bool NaturesMinneWithNeutralSect = true; // 大地神对齐中间学派

    public bool IsDailyMode = false; //false 表示高难模式，true 表示日随模式
    public bool ApplyDotOnTrashMobs = false; // 是否在小怪身上使用DOT
    public bool EnableAutoPeloton  = true; // 是否在自动使用速行
    
    public bool ImitateGreenPlayer = false; // 是否模仿绿玩
 
    public bool ShowWardensPaeanPanel = true; // 是否显示光阴神面板
    public int WardensPaeanPanelIconSize = 47; // 光阴神面板图标大小
    public bool IsWardensPanelLocked = false; // 是否锁定光阴神面板位置

    
    public bool IsReadInfoWindow08 = false;
    
    public bool IsOpenCommandWindow = true; // 是否开启指令窗口
    
    public Dictionary<string, bool> UserDefinedQtValues = new(); // 用户自定义QT值

    public List<string> QwertyList = [];
    
    public string UnlockPassword = ""; // 解锁密码
    
    public Dictionary<string, bool> SelectedTimeLinesForUpdate = new();
    
    /// <summary>
    /// 初始化用户自定义的 QT 值。如果没有自定义值，则使用默认值。
    /// </summary>
    public void InitializeQtValues()
    {
        foreach (var def in BardQtHotkeyRegistry.Qts)
        {
            if (!UserDefinedQtValues.ContainsKey(def.Key))
            {
                UserDefinedQtValues[def.Key] = def.Default; // 只写默认值
            }
        }
    }
    
    public JobViewSave JobViewSave = new JobViewSave()
    {
        MainColor = new(0f, 0.3012f, 0.2306f, 1f),
        QtLineCount = 2,
        QtWindowBgAlpha = 0.1f,
    }; // QT设置存档
}