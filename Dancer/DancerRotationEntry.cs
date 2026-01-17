using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using AEAssist.Extension;
using AEAssist.GUI;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Dancer.Ability;
using Wotou.Dancer.Data;
using Wotou.Dancer.GCD;
using Wotou.Dancer.Opener;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Trigger;
using Dalamud.Bindings.ImGui;
using Wotou.Common;
using Wotou.Dancer.Sequence;
using Wotou.Dancer.Triggers;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer;

public class DancerRotationEntry : IRotationEntry
{
    public static JobViewWindow QT { get; private set; }
    
    public static HotkeyWindow? DancePartnerPanel { get; set; }
    
    public static HotkeyWindow? EnAvantPanel { get; set; }
    
    public string AuthorName { get; set; } = "Wotou";
    
    private const string UpdateLog = "";
    
    public void Dispose()
    {
    }

    private List<SlotResolverData> SlotResolvers = new()
    {
        new SlotResolverData(new DancerLastDanceHighGcd(), SlotMode.Gcd), //马上到期的落幕舞
        new SlotResolverData(new DancerTechnicalStepDancingGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerTechnicalStepGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerFinishingMoveGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerStandardStepDancingGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerStandardStepGcd(), SlotMode.Gcd),
        new SlotResolverData(new Dancer1GBeforeTechStepGcd(), SlotMode.Gcd),
       
        new SlotResolverData(new DancerStarfallDanceHighGCD(), SlotMode.Gcd), // 快过期的流星舞
        new SlotResolverData(new DancerProcFountainFailHighGcd(), SlotMode.Gcd), // 快过期的触发
        new SlotResolverData(new DancerProcReverseCascadeHighGcd(), SlotMode.Gcd), // 快过期的触发
        new SlotResolverData(new DancerSaberDanceHighGcd(), SlotMode.Gcd), //团辅期高能量剑舞
        new SlotResolverData(new DancerTillanaGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerProcFountainFallMediumGcd(), SlotMode.Gcd), // 团辅外等不到的坠喷泉触发
        new SlotResolverData(new DancerProcReverseCascadeMediumGcd(), SlotMode.Gcd), // 团辅外等不到的逆瀑泻触发
        new SlotResolverData(new DancerStarfallDanceGCD(), SlotMode.Gcd),
        new SlotResolverData(new DancerSaberDanceMediumGcd(), SlotMode.Gcd), //团辅期低能量剑舞
        new SlotResolverData(new DancerSaberDanceGcd(), SlotMode.Gcd), //普通剑舞 >=70 默认
        new SlotResolverData(new DancerLastDanceGcd(), SlotMode.Gcd),
        
        new SlotResolverData(new DancerProcGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerBaseGcd(), SlotMode.Gcd),
        
        new SlotResolverData(new DancerDevilmentAbility(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFlourishAbility(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFanDance3Ability(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFanDanceAbility(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFanDance4Ability(), SlotMode.OffGcd),
        new SlotResolverData(new DancerCuringWaltzAbility(), SlotMode.OffGcd),
    };
    
    public Rotation Build(string settingFolder)
    {
        DancerDefinesData.InitializeDictionary();
        DancerSettings.Build(settingFolder);
        BuildQt(settingFolder);
        var rot = new Rotation(SlotResolvers)
        {
            TargetJob = Jobs.Dancer,
            AcrType = AcrType.Both,
            MinLevel = 1,
            MaxLevel = 100,
            Description = "舞者ACR" +
                          "\n请在 FuckAnimationLock 插件中勾选减少爆发药后摇" +
                          "\n本ACR所用的是固定4小舞循环，技速推荐2.50，有小舞延后现象的可以改用2.49" +
                          "\n" + UpdateLog,
        };
        rot.SetRotationEventHandler(new DancerRotationEventHandler());
        rot.AddOpener(GetOpener);
        rot.AddTriggerAction(new DancerTriggerActionNewQt());
        rot.AddTriggerAction(new DancerTriggerActionQt());
        rot.AddTriggerAction(new DancerEspritSaveAction());
        rot.AddTriggerAction(new DancerTriggerActionEspritThreshold());
        rot.AddTriggerAction(new DancerFeatherSaveAction());
        rot.AddTriggerAction(new DancerTriggerActionToggleDailyMode());
        rot.AddTriggerAction(new DancerTriggerActionPotionMode());
        rot.AddTriggerCondition(new DancerFeatherCondition());
        rot.AddTriggerCondition(new DancerEspritCondition());
        rot.AddSlotSequences(new TechnicalStandardStepSequence());

        rot.AddCanUseHighPrioritySlotCheck((mode, slot) =>
        {
            if (mode == SlotMode.OffGcd)
            {
                if (slot.Actions.Count == 2 &&
                    slot.Actions.ToArray()[0].Spell.Id == DancerDefinesData.Spells.Improvisation &&
                    GCDHelper.GetGCDCooldown() < 1600)
                    return -1;
                if (slot.Actions.Count > 1 &&
                    slot.Actions.ToArray()[1].Spell.Id == DancerDefinesData.Spells.ImprovisationFinish  &&
                    DancerDefinesData.Spells.Improvisation.RecentlyUsed(1600) &&
                    !DancerDefinesData.Spells.Improvisation.RecentlyUsed(1000) &&
                    !Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.Improvisation)
                    )
                {
                    LogHelper.Print("not use ImprovisationFinish");
                    slot.Actions.RemoveAt(0);
                    return -1;
                }
            }
            return 1;
        });
        
        return rot;
    }

    private static IOpener GetOpener(uint level)
    {
        return new DNCStdOpener100();
    }
    
    public IRotationUI GetRotationUI()
    {
        return DancerRotationEntry.QT;
    }
    
    // 构造函数里初始化QT
    public void BuildQt(string settingFolder)
    {
        // JobViewSave是AE底层提供的QT设置存档类 在你自己的设置里定义即可
        // 第二个参数是你设置文件的Save类 第三个参数是QT窗口标题
        QT = new JobViewWindow(DancerSettings.Instance.JobViewSave, DancerSettings.Instance.Save, "Wotou");
        
        var myJobViewSave = new JobViewSave();
        myJobViewSave.ShowHotkey = DancerSettings.Instance.ShowDancePartnerPanel;
        myJobViewSave.QtHotkeySize = new Vector2(DancerSettings.Instance.DancePartnerPanelIconSize, DancerSettings.Instance.DancePartnerPanelIconSize);
        DancePartnerPanel = new HotkeyWindow(myJobViewSave, "Custom DNC HotkeyWindow");
        
        var enAvantJobViewSave = new JobViewSave();
        enAvantJobViewSave.ShowHotkey = DancerSettings.Instance.ShowEnAvantPanel;
        enAvantJobViewSave.ShowHotkey = false;
        enAvantJobViewSave.QtHotkeySize = new Vector2(DancerSettings.Instance.EnAvantPanelIconSize, DancerSettings.Instance.EnAvantPanelIconSize);
        EnAvantPanel = new HotkeyWindow(enAvantJobViewSave, "Custom DNC En Avant HotkeyWindow");

        // 创建 JobViewWindow 并传递回调
        //var QT2 = new JobViewWindow(DancerSettings.Instance.JobViewSave, DancerSettings.Instance.Save, "Wotou");

        // 为 JobViewWindow 设置 UpdateAction 来渲染 HotkeyWindow2
        QT.SetUpdateAction(() =>
        {   
            UpdateDancerPartnerPanel();
            var viewSave = new JobViewSave();
            viewSave.QtHotkeySize = new Vector2(DancerSettings.Instance.DancePartnerPanelIconSize, DancerSettings.Instance.DancePartnerPanelIconSize);
            viewSave.ShowHotkey = DancerSettings.Instance.ShowDancePartnerPanel;
            viewSave.LockWindow = DancerSettings.Instance.isDancePartnerPanelLocked;
            DancePartnerPanel.DrawHotkeyWindow(new QtStyle(DancerSettings.Instance.JobViewSave));
            DancePartnerPanel = new HotkeyWindow(viewSave, "Custom DNC HotkeyWindow");
            DancePartnerPanel.HotkeyLineCount = 1;
            
            
            EnAvantPanel.AddHotkey("前冲步 - 左上", new EnAvantHotkeyResolver(225f * MathF.PI / 180f)); // ⬉ 西北 (45° → 225°)
            EnAvantPanel.AddHotkey("前冲步 - 上", new EnAvantHotkeyResolver(180f * MathF.PI / 180f));  // ⬆ 北 (0° → 180°)
            EnAvantPanel.AddHotkey("前冲步 - 右上", new EnAvantHotkeyResolver(135f * MathF.PI / 180f)); // ⬈ 东北 (315° → 135°)

            EnAvantPanel.AddHotkey("前冲步 - 左", new EnAvantHotkeyResolver(270f * MathF.PI / 180f));  // ⬅ 西 (90° → 270°)
            EnAvantPanel.AddHotkey("前冲步 - 人物面向", new MyNormalSpellHotKeyResolver(DancerDefinesData.Spells.EnAvant, SpellTargetType.Self));
            EnAvantPanel.AddHotkey("前冲步 - 右", new EnAvantHotkeyResolver(90f * MathF.PI / 180f));   // ➡ 东 (270° → 90°)

            EnAvantPanel.AddHotkey("前冲步 - 左下", new EnAvantHotkeyResolver(315f * MathF.PI / 180f)); // ⬋ 西南 (135° → 315°)
            EnAvantPanel.AddHotkey("前冲步 - 下", new EnAvantHotkeyResolver(0f * MathF.PI / 180f));    // ⬇ 南 (180° → 0°)
            EnAvantPanel.AddHotkey("前冲步 - 右下", new EnAvantHotkeyResolver(45f * MathF.PI / 180f));  // ⬊ 东南 (225° → 45°)

            
            var enAvantViewSave = new JobViewSave();
            enAvantViewSave.QtHotkeySize = new Vector2(DancerSettings.Instance.EnAvantPanelIconSize, DancerSettings.Instance.EnAvantPanelIconSize);
            enAvantViewSave.ShowHotkey = DancerSettings.Instance.ShowEnAvantPanel;
            enAvantViewSave.ShowHotkey = false;
            enAvantViewSave.LockWindow = DancerSettings.Instance.isEnAvantPanelLocked;
            EnAvantPanel.DrawHotkeyWindow(new QtStyle(DancerSettings.Instance.JobViewSave));
            EnAvantPanel = new HotkeyWindow(enAvantViewSave, "Custom DNC En Avant HotkeyWindow");
            EnAvantPanel.HotkeyLineCount = 3;
            
            if (!DancerSettings.Instance.IsReadInfoWindow08)
                InfoWindow.Draw();
            if (DancerSettings.Instance.IsOpenCommandWindow)
                DancerCommandWindow.Draw();
        });
        
        QT.AddTab("通用", DrawGeneral);
        QT.AddTab("Dev", DrawQtDev);
        QT.AddTab("Qt默认值", DrawQtDefaults);
        
        // 初始化 QT 选项
        foreach (var def in DancerQtHotkeyRegistry.Qts)
        {
            bool initialValue = DancerSettings.Instance.UserDefinedQtValues.TryGetValue(def.Key, out var qtValue)
                ? qtValue
                : def.Default;

            if (def.Callback != null) QT.AddQt(def.Key, initialValue, def.Callback);
            else QT.AddQt(def.Key, initialValue, def.Description);

            QT.SetQtToolTip(def.Description);
        }
        
        foreach (var hk in DancerQtHotkeyRegistry.Hotkeys) QT.AddHotkey(hk.LabelZh, hk.Factory());
    }
    
    public static void UpdateDancerPartnerPanel()
    {
        PartyHelper.UpdateAllies();
        for (var i = 1; i < PartyHelper.Party.Count; i++)
        {
            var index = i;
            DancePartnerPanel?.AddHotkey("[" + i  +"] 切换舞伴: " + PartyHelper.Party[i].Name , new ClosedPositionHotkeyResolver(index));
        }
    }
    
    private void DrawModeButton(string label, bool isDailyMode)
    {
        // 保存初始状态
        bool initialIsDailyMode = DancerSettings.Instance.IsDailyMode;
        bool isSelected = (initialIsDailyMode == isDailyMode);

        // 根据初始状态决定是否 PushStyleColor
        if (isSelected)
            ImGui.PushStyleColor(ImGuiCol.Button, DancerSettings.Instance.JobViewSave.MainColor);

        // 绘制按钮，并记录是否被点击
        bool buttonClicked = ImGui.Button(label);

        // 根据初始状态决定是否 PopStyleColor
        if (isSelected)
            ImGui.PopStyleColor();

        // 在 Push/Pop 操作之后，处理按钮点击事件
        if (buttonClicked)
            DancerSettings.Instance.IsDailyMode = isDailyMode;
    }
    
    public void DrawGeneral(JobViewWindow jobViewWindow)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1f,0.36f,0.54f, 1));
        if (ImGui.CollapsingHeader("   重要说明"))
        {
            ImGui.Text("舞者ACR - 技速推荐2.50\n本ACR所用的是固定4小舞循环，技速推荐2.50\n如小舞有延后，可用2.49技速，或开启Gcd偏移优化\n请在 FuckAnimationLock 插件中勾选减少爆发药后摇");
            ImGui.Text("插件地址:");
            ImGui.SameLine();
            var hyperlink = new Hyperlink("FuckAnimationLock", "https://github.com/NiGuangOwO/DalamudPlugins");
            hyperlink.Render();
            ImGui.Separator();
            if (UpdateLog != "")
            {
                ImGui.Text(UpdateLog);
                ImGui.Separator();
            }
            if (ImGui.Button("查看指令"))
            {
                DancerSettings.Instance.IsOpenCommandWindow = true;
            }
            ImGui.SameLine();
            if (ImGui.Button("反馈问题"))
            {
                string url = "https://discord.com/channels/1191648233454313482/1296269368224911461";  
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true  // 在默认浏览器中打开
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"打开浏览器失败：{ex.Message}");
                }
            }
        }

        ImGui.Separator();
        if (ImGui.CollapsingHeader("   基础设置"))
        {
            ImGui.Text("当前模式：" + (DancerSettings.Instance.IsDailyMode ? "日随模式" : "高难模式"));
            DrawModeButton("日随模式", true);
            ImGui.SameLine();
            DrawModeButton("高难模式", false);
            if (DancerSettings.Instance.IsDailyMode)
            {
                ImGui.Separator();
                ImGui.Checkbox("自动舞步", ref DancerSettings.Instance.EnableAutoDancing);
                ImGui.SameLine();
                ImGui.Checkbox("自动速行", ref DancerSettings.Instance.EnableAutoPeloton);
                ImGui.SameLine();
                ImGui.Checkbox("自动舞伴", ref DancerSettings.Instance.EnableAutoDancePartner);
            }
            else
            {
                ImGui.Separator();
                ImGui.Checkbox("自动舞伴（FA专用）", ref DancerSettings.Instance.EnableAutoDancePartnerInFullAutoMode);
            }
            
            ImGui.Separator();
            UiHelper.RightInputInt("倒计时提前使用小舞", ref DancerSettings.Instance.OpenerStandardStepTime, 6500, 15000, "(毫秒)");
            ImGui.Separator();
            UiHelper.RightInputInt("倒计时提前使用起手", ref DancerSettings.Instance.OpenerTime, 0, 1000, "(毫秒)");
            ImGui.Separator();
            UiHelper.RightInputInt("非爆发期使用剑舞", ref DancerSettings.Instance.SaberDanceEspritThreshold, 50, 100,"大于等于", 5);
            ImGui.Separator();
            UiHelper.RightInputInt("爆发期使用提拉纳", ref DancerSettings.Instance.TillanaEspritThreshold, 0, 50,"小于等于", 5);
            ImGui.Separator();
            UiHelper.RightInputInt("爆发最后1G使用提拉纳", ref DancerSettings.Instance.TillanaLastGcdEspritThreshold, 0, 50,"小于等于", 5);
            ImGui.Separator();
            
            ImGui.Text("爆发药设置：" + (DancerSettings.Instance.UsePotionInOpener ? "起手吃" : "2分钟爆发吃"));
            if (!QT.GetQt("爆发药"))
                ImGui.TextColored(new Vector4(0.7f, 0.8f, 0.0f, 1.0000f), "如果你希望使用爆发药，请在QT面板中开启爆发药开关");
            ImGui.Checkbox("起手吃爆发药", ref DancerSettings.Instance.UsePotionInOpener);
            ImGui.Separator();
            ImGui.BeginGroup(); // 开始一个整体分组
            ImGui.Checkbox("##DanceDistanceWarning", ref DancerSettings.Instance.DanceDistanceWarning);
            ImGui.SameLine();
            ImGui.Text("是否播放大小舞距离提醒 - 依赖插件");
            ImGui.SameLine();
            var dailyRoutinesLink = new Hyperlink("Daily Routines", "https://github.com/AtmoOmen/DalamudPlugins");
            dailyRoutinesLink.Render();
            ImGui.EndGroup(); // 结束分组

            // 检查鼠标是否悬停在整体分组上
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("当大舞、小舞或结束动作CD剩余5秒且距离超过有效范围时，会播放语音提醒\n请确保安装并正确配置Daily Routines插件以启用此功能");
            }  
            ImGui.Separator();
            ImGui.Checkbox("##WelcomeVoice", ref DancerSettings.Instance.WelcomeVoice);
            ImGui.SameLine();
            ImGui.Text("是否播放欢迎语音 - 依赖插件");
            ImGui.SameLine();
            dailyRoutinesLink.Render();      
            ImGui.Separator();
            /*ImGui.BeginGroup();
            ImGuiHelper.LeftInputInt("小舞冷却时间容差值  (毫秒)", ref DancerSettings.Instance.StandardStepCdTolerance, 0, 1000);
            ImGui.EndGroup();
            if (ImGui.IsItemHovered())
            {
                // 显示 Tooltip
                ImGui.SetTooltip("仅当你技速小于2.50时，为了避免小舞延后才需做调整");
            }
            ImGui.Separator();*/
            if (ImGui.Button("保存设置")) DancerSettings.Instance.Save();
        }
        ImGui.Separator();
        if (ImGui.CollapsingHeader("   界面设置"))
        {
            ImGui.Checkbox("显示快速舞伴切换面板", ref DancerSettings.Instance.ShowDancePartnerPanel);
            ImGuiHelper.LeftInputInt("图标大小", ref DancerSettings.Instance.DancePartnerPanelIconSize, 10, 80);
            ImGui.Checkbox("锁定舞伴面板", ref DancerSettings.Instance.isDancePartnerPanelLocked);
            ImGui.Separator();
            /*ImGui.Checkbox("显示前冲步面板 - 镜头面向控制前冲步", ref DancerSettings.Instance.ShowEnAvantPanel);
            ImGuiHelper.LeftInputInt("图标大小", ref DancerSettings.Instance.EnAvantPanelIconSize, 10, 80);
            ImGui.Checkbox("锁定前冲步面板", ref DancerSettings.Instance.isEnAvantPanelLocked);
            ImGui.Separator();*/
            ImGui.Checkbox("是否启用舞伴宏", ref DancerSettings.Instance.UseDancePartnerMacro);
            ImGui.InputTextMultiline("", ref DancerSettings.Instance.DancePartnerMacroText, 1000, new Vector2(-1, ImGui.GetTextLineHeight() * 6));
            ImGui.Separator();
            if (ImGui.Button("保存界面设置")) DancerSettings.Instance.Save();
        }
        
        ImGui.Separator();
        if (ImGui.CollapsingHeader("   时间轴更新"))
        {
            ImGui.Separator();
            if (TimeLineUpdater.jsonData == null)
            {
                ImGui.TextColored(new Vector4(1, 0.7f, 0, 1), "时间轴数据未加载，请检查 Github 是否能正常访问");
            }
            else
            {
                foreach (var timeline in TimeLineUpdater.jsonData)
                {
                    bool selected = DancerSettings.Instance.SelectedTimeLinesForUpdate.TryGetValue(timeline.Name, out bool isSelected) && isSelected;
                    if (ImGui.Checkbox(timeline.Name, ref selected))
                    {
                        DancerSettings.Instance.SelectedTimeLinesForUpdate[timeline.Name] = selected;
                        DancerSettings.Instance.Save();
                    }
                    ImGui.Separator();
                }
                if (ImGui.Button("保存时间轴设置"))
                    DancerSettings.Instance.Save();
            }
            ImGui.Separator();
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
                foreach (object obj in AI.Instance.BattleData.HighPrioritySlots_GCD)
                    ImGui.Text(" ==" + obj);
            if (AI.Instance.BattleData.HighPrioritySlots_OffGCD.Count > 0)
                foreach (object obj in AI.Instance.BattleData.HighPrioritySlots_OffGCD)
                    ImGui.Text(" --" + obj);
            ImGui.Separator();
        }
        ImGui.PopStyleColor(2);
    }
    
    public void DrawQtDefaults(JobViewWindow jobViewWindow)
    {
        ImGui.Text("在这里设置 Qt 的默认值：");

        foreach (var def in DancerQtHotkeyRegistry.Qts) // [CHANGED]
        {
            bool currentValue = DancerSettings.Instance.UserDefinedQtValues.TryGetValue(def.Key, out var qtValue)
                ? qtValue
                : def.Default;

            if (ImGui.Checkbox($"{def.Key}##{def.Key}", ref currentValue))
            {
                DancerSettings.Instance.UserDefinedQtValues[def.Key] = currentValue;
                DancerSettings.Instance.Save();
            }
        }

        ImGui.Separator();

        if (ImGui.Button("重置##Reset"))
        {
            DancerSettings.Instance.UserDefinedQtValues.Clear(); // 清空用户定义值
            DancerSettings.Instance.Save();
        }
        ImGui.SameLine();
        if (ImGui.Button("保存##Save"))
        {
            DancerSettings.Instance.Save();
        }
    }

    public void OnDrawSetting(){
        DancerSettingsUI.Instance.Draw();
    }
    
    private string searchQuery = "";
    private Dictionary<string, uint> matchingSkills = new Dictionary<string, uint>();
    
    public void DrawQtDev(JobViewWindow jobViewWindow)
    {
        Vector3 vector3 = Core.Resolve<MemApiMove>().MousePos();
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 3);
        interpolatedStringHandler.AppendLiteral("鼠标坐标: (");
        interpolatedStringHandler.AppendFormatted<float>(vector3.X);
        interpolatedStringHandler.AppendLiteral(", ");
        interpolatedStringHandler.AppendFormatted<float>(vector3.Y);
        interpolatedStringHandler.AppendLiteral(", ");
        interpolatedStringHandler.AppendFormatted<float>(vector3.Z);
        interpolatedStringHandler.AppendLiteral(")");
        ImGui.Text(interpolatedStringHandler.ToStringAndClear());
        ImGui.Separator();
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1f,0.36f,0.54f, 1));
        ImGui.Text("神秘代码：");
        if (ImGui.InputText("##UnlockPassword", ref DancerSettings.Instance.UnlockPassword, 100))
        {
            DancerSettings.Instance.Save();
        }
        ImGui.Separator();
        ImGui.Text("请输入技能名，以搜索对应id：");
        if (ImGui.InputText("##SearchQuery", ref searchQuery, 100))
        {
            // 在输入框变化时实时更新匹配项
            matchingSkills = DancerDefinesData.GetMatchingSkills(searchQuery);
        }

        // 显示匹配结果
        if (matchingSkills.Count > 0)
        {
            ImGui.Text("匹配的技能：");
            int index = 0; // 用于区分每个按钮
            foreach (var skill in matchingSkills)
            {
                if (ImGui.Button($"复制ID##{index}"))
                {
                    ImGui.SetClipboardText(skill.Value.ToString()); // 将ID复制到剪贴板
                }
                ImGui.SameLine();
                ImGui.Text($"{skill.Key} - ID: {skill.Value}");
                index++;
            }
        }
        else if (!string.IsNullOrEmpty(searchQuery))
        {
            ImGui.Text("未找到匹配的技能。");
        }
        ImGui.Separator();
        ImGui.PopStyleColor(2);
    }
}
