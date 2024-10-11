using Wotou.Bard.Setting;
using Wotou.Bard.SlotResolvers.GCD;
using Wotou.Bard.SlotResolvers.Ability;
using Wotou.Bard.Triggers;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using AEAssist.Helper;
using ImGuiNET;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;

namespace Wotou.Bard;

// 重要 类一定要Public声明才会被查找到
public class BardRotationEntry : IRotationEntry
{
    public string AuthorName { get; set; } = "Wotou";

    

    // 逻辑从上到下判断，通用队列是无论如何都会判断的 
    // gcd则在可以使用gcd时判断
    // offGcd则在不可以使用gcd 且没达到gcd内插入能力技上限时判断
    // pvp环境下 全都强制认为是通用队列
    private List<SlotResolverData> SlotResolvers = new ()
    {
        // 通用队列 不管是不是gcd 都会判断的逻辑
        //new(new XXXXXXXX(),SlotMode.Always),
        
        // gcd队列
        
        // Dots
        new(new BardDotGcd(),SlotMode.Gcd),
        new (new BardIronJawsGcd(),SlotMode.Gcd),
        // 1与触发1
        new(new BardBaseGcd(),SlotMode.Gcd),
        
        // offGcd队列
        new(new BardSongAbility(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_Bloodletter(),SlotMode.OffGcd),
    };
    

    public Rotation Build(string settingFolder)
    {
        // 初始化设置
        BardSettings.Build(settingFolder);
        // 初始化QT （依赖了设置的数据）
        BuildQT();
        if (!25785U.IsReady())
        {
            LogHelper.Print("25785U is not ready");
        }
        
        var rot = new Rotation(SlotResolvers)
        {
            TargetJob = Jobs.Bard,
            AcrType = AcrType.Normal,
            MinLevel = 0,
            MaxLevel = 90,
            Description = "这是一个测试描述\n123123123123",
        };
        
        // 添加各种事件回调
        rot.SetRotationEventHandler(new BardRotationEventHandler());
        // 添加QT开关的时间轴行为
        rot.AddTriggerAction(new TriggerAction_QT());

        return rot; 
    }
    
    // 声明当前要使用的UI的实例 示例里使用QT
    public static JobViewWindow QT { get; private set; }
    
    // 如果你不想用QT 可以自行创建一个实现IRotationUI接口的类
    public IRotationUI GetRotationUI()
    {
        return QT;
    }
    
    // 构造函数里初始化QT
    public void BuildQT()
    {
        // JobViewSave是AE底层提供的QT设置存档类 在你自己的设置里定义即可
        // 第二个参数是你设置文件的Save类 第三个参数是QT窗口标题
        QT = new JobViewWindow(BardSettings.Instance.JobViewSave, BardSettings.Instance.Save, "AE BRD [仅作为开发示范]");
        QT.SetUpdateAction(OnUIUpdate); // 设置QT中的Update回调 不需要就不设置

        //添加QT分页 第一个参数是分页标题 第二个是分页里的内容
        QT.AddTab("通用", DrawQtGeneral);
        QT.AddTab("Dev", DrawQtDev);

        // 添加QT开关 第二个参数是默认值 (开or关) 第三个参数是鼠标悬浮时的tips
        QT.AddQt(QTKey.UseBaseGcd, true, "是否使用基础的Gcd");
        QT.AddQt(QTKey.DOT, true);
        QT.AddQt(QTKey.Song, true);
        QT.AddQt(QTKey.UsePotion,false);

        // 添加快捷按钮 (带技能图标)
        QT.AddHotkey("战斗之声",
            new HotKeyResolver_NormalSpell(BardDefinesData.Spells.BattleVoice, SpellTargetType.Self));
        QT.AddHotkey("失血",
            new HotKeyResolver_NormalSpell(BardDefinesData.Spells.HeartBreak, SpellTargetType.Target));
        QT.AddHotkey("爆发药", new HotKeyResolver_Potion());
        QT.AddHotkey("极限技", new HotKeyResolver_LB());
        
        /*
        // 这是一个自定义的快捷按钮 一般用不到
        // 图片路径是相对路径 基于AEAssist(C|E)NVersion/AEAssist
        // 如果想用AE自带的图片资源 路径示例: Resources/AE2Logo.png
        QT.AddHotkey("极限技", new HotkeyResolver_General("#自定义图片路径", () =>
        {
            // 点击这个图片会触发什么行为
            LogHelper.Print("你好");
        }));
        */
    }

    // 设置界面
    public void OnDrawSetting()
    {
        BardSettingUI.Instance.Draw();
    }

    public void OnUIUpdate()
    {

    }
    
    public void DrawQtGeneral(JobViewWindow jobViewWindow)
    {
        ImGui.Text("画通用信息");
    }

    public void DrawQtDev(JobViewWindow jobViewWindow)
    {
        ImGui.Text("画Dev信息");
        foreach (var v in jobViewWindow.GetQtArray())
        {
            ImGui.Text($"Qt按钮: {v}");
        }

        foreach (var v in jobViewWindow.GetHotkeyArray())
        {
            ImGui.Text($"Hotkey按钮: {v}");
        }
    }

    
    public void Dispose()
    {
        // 释放需要释放的东西 没有就留空
    }
}