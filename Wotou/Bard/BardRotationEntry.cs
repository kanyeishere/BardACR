using System.Numerics;
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
using Wotou.Bard.Trigger;

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
        new SlotResolverData(new BardIronJawsGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardApexGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardBlastArrowGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardRadiantEncoreGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardResonantArrowGcd(),SlotMode.Gcd),
        new SlotResolverData(new BardBaseGcd(),SlotMode.Gcd),
        
        
        // offGcd队列
        new SlotResolverData(new BardSongMaxAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardRagingStrikesAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardBattleVoiceAndRadiantFinaleAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardEmpyrealArrowAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardPitchPerfectMaxAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardHeartBreakMaxChargeAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardBarrageAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardSideWinderAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardPitchPerfectAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardSongAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardHeartBreakAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardPotionAbility(),SlotMode.OffGcd),
        new SlotResolverData(new BardNaturesMinneAbility(),SlotMode.OffGcd),
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
            Description = "诗人ACR" +
                          "\n 更新日志：10.18.1 " +
                          "\n- 现在可以在战斗中动态修改歌轴了! " +
                          "\n 更新日志：10.17.3 " +
                          "\n- 添加高级设置-九天连箭最晚在下个GCD前多久使用" +
                          "\n- 修复了一些非对齐旅神的特殊循环中的切歌问题" +
                          "\n- 添加非团辅期灵魂之声不足80的处理逻辑（脸黑...这都给我碰上了）" +
                          "\n 更新日志：10.16.4 " +
                          "\n- 优化旅神歌的插入时机" +
                          "\n- 优化猛者强击的插入时机" +
                          "\n- 优化战斗之声和光明神的插入时机" +
                          "\n- 添加2G团辅起手（零式4层特化起手）" +
                          "\n- 添加预设歌轴选择" +
                          "\n- 添加防呆设计，如果你没关全局能力技不卡GCD，聊天框内会有提示" +
                          "\n- 修改续毒逻辑，现在鹰眼buff更不容易被覆盖了" +
                          "\n- 修复高级设置的保存功能" +
                          "\n- 修改高级设置的描述" +
                          "\n- 添加重置高级设置",
        };
        
        // 添加各种事件回调
        rot.SetRotationEventHandler(new BardRotationEventHandler());
        rot.AddOpener(GetOpener);
        
        // 添加时间轴行为
        rot.AddTriggerAction(new BardTriggerActionQt());
        rot.AddTriggerAction(new BardSongDurationAction());
        
        // 添加时间轴控制
        rot.AddTriggerCondition(new BardSongTimerCondition());
        rot.AddTriggerCondition(new BardActiveSongCondition());
        rot.AddTriggerCondition(new BardSoulVoiceCondition());
        rot.AddTriggerCondition(new BardRepertoireCondition());
        
        return rot; 
    }

    private IOpener? GetOpener(uint level)
    {
        switch (BardSettings.Instance.Opener)
        {
            case 0:
                return new Bard3GOpener100();
            case 1:
                return new Bard2GOpener100();
        }
        return new Bard3GOpener100();
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
        QT.AddQt(QTKey.Aoe, true, "是否使用AOE");
        QT.AddQt(QTKey.Burst, true, "是否使用爆发");
        QT.AddQt(QTKey.BurstWithWanderer, true, "爆发是否对齐旅神");
        QT.AddQt(QTKey.Apex, true, "是否使用绝峰箭");
        QT.AddQt(QTKey.HeartBreak, true, "是否攒碎心箭进团辅");
        QT.AddQt(QTKey.DOT, true, "是否使用DOT");
        QT.AddQt(QTKey.Song, true, "是否使用歌曲");
        QT.AddQt(QTKey.NatureMinne,true,"大地神自动对齐秘策/活化/中间学派");
        QT.AddQt(QTKey.UsePotion,false,"是否使用爆发药水");
        QT.AddQt(QTKey.Debug,false,"是否打印调试信息");
        
        //QT.AddQt(QTKey.UsePotionAsap,false,"是否在CD好了就吃爆发药水");

        // 添加快捷按钮 (带技能图标)
        QT.AddHotkey("防击退", new HotKeyResolver_NormalSpell(BardDefinesData.Spells.ArmsLength, SpellTargetType.Target));
        QT.AddHotkey("内丹",
            new HotKeyResolver_NormalSpell(BardDefinesData.Spells.SecondWind, SpellTargetType.Target));
        QT.AddHotkey("行吟",new HotKeyResolver_NormalSpell(BardDefinesData.Spells.Troubadour, SpellTargetType.Target));
        QT.AddHotkey("大地神",new HotKeyResolver_NormalSpell(BardDefinesData.Spells.NaturesMinne, SpellTargetType.Target));
        QT.AddHotkey("疾跑", (IHotkeyResolver) new HotKeyResolver_疾跑());
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
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.4314f, 0.6667f, 0.5569f, 1));

        if (ImGui.CollapsingHeader("   重要说明"))
        {
            ImGui.Text("诗人ACR\n适配技速2.48-2.5\n精细调整过能力技插入窗口，所以请在fuck插件中修改动画锁至530ms（重要）");
            ImGui.TextColored(new Vector4(1, 0.7f, 0, 1),"并且关闭全局能力技不卡GCD（重要）");
            ImGui.Separator();
            ImGui.Text("如果你希望打满警察网上要求的爆发8G，光明神9G，战斗之声9G和猛者强击9G\n那你需要根据你的网络延迟，精细调节fuck中的动画锁数值\n直到你连续两个能力技插入间隔在620ms以下（这个数字可以在Logs网站上查看）\n但也别让间隔低于520ms，会有概率被Logs网站标红");
            ImGui.Separator();
            ImGui.Text("更新日志：10.18.1 " +
                       "\n- 现在可以在战斗中动态修改歌轴了! " +
                       "\n更新日志：10.17.3 " +
                       "\n- 添加高级设置-九天连箭最晚在下个GCD前多久使用" +
                       "\n- 修复了一些非对齐旅神的特殊循环中的切歌问题" +
                       "\n- 添加非团辅期灵魂之声不足80的处理逻辑（脸黑...这都给我碰上了）" +
                       "\n更新日志：10.16.4 " +
                       "\n- 优化旅神歌的插入时机" +
                       "\n- 优化猛者强击的插入时机" +
                       "\n- 优化战斗之声和光明神的插入时机" +
                       "\n- 添加2G团辅起手（零式4层特化起手）" +
                       "\n- 添加预设歌轴选择" +
                       "\n- 添加防呆设计，如果你没关全局能力技不卡GCD，聊天框内会有提示" +
                       "\n- 修改续毒逻辑，现在鹰眼buff更不容易被覆盖了" +
                       "\n- 修复高级设置的保存功能" +
                       "\n- 修改高级设置的描述" +
                       "\n- 添加重置高级设置") ;

        }
        ImGui.Separator();
        if (ImGui.CollapsingHeader("   基础设置"))
        {
            ImGui.Text("木桩Boss建议军神歌设置略长些，开启爆发和对齐旅神时，120秒会自动切旅神");
            if (BardSettings.Instance.WandererSongDuration + BardSettings.Instance.MageSongDuration +
                BardSettings.Instance.ArmySongDuration < 120)
                ImGui.TextColored(new Vector4(1, 0.7f, 0, 1),"警告，你设置的三首歌累计时长小于120秒，建议设置略长于120秒");
            
            ImGuiHelper.LeftInputFloat("旅神歌时长", ref BardSettings.Instance.WandererSongDuration, 3f, 45f);
            ImGuiHelper.LeftInputFloat("贤者歌时长", ref BardSettings.Instance.MageSongDuration, 3f, 45f);
            ImGuiHelper.LeftInputFloat("军神歌时长", ref BardSettings.Instance.ArmySongDuration, 3f, 45f);
            var songTimelineSelectionIndex = 0;
            var songTimelineSelection = "";
            if ( BardSettings.Instance.WandererSongDuration == 42.6f && BardSettings.Instance.MageSongDuration == 39.2f && BardSettings.Instance.ArmySongDuration == 39f)
                songTimelineSelectionIndex = 1;
            if ( BardSettings.Instance.WandererSongDuration == 42.6f && BardSettings.Instance.MageSongDuration == 42.2f && BardSettings.Instance.ArmySongDuration == 36f)
                songTimelineSelectionIndex = 2;
            if ( BardSettings.Instance.WandererSongDuration == 42.6f && BardSettings.Instance.MageSongDuration == 33.4f && BardSettings.Instance.ArmySongDuration == 45f)
                songTimelineSelectionIndex = 3;
            if ( BardSettings.Instance.WandererSongDuration == 42.6f && BardSettings.Instance.MageSongDuration == 42.2f && BardSettings.Instance.ArmySongDuration == 45f)
                songTimelineSelectionIndex = 4;
            if ( BardSettings.Instance.WandererSongDuration == 45f && BardSettings.Instance.MageSongDuration == 45f && BardSettings.Instance.ArmySongDuration == 45f)
                songTimelineSelectionIndex = 5;
            switch (songTimelineSelectionIndex)
            {
                case 0:
                    songTimelineSelection = "请选择";
                    break;
                case 1:
                    songTimelineSelection = "43-40-37 建议2.49-2.50技速";
                    break;
                case 2:
                    songTimelineSelection = "43-43-34 建议2.48技速";
                    break;
                case 3:
                    songTimelineSelection = "43-34-43 90级歌轴";
                    break;
                case 4:
                    songTimelineSelection = "43-43-45 特化歌轴";
                    break;
                case 5:
                    songTimelineSelection = "45-45-45 特化歌轴";
                    break;
            }
            if (ImGui.BeginCombo("快速歌轴设置", songTimelineSelection))
            {
                if (ImGui.Selectable("请选择")) {}
                if (ImGui.Selectable("43-40-37 建议2.49-2.50技速"))
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 39.2f;
                    BardSettings.Instance.ArmySongDuration = 39f;
                }

                if (ImGui.Selectable("43-43-34 建议2.48技速") )
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 42.2f;
                    BardSettings.Instance.ArmySongDuration = 36f;
                }
                
                if (ImGui.Selectable("43-34-43 90级歌轴") )
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 33.4f;
                    BardSettings.Instance.ArmySongDuration = 45f;
                }

                if (ImGui.Selectable("43-43-45 特化歌轴"))
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 42.2f;
                    BardSettings.Instance.ArmySongDuration = 45f;
                }

                if (ImGui.Selectable("45-45-45 特化歌轴"))
                {
                    BardSettings.Instance.WandererSongDuration = 45f;
                    BardSettings.Instance.MageSongDuration = 45f;
                    BardSettings.Instance.ArmySongDuration = 45f;
                }
                ImGui.EndCombo();
            }
            ImGui.Separator();
            var opener = "";
            switch (BardSettings.Instance.Opener)
            {
                case 0:
                    opener = "70-100级 3G团辅起手";
                    break;
                case 1:
                    opener = "70-100级 2G团辅起手";
                    break;
            }
            if (ImGui.BeginCombo("起手选择", opener))
            {
                if (ImGui.Selectable("70-100级 3G团辅起手"))
                    BardSettings.Instance.Opener = 0;
                if (ImGui.Selectable("70-100级 2G团辅起手"))
                    BardSettings.Instance.Opener = 1;
                ImGui.EndCombo();
            }
            ImGuiHelper.LeftInputInt("倒计时提前使用起手  (毫秒)", ref BardSettings.Instance.OpenerTime, 0, 1000);
            ImGui.Separator();
            ImGui.Text("爆发药设置：" + (BardSettings.Instance.UsePotionInOpener ? "起手吃" : "2分钟爆发吃"));
            if (!QT.GetQt("爆发药"))
                ImGui.TextColored(new Vector4(0.7f, 0.8f, 0.0f, 1.0000f),"如果你希望使用爆发药，请在QT面板中开启爆发药开关");
            ImGui.Checkbox("起手吃爆发药", ref BardSettings.Instance.UsePotionInOpener);
            ImGui.Separator();
            if (BardSettings.Instance.GambleTripleApex)
                ImGui.TextColored(new Vector4(1, 0.7f, 0, 1),"警告，你选择赌每120秒三根绝峰箭，可能会导致爆发期灵魂之声能量不足");
            ImGui.Checkbox("是否赌每120秒三根绝峰箭", ref BardSettings.Instance.GambleTripleApex);
            ImGui.Separator();
            if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
                ImGui.TextColored(new Vector4(1, 0.7f, 0, 1),"警告，你开启了全局能力技能不卡GCD，可能导致本ACR产生能力技插入问题，建议关闭");
            ImGui.Checkbox("全局能力技能不卡GCD", ref SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3);
            ImGui.Separator();
            ImGui.Text("大地神自动对齐：");
            if ((BardSettings.Instance.NaturesMinneWithRecitation || BardSettings.Instance.NaturesMinneWithZoe ||
                 BardSettings.Instance.NaturesMinneWithNeutralSect) && !QT.GetQt("大地神"))
                ImGui.TextColored(new Vector4(0.7f, 0.8f, 0.0f, 1.0000f),"如果你希望使用自动大地神，请在QT面板中开启大地神开关");
            ImGui.Checkbox("秘策", ref BardSettings.Instance.NaturesMinneWithRecitation);
            ImGui.SameLine();
            ImGui.Checkbox("活化", ref BardSettings.Instance.NaturesMinneWithZoe);
            ImGui.SameLine();
            ImGui.Checkbox("中间学派", ref BardSettings.Instance.NaturesMinneWithNeutralSect);
            ImGui.Separator();
            if (ImGui.Button("保存设置"))
                BardSettings.Instance.Save();
        }
        ImGui.Separator();
        if (ImGui.CollapsingHeader("   高级设置"))
        {
            ImGui.TextColored(new Vector4(1, 0.7f, 0, 1),"除非你明白你要做什么，不然请别动这几项\n建议仅在受网络延迟与动画锁影响，爆发期打不满或者卡GCD时，再做调整");
            ImGuiHelper.LeftInputInt("非起手的旅神歌在下个GCD前多久使用     (毫秒)", ref BardSettings.Instance.WandererBeforeGcdTime, 500, 2000);
            ImGuiHelper.LeftInputInt("爆发药水的动画持续时间                           (毫秒)", ref BardSettings.Instance.PotionBeforeGcdTime, 500, 2000);
            ImGuiHelper.LeftInputInt("猛者强击在下个GCD前多久使用                (毫秒)", ref BardSettings.Instance.RagingStrikeBeforeGcdTime, 500, 2000);
            ImGuiHelper.LeftInputInt("战斗之声和光明神在下个GCD前多久使用  (毫秒)", ref BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs, 500, 2000);
            ImGuiHelper.LeftInputInt("九天连箭最晚在下个GCD前多久使用         (毫秒)", ref BardSettings.Instance.EmpyrealArrowNotBeforeGcdTime, 400, 2000);
            ImGui.Separator();
            if (ImGui.Button("保存高级设置"))
                BardSettings.Instance.Save();
            ImGui.SameLine();  
            if (ImGui.Button("重置高级设置"))
            {
                BardSettings.Instance.WandererBeforeGcdTime = 600;
                BardSettings.Instance.PotionBeforeGcdTime = 800;
                BardSettings.Instance.RagingStrikeBeforeGcdTime = 600;
                BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs = 1350;
                BardSettings.Instance.EmpyrealArrowNotBeforeGcdTime = 500;
                BardSettings.Instance.Save();
            }
            
        }
        ImGui.Separator(); 
        if (ImGui.CollapsingHeader("   技能队列"))
        {
            ImGui.Separator();
            if (ImGui.Button("清除队列"))
            {
                AI.Instance.BattleData.HighPrioritySlots_OffGCD.Clear();
                AI.Instance.BattleData.HighPrioritySlots_GCD.Clear();
            }
            ImGui.SameLine();
            if (ImGui.Button("清除一个"))
            {
                AI.Instance.BattleData.HighPrioritySlots_OffGCD.Dequeue();
                AI.Instance.BattleData.HighPrioritySlots_GCD.Dequeue();
            }
            if (AI.Instance.BattleData.HighPrioritySlots_GCD.Count > 0)
            {
                foreach (object obj in AI.Instance.BattleData.HighPrioritySlots_GCD)
                    ImGui.Text(" ==" + obj?.ToString());
            }
            if (AI.Instance.BattleData.HighPrioritySlots_OffGCD.Count > 0)
            {
                foreach (object obj in AI.Instance.BattleData.HighPrioritySlots_OffGCD)
                    ImGui.Text(" --" + obj?.ToString());
            }
            ImGui.Separator();
        }
    }

    public void DrawQtDev(JobViewWindow jobViewWindow)
    {
        ImGui.Text("Dev信息");
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