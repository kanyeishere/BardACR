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
        Instance.InitializeQtValues();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonHelper.ToJson(this));
    }
    #endregion
    
    public bool UsePotionInOpener = false; // 是否在开场使用药水
    public bool ShowDancePartnerPanel = true; // 是否显示舞者配偶面板
    public bool ShowEnAvantPanel = true; // 是否显示前跳面板
    public int DancePartnerPanelIconSize = 47; // 舞者配偶面板图标大小
    public int EnAvantPanelIconSize = 47; // 前跳面板图标大小
    public bool isDancePartnerPanelLocked = false; // 是否锁定舞配面板
    public bool isEnAvantPanelLocked = false; // 是否锁定前跳面板
    public bool UseDancePartnerMacro = true; // 是否使用舞伴宏
    public bool WelcomeVoice = true; // 是否开启欢迎声音
    public bool DanceDistanceWarning = true; // 是否开启舞步距离警告

    public string DancePartnerMacroText =
        "/p <t> 欧尼酱， <t> 欧尼酱 <se.2>\n/p 阔咧[闭式舞姿]跌酷烈跌\n/p 要是 啊呐哒のCP西哒啦 发飙西奈哟呐\n/p pr...pr...哦依稀！~\n/p <t> 欧尼酱 一托库叽 [尝一口治疗之华尔兹]\n/p [倒吸气]<t> 欧尼酱~~ <se.7>\n/p 昂呐哒のCP莫西知道昂呐哒跟瓦大喜做[舞伴]\n/p 吃醋西奈哟呐~~\n/p <t> 欧尼酱[伶俐]给瓦大吸<se.6>\n/p 啊呐哒のCP莫西知道了不会揍瓦大喜吧~~\n/p 阔哇一呐~啊呐哒のCP 不像瓦大喜\n/p 塔哒[防守之桑巴]<t> 欧尼酱";

    public int OpenerTime = 300; // 起手提前多少时间 （毫秒）
    public int OpenerStandardStepTime = 15000; // 起手标准舞时间
    public int StandardStepCdTolerance = 1000; // 标准舞CD容差
    public int SaberDanceEspritThreshold = 70; //剑舞阈值
    public int TillanaEspritThreshold = 20; //提拉纳阈值
    public int TillanaLastGcdEspritThreshold = 30; //提拉纳爆发期最后1G阈值
    
    public bool IsDailyMode = false; //false 表示高难模式，true 表示日随模式
    public bool EnableAutoPeloton  = true; // 是否在自动使用速行
    public bool EnableAutoDancing = true; // 是否在自动使用舞步
    public bool EnableAutoDancePartner = true; // 是否在自动使用舞伴
    public bool EnableAutoDancePartnerInFullAutoMode = true; // FA模式自动舞伴
    
    public bool IsReadInfoWindow08 = false;
    
    public bool IsOpenCommandWindow = true;
    
    // 扇舞保留层数
    public int FanDanceSaveStack = 3;
    
    public List<string> QwertyList = [];
    
    public string UnlockPassword = ""; // 解锁密码
    
    public Dictionary<string, bool> SelectedTimeLinesForUpdate = new();
    
    public int M6SAutoTargetCount = 1; 
    
    // 用户自定义的 QT 配置值
    public Dictionary<string, bool> UserDefinedQtValues = new(); // 用户自定义QT值

    /// <summary>
    /// 初始化用户自定义的 QT 值。如果没有自定义值，则使用默认值。
    /// </summary>
    public void InitializeQtValues()
    {
        foreach (var def in DancerQtHotkeyRegistry.Qts)
        {
            if (!UserDefinedQtValues.ContainsKey(def.Key))
            {
                UserDefinedQtValues[def.Key] = def.Default; // 只写默认值
            }
        }
    }
    
    public JobViewSave JobViewSave = new JobViewSave()
    {
        MainColor = new(0.3f, 0.00f, 0.12f, 1f),
        QtLineCount = 2,
        QtWindowBgAlpha = 0.1f,
    }; 
}