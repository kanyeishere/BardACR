using Wotou.Bard.Setting;
using Wotou.Bard.SlotResolvers.GCD;
using Wotou.Bard.SlotResolvers.Ability;
using Wotou.Bard.Triggers;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using AEAssist.GUI;
using AEAssist.Helper;
using ImGuiNET;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Opener;

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
        new SlotResolverData(new BardDotGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardApexMaxGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardBlastArrowMaxGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardRefulgentArrowMaxGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardRadiantEncoreGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardResonantArrowGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardIronJawsGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardApexGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardBlastArrowGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardBaseGcd(),SlotMode.Gcd),
        
        
        // offGcd队列
        new SlotResolverData(new BardRagingStrikesAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardBattleVoiceAndRadiantFinaleAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardEmpyrealArrowAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardHeartBreakMaxChargeAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardPitchPerfectAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardBarrageAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardSideWinderAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardHeartBreakAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardSongAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardPotionAbility(),SlotMode.OffGcd),
    };
    

    public Rotation Build(string settingFolder)
    {
        // 初始化设置
        BardSettings.Build(settingFolder);
        // 初始化QT （依赖了设置的数据）
        BuildQT();
        
        var rot = new Rotation(SlotResolvers)
        {
            TargetJob = Jobs.Bard,
            AcrType = AcrType.HighEnd,
            MinLevel = 70,
            MaxLevel = 100,
            Description = "诗人ACR\n适配技速2.48-2.5\n请在fuck插件中修改动画锁至530ms！！（重要）\n并且关闭全局能力技不卡GCD！！（重要）\n这样设置爆发应该能打满警察网8G，光明神9G和强者猛击9G" ,
        };
        
        // 添加各种事件回调
        rot.SetRotationEventHandler(new BardRotationEventHandler());
        rot.AddOpener(GetOpener);
        // 添加QT开关的时间轴行为
        rot.AddTriggerAction(new TriggerAction_QT());
        
        // 添加时间轴控制
        rot.AddTriggerCondition(new BardSongTimerCondition());

        return rot; 
    }

    private IOpener? GetOpener(uint level)
    {
        switch (BardSettings.Instance.Opener)
        {
            case 0:
                return new BardOpener100();
        }
        return new BardOpener100();
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
        QT = new JobViewWindow(BardSettings.Instance.JobViewSave, BardSettings.Instance.Save, "Wotou Bard 诗人]");
        QT.SetUpdateAction(OnUIUpdate); // 设置QT中的Update回调 不需要就不设置

        //添加QT分页 第一个参数是分页标题 第二个是分页里的内容
        QT.AddTab("通用", DrawQtGeneral);
        QT.AddTab("Dev", DrawQtDev);

        // 添加QT开关 第二个参数是默认值 (开or关) 第三个参数是鼠标悬浮时的tips
        QT.AddQt(QTKey.Aoe, true, "是否使用AoE");
        QT.AddQt(QTKey.Burst, true, "是否使用爆发");
        QT.AddQt(QTKey.Apex, true, "是否使用绝峰箭");
        QT.AddQt(QTKey.HeartBreak, true, "是否攒碎心箭进团辅");
        QT.AddQt(QTKey.DOT, true, "是否使用DoT");
        QT.AddQt(QTKey.Song, true, "是否使用歌曲");
        QT.AddQt(QTKey.UsePotion,false,"是否使用爆发药水");
        //QT.AddQt(QTKey.UsePotionAsap,false,"是否在CD好了就吃爆发药水");

        // 添加快捷按钮 (带技能图标)
        QT.AddHotkey("防击退", new HotKeyResolver_NormalSpell(BardDefinesData.Spells.ArmsLength, SpellTargetType.Target));
        QT.AddHotkey("内丹",
            new HotKeyResolver_NormalSpell(BardDefinesData.Spells.SecondWind, SpellTargetType.Target));
        QT.AddHotkey("行吟",new HotKeyResolver_NormalSpell(BardDefinesData.Spells.Troubadour, SpellTargetType.Target));
        QT.AddHotkey("大地神",new HotKeyResolver_NormalSpell(BardDefinesData.Spells.NaturesMinne, SpellTargetType.Target));
        QT.AddHotkey("冲刺",new HotKeyResolver_NormalSpell(3, SpellTargetType.Target));
        QT.AddHotkey("后跳",new HotKeyResolver_NormalSpell(BardDefinesData.Spells.RepellingShot, SpellTargetType.Target));
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
        ImGui.Text("诗人ACR\n适配技速2.48-2.5\n精细调整过能力技插入窗口，\n所以请在fuck插件中修改动画锁至530ms！！（重要）\n并且关闭全局能力技不卡GCD！！（重要）\n打零式建议军神歌时间设置略长一些，爆发QT开启的情况下会120s自动切旅神");
        ImGuiHelper.LeftInputFloat("旅神歌时长", ref BardSettings.Instance.WandererSongDuration, 5f, 45f);
        ImGuiHelper.LeftInputFloat("贤者歌时长", ref BardSettings.Instance.MageSongDuration, 5f, 45f);
        ImGuiHelper.LeftInputFloat("军神歌时长", ref BardSettings.Instance.ArmySongDuration, 5f, 45f);
        ImGui.Checkbox("是否赌每120秒三根绝峰箭（建议关闭）", ref BardSettings.Instance.GambleTripleApex);
        var opener = "";
        switch (BardSettings.Instance.Opener)
        {
            case 0:
                opener = "70-100级 3G团辅起手";
                break;
        }
        if (ImGui.BeginCombo("起手选择", opener))
        {
            if (ImGui.Selectable("70-100级 3G团辅起手"))
                BardSettings.Instance.Opener = 0;
            ImGui.EndCombo();
        }
        if (ImGui.Button("Save"))
            BardSettings.Instance.Save();
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