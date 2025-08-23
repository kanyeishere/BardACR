using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Interface;
using ImGuiNET;
using Wotou.Bard.Data;
using Wotou.Bard.Opener;
using Wotou.Bard.Sequence;
using Wotou.Bard.Setting;
using Wotou.Bard.SlotResolvers;
using Wotou.Bard.Triggers;
using Wotou.Bard.Utility;
using Wotou.Common;

namespace Wotou.Bard;

// 重要 类一定要Public声明才会被查找到
public class BardRotationEntry : IRotationEntry
{
    public static JobViewWindow QT { get; private set; }
    
    public static HotkeyWindow? WardensPaeanPanel { get; set; }

    public string AuthorName { get; set; } = "Wotou";
    
    private const string UpdateLog = "更新日志 0104" +
                                     "\n- 添加70-80级起手" +
                                     "\n- 添加70-80级高难特化循环";
    
    public Rotation Build(string settingFolder)
    {
        BardDefinesData.InitializeDictionary();
        BardSettings.Build(settingFolder);
        BuildQT();
        var rot = new Rotation(BardSlotResolverConfig.SlotResolvers)
        {
            TargetJob = Jobs.Bard,
            AcrType = AcrType.Both,
            MinLevel = 1,
            MaxLevel = 100,
            Description = "诗人ACR\n" + UpdateLog,
                          
        };
        // 添加死后爆发
        rot.AddSlotSequences(new BurstingAfterDeathSequence());
        rot.AddSlotSequences(new EmpyrealAfterDeathSequence());
        // 添加各种事件回调
        rot.SetRotationEventHandler(new BardRotationEventHandler());
        rot.AddOpener(GetOpener);

        // 添加时间轴行为
        rot.AddTriggerAction(new BardTriggerActionQt());
        rot.AddTriggerAction(new BardTriggerActionNewQt());
        rot.AddTriggerAction(new BardSongDurationAction());
        rot.AddTriggerAction(new BardSongOrderAction());
        rot.AddTriggerAction(new BardHeartBreakSaveAction());
        rot.AddTriggerAction(new BardDotBlacklistAction());
        rot.AddTriggerAction(new BardPitchPerfectMinCountAction());
        rot.AddTriggerAction(new BardTriggerActionToggleDailyMode());
        rot.AddTriggerAction(new BardTriggerActionResetSongOrder());
        rot.AddTriggerAction(new BardTriggerActionPotionMode());

        // 添加时间轴控制
        rot.AddTriggerCondition(new BardSongTimerCondition());
        rot.AddTriggerCondition(new BardActiveSongCondition());
        rot.AddTriggerCondition(new BardSoulVoiceCondition());
        rot.AddTriggerCondition(new BardRepertoireCondition());
        rot.AddTriggerCondition(new TargetInRangeCondition());
        rot.AddTriggerCondition(new TargetNotInRangeCondition());

        return rot;
    }

    // 如果你不想用QT 可以自行创建一个实现IRotationUI接口的类
    public IRotationUI GetRotationUI()
    {
        return QT;
    }

    // 设置界面
    public void OnDrawSetting()
    {
        BardSettingUI.Instance.Draw();
    }


    public void Dispose()
    {
        // 释放需要释放的东西 没有就留空
    }

    private IOpener? GetOpener(uint level)
    {
        switch (BardSettings.Instance.Opener)
        {
            case 0:
                return new Bard3GOpener100();
            case 1:
                return new Bard2GOpener100();
            case 2:
                return new BardFROpener100();
            case 3:
                return new Bard3GOpener7080();
            case 4:
                return new Bard5GOpener70();
        }
        return new Bard3GOpener100();
    }

    public static void OnClickStrongAlign(bool value)
    {
        if (!BardUtil.IsSongOrderNormal())
        {
            QT.SetQt("强对齐", false);
            LogHelper.Print("特殊歌轴顺序不支持强对齐");
            return;
        }
        if (value)
        {
            QT.SetQt("爆发", true);
            QT.SetQt("对齐旅神", true);
            LogHelper.Print("你已启用强对齐，强制开启爆发");
            LogHelper.Print("你已启用强对齐，强制开启对齐旅神");
        }
    }

    public static void OnClickBurstQT()
    {
        if (QT.GetQt("强对齐"))
        {
            QT.SetQt("爆发", true);
            LogHelper.Print("你已启用强对齐，强制开启爆发");
        }
    }

    public static void OnClickBurstWithWandererQT()
    {
        if (!BardUtil.IsSongOrderNormal())
        {
            QT.SetQt("对齐旅神", false);
            LogHelper.Print("特殊歌轴顺序不支持对齐旅神");
            return;
        }
        if (QT.GetQt("强对齐"))
        {
            QT.SetQt("对齐旅神", true);
            LogHelper.Print("你已启用强对齐，强制开启对齐旅神");
        }
    }

    // 构造函数里初始化QT
    public void BuildQT()
    {
        // JobViewSave是AE底层提供的QT设置存档类 在你自己的设置里定义即可
        // 第二个参数是你设置文件的Save类 第三个参数是QT窗口标题
        QT = new JobViewWindow(BardSettings.Instance.JobViewSave, BardSettings.Instance.Save, "Wotou Bard 诗人");
        
        var myJobViewSave = new JobViewSave();
        myJobViewSave.ShowHotkey = BardSettings.Instance.ShowWardensPaeanPanel;
        myJobViewSave.QtHotkeySize = new Vector2(BardSettings.Instance.WardensPaeanPanelIconSize, BardSettings.Instance.WardensPaeanPanelIconSize);
        WardensPaeanPanel = new HotkeyWindow(myJobViewSave, "WardensPaeanPanel");
        
        QT.SetUpdateAction(OnUIUpdate); // 设置QT中的Update回调 不需要就不设置

        //添加QT分页 第一个参数是分页标题 第二个是分页里的内容
        QT.AddTab("通用", DrawQtGeneral);
        QT.AddTab("Dev", DrawQtDev);
        QT.AddTab("Qt默认值", DrawQtDefaults);

        // 添加QT开关 第二个参数是默认值 (开or关) 第三个参数是鼠标悬浮时的tips
        // 初始化 QT 选项
        foreach (var def in BardQtHotkeyRegistry.Qts)
        {
            var initial = BardSettings.Instance.UserDefinedQtValues.TryGetValue(def.Key, out var v) ? v : def.Default;
            if (def.Callback != null) QT.AddQt(def.Key, initial, def.Callback);
            else QT.AddQt(def.Key, initial, def.Description);
            QT.SetQtToolTip(def.Description);
        }

        if(BardSettings.Instance.JobViewSave.QtUnVisibleList.Count == 0 )
        {
            BardSettings.Instance.JobViewSave.QtUnVisibleList.Add("Debug");
            BardSettings.Instance.JobViewSave.QtUnVisibleList.Add(QTKey.EmpyrealArrow);
            BardSettings.Instance.JobViewSave.QtUnVisibleList.Add(QTKey.Sidewinder);
            BardSettings.Instance.JobViewSave.QtUnVisibleList.Add(QTKey.EmpyrealArrowBeforeGcd);
        }

        // 添加快捷按钮 (带技能图标)
        foreach (var hk in BardQtHotkeyRegistry.Hotkeys)
        {
            // UI 显示中文名；如果你做多语言切换，这里可按语言显示 zh/en
            QT.AddHotkey(hk.LabelZh, hk.Factory());
        }
    }

    public void OnUIUpdate()
    {
        UpdateWardensPaeanPanel();
        var myJobViewSave = new JobViewSave();
        myJobViewSave.ShowHotkey = BardSettings.Instance.ShowWardensPaeanPanel;
        myJobViewSave.QtHotkeySize = new Vector2(BardSettings.Instance.WardensPaeanPanelIconSize, BardSettings.Instance.WardensPaeanPanelIconSize);
        myJobViewSave.LockWindow = BardSettings.Instance.IsWardensPanelLocked;
        WardensPaeanPanel?.DrawHotkeyWindow(new QtStyle(BardSettings.Instance.JobViewSave));
        WardensPaeanPanel = new HotkeyWindow(myJobViewSave, "WardensPaeanPanel");
        WardensPaeanPanel.HotkeyLineCount = 1;
        if (!BardSettings.Instance.IsReadInfoWindow08)
            InfoWindow.Draw();
        if (BardSettings.Instance.IsOpenCommandWindow)
            BardCommandWindow.Draw();
    }
    
    public static void UpdateWardensPaeanPanel()
    {
        PartyHelper.UpdateAllies();
        if (PartyHelper.Party.Count <= 1) return;
        for (var i = 0; i < PartyHelper.Party.Count; i++)
        {
            var index = i;
            WardensPaeanPanel?.AddHotkey("[" + i  +"]净化: " + PartyHelper.Party[i].Name, new WardensPaeanHotkeyResolver(index));
        }
    }
    
    private void DrawModeButton(string label, bool isDailyMode, Action onClickAction)
    {
        // 保存初始状态
        bool initialIsDailyMode = BardSettings.Instance.IsDailyMode;
        bool isSelected = (initialIsDailyMode == isDailyMode);

        // 根据初始状态决定是否 PushStyleColor
        if (isSelected) 
            ImGui.PushStyleColor(ImGuiCol.Button, BardSettings.Instance.JobViewSave.MainColor);

        // 绘制按钮，并记录是否被点击
        bool buttonClicked = ImGui.Button(label);

        // 根据初始状态决定是否 PopStyleColor
        if (isSelected)
            ImGui.PopStyleColor();

        // 在 Push/Pop 操作之后，处理按钮点击事件
        if (buttonClicked)
            onClickAction?.Invoke();
    }
    
    public void DrawQtDefaults(JobViewWindow jobViewWindow)
    {
        ImGui.Text("在这里设置 Qt 的默认值：");

        foreach (var def in BardQtHotkeyRegistry.Qts)
        {
            var currentValue = BardSettings.Instance.UserDefinedQtValues.TryGetValue(def.Key, out var qtValue)
                ? qtValue
                : def.Default;

            // 左侧显示中文键名，右侧是开关
            ImGui.PushID(def.Key);
            var clicked = ImGui.Checkbox($"{def.Key}##{def.Key}", ref currentValue);

            // 提示
            if (ImGui.IsItemHovered() && !string.IsNullOrWhiteSpace(def.Description))
                ImGui.SetTooltip(def.Description);

            if (clicked)
            {
                BardSettings.Instance.UserDefinedQtValues[def.Key] = currentValue;
                BardSettings.Instance.Save();
            }
        }

        ImGui.Separator();

        if (ImGui.Button("重置##Reset"))
        {
            BardSettings.Instance.UserDefinedQtValues.Clear(); // 清空用户定义值
            BardSettings.Instance.Save();
        }
        ImGui.SameLine();
        if (ImGui.Button("保存##Save"))
        {
            BardSettings.Instance.Save();
        }
    }

    public void DrawQtGeneral(JobViewWindow jobViewWindow)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.4314f, 0.6667f, 0.5569f, 1));
        
        if (ImGui.CollapsingHeader("   重要说明"))
        {
            ImGui.Text("诗人ACR\n适配技速2.48-2.50\n请在 FuckAnimationLock 中开启三插");
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
                BardSettings.Instance.IsOpenCommandWindow = true;
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
            ImGui.Text("当前模式：" + (BardSettings.Instance.IsDailyMode ? "日随模式" : "高难模式"));
            DrawModeButton("日随模式", true, () =>
            {
                // 日随模式的相关设置
                BardSettings.Instance.IsDailyMode = true;
                QT.SetQt("强对齐", false);
                QT.SetQt("对齐旅神", false);
                QT.SetQt("攒碎心箭", false);
                QT.SetQt(QTKey.SmartAoeTarget, true);
            });
            ImGui.SameLine();
            DrawModeButton("高难模式", false, () =>
            {
                // 高难模式的相关设置
                BardSettings.Instance.IsDailyMode = false;
                QT.SetQt("强对齐", true);
                QT.SetQt("对齐旅神", true);
                QT.SetQt("攒碎心箭", true);
                QT.SetQt(QTKey.SmartAoeTarget, false);
            });
            if (BardSettings.Instance.IsDailyMode)
            {
                ImGui.Separator();
                ImGui.Checkbox("对小怪用DOT", ref BardSettings.Instance.ApplyDotOnTrashMobs);
                ImGui.SameLine();
                ImGui.Checkbox("自动使用速行", ref BardSettings.Instance.EnableAutoPeloton);
            }
            ImGui.Separator();
            ImGui.Text("当前歌轴时长及顺序：");
            if (BardSettings.Instance.WandererSongDuration + BardSettings.Instance.MageSongDuration +
                BardSettings.Instance.ArmySongDuration < 120)
                ImGui.TextColored(new Vector4(1, 0.7f, 0, 1), "警告，你设置的三首歌累计时长小于120秒，建议设置略长于120秒");

            var moveFrom = -1; // 记录拖拽的起始位置
            var moveTo = -1; // 记录拖拽的目标位置
            var songSettings = BardSongSettingsManager.Instance.SongSettings;
            unsafe
            {
                for (var i = 0; i < songSettings.Count; i++)
                {
                    var setting = songSettings[i];

                    // 同步 Value，与 BardSettings.Instance 保持一致
                    switch (setting.Song)
                    {
                        case Song.Wanderer:
                            setting.Value = BardSettings.Instance.WandererSongDuration;
                            break;
                        case Song.Mage:
                            setting.Value = BardSettings.Instance.MageSongDuration;
                            break;
                        case Song.Army:
                            setting.Value = BardSettings.Instance.ArmySongDuration;
                            break;
                    }
                    // 开始一个组，将 Label 和 Input Box 一起处理
                    ImGui.BeginGroup();
                    // 获取当前光标位置
                    Vector2 cursorPos = ImGui.GetCursorScreenPos();
                    // 定义方块的大小
                    float squareSize = ImGui.GetFontSize() - 9; // 使用字体大小作为方块大小，确保与文本高度一致

                    // 获取文本的高度
                    float lineHeight = ImGui.GetTextLineHeight();

                    // 计算方块的 Y 坐标，使其与文本垂直居中
                    float framePaddingY = ImGui.GetStyle().FramePadding.Y;
                    float squareY = cursorPos.Y + framePaddingY + (lineHeight - squareSize) / 2.0f;
                    // 根据歌曲类型设置颜色
                    uint color;
                    switch (setting.Song)
                    {
                        case Song.Wanderer: // 绿色
                            color = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f,0.9f,0.35f, 1.0f));
                            break;
                        case Song.Army: // 黄色
                            color = ImGui.ColorConvertFloat4ToU32(new Vector4(0.88f, 0.55f, 0.03f, 1.0f));
                            break;
                        case Song.Mage: // 蓝色
                            color = ImGui.ColorConvertFloat4ToU32(new Vector4(0.31f, 0.33f, 0.89f, 1.0f));
                            break;
                        default: // 默认白色
                            color = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                            break;
                    }

                    // 获取绘制列表
                    var drawList = ImGui.GetWindowDrawList();

                    // 绘制带圆角的方块
                    float rounding = squareSize / 2.0f; // 设置圆角半径为方块大小的四分之一
                    Vector2 squareMin = new Vector2(cursorPos.X, squareY);
                    Vector2 squareMax = new Vector2(cursorPos.X + squareSize, squareY + squareSize);
                    drawList.AddRectFilled(squareMin, squareMax, color, rounding);

                    // 移动光标位置，以避免方块和文本重叠
                    ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + squareSize + 5, cursorPos.Y));

                    ImGui.Text(setting.Label); // Label
                    ImGui.SameLine();
                    ImGui.PushID(i); // 使用索引作为 ID
                    float globalFontSize = UiBuilder.DefaultFontSizePx;   // Get the global font size in pixels
                    float globalFontScale = ImGui.GetIO().FontGlobalScale;
                    float widthForNextItem = globalFontSize * globalFontScale * 6 + 20;  
                    ImGui.SetNextItemWidth(widthForNextItem);

                    if (ImGui.InputFloat("", ref setting.Value, 0.1f))
                    {
                        setting.Value = Math.Clamp(setting.Value, setting.Min, setting.Max);
                        // 更新 BardSettings.Instance 中的值
                        switch (setting.Song)
                        {
                            case Song.Wanderer:
                                BardSettings.Instance.WandererSongDuration = setting.Value;
                                break;
                            case Song.Mage:
                                BardSettings.Instance.MageSongDuration = setting.Value;
                                break;
                            case Song.Army:
                                BardSettings.Instance.ArmySongDuration = setting.Value;
                                break;
                        }
                    }
                    ImGui.SameLine();
                    ImGui.Button("\u2195");
                    ImGui.SameLine();
                    ImGui.Text("\u2190 拖动调整顺序");
                    ImGui.SameLine();
                    ImGui.Dummy(new Vector2(100f, 0.0f));
                    ImGui.PopID();
                    ImGui.EndGroup();
                    // 处理拖拽源
                    // ImGui.PushStyleColor(ImGuiCol.DragDropTarget, new Vector4(0f, 0.3012f, 0.2306f, 1f)); 
                    if (ImGui.BeginDragDropSource())
                    {
                        moveFrom = i; // 记录当前项的索引
                        ImGui.SetDragDropPayload("DND_SONG_SETTING", new IntPtr(&moveFrom),
                            sizeof(int)); // 传递 moveFrom 索引
                        ImGui.SetTooltip($"调整顺序: {setting.Label.Substring(0, 3)}");
                        ImGui.EndDragDropSource();
                    }

                    // 处理拖拽目标
                    if (ImGui.BeginDragDropTarget())
                    {
                        // var payload = ImGui.AcceptDragDropPayload("DND_SONG_SETTING");
                        var payload = ImGui.AcceptDragDropPayload("DND_SONG_SETTING",
                            ImGuiDragDropFlags.AcceptNoDrawDefaultRect);

                        if (payload.NativePtr != (void*)0)
                        {
                            // 从 payload 中获取 moveFrom 的值
                            moveFrom = *(int*)payload.Data;
                            moveTo = i; // 记录目标位置

                            // 交换逻辑
                            if (moveFrom != -1 && moveTo != -1 && moveFrom != moveTo)
                            {
                                var draggedSetting = songSettings[moveFrom];
                                songSettings.RemoveAt(moveFrom);    
                                songSettings.Insert(moveTo, draggedSetting);

                                // 更新 BardSettings.Instance 的歌曲顺序
                                BardSettings.Instance.FirstSong = songSettings[0].Song;
                                BardSettings.Instance.SecondSong = songSettings[1].Song;
                                BardSettings.Instance.ThirdSong = songSettings[2].Song;
                                // 重置
                                moveFrom = -1;
                                moveTo = -1;

                                if (!BardUtil.IsSongOrderNormal())
                                {
                                    LogHelper.Print("特殊歌轴顺序，将禁用强对齐和对齐旅神");
                                    QT.SetQt("强对齐", false);
                                    QT.SetQt("对齐旅神", false);
                                    if (QT.GetQt("Debug"))
                                    {
                                        LogHelper.Print("第一首：" + BardSettings.Instance.FirstSong);
                                        LogHelper.Print("第二首：" + BardSettings.Instance.SecondSong);
                                        LogHelper.Print("第三首：" + BardSettings.Instance.ThirdSong);
                                    }
                                } 
                                else
                                {
                                    LogHelper.Print("正常歌轴顺序，可以启用强对齐和对齐旅神");
                                }
                            }
                        }
                        ImGui.EndDragDropTarget();
                    }
                }
            }


            var songTimelineSelectionIndex = 0;
            var songTimelineSelection = "";
            if (BardSettings.Instance.WandererSongDuration == 42.6f &&
                BardSettings.Instance.MageSongDuration == 39f &&
                BardSettings.Instance.ArmySongDuration == 39.2f &&
                BardUtil.IsSongOrderNormal())
                songTimelineSelectionIndex = 1;
            if (BardSettings.Instance.WandererSongDuration == 42.6f &&
                BardSettings.Instance.MageSongDuration == 42f &&
                BardSettings.Instance.ArmySongDuration == 36.2f &&
                BardUtil.IsSongOrderNormal())
                songTimelineSelectionIndex = 2;
            if (BardSettings.Instance.WandererSongDuration == 42.6f &&
                BardSettings.Instance.MageSongDuration == 33f &&
                BardSettings.Instance.ArmySongDuration == 45f &&
                BardUtil.IsSongOrderNormal())
                songTimelineSelectionIndex = 3;
            if (BardSettings.Instance.WandererSongDuration == 42.6f &&
                BardSettings.Instance.MageSongDuration == 42f &&
                BardSettings.Instance.ArmySongDuration == 45f &&
                BardUtil.IsSongOrderNormal())
                songTimelineSelectionIndex = 4;
            if (BardSettings.Instance.WandererSongDuration == 45f &&
                BardSettings.Instance.MageSongDuration == 45f &&
                BardSettings.Instance.ArmySongDuration == 45f &&
                BardUtil.IsSongOrderNormal())
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
                if (ImGui.Selectable("请选择"))
                {
                }

                if (ImGui.Selectable("43-40-37 建议2.49-2.50技速"))
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 39f;
                    BardSettings.Instance.ArmySongDuration = 39.2f;
                    BardSongSettingsManager.Instance.ResetOrder();
                }

                if (ImGui.Selectable("43-43-34 建议2.48技速"))
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 42f;
                    BardSettings.Instance.ArmySongDuration = 36.2f;
                    BardSongSettingsManager.Instance.ResetOrder();
                }

                if (ImGui.Selectable("43-34-43 90级歌轴"))
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 33f;
                    BardSettings.Instance.ArmySongDuration = 45f;
                    BardSongSettingsManager.Instance.ResetOrder();
                }

                if (ImGui.Selectable("43-43-45 特化歌轴"))
                {
                    BardSettings.Instance.WandererSongDuration = 42.6f;
                    BardSettings.Instance.MageSongDuration = 42f;
                    BardSettings.Instance.ArmySongDuration = 45f;
                    BardSongSettingsManager.Instance.ResetOrder();
                }

                if (ImGui.Selectable("45-45-45 特化歌轴"))
                {
                    BardSettings.Instance.WandererSongDuration = 45f;
                    BardSettings.Instance.MageSongDuration = 45f;
                    BardSettings.Instance.ArmySongDuration = 45f;
                    BardSongSettingsManager.Instance.ResetOrder();
                }
                ImGui.EndCombo();
            }
            
            ImGui.Checkbox("战斗开始时重置顺序：", ref BardSettings.Instance.ResetSongOrder);
            var songOrder = BardSettings.Instance.SongOrderOnReset;

            for (int i = 0; i < songOrder.Count; i++)
            {
                var song = songOrder[i];
                string name = song switch
                {
                    Song.Wanderer => "旅神",
                    Song.Mage => "贤者",
                    Song.Army => "军神",
                    _ => song.ToString()
                };

                uint color = song switch
                {
                    Song.Wanderer => // 绿色
                        ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.9f, 0.35f, 1.0f)),
                    Song.Army => // 黄色
                        ImGui.ColorConvertFloat4ToU32(new Vector4(0.88f, 0.55f, 0.03f, 1.0f)),
                    Song.Mage => // 蓝色
                        ImGui.ColorConvertFloat4ToU32(new Vector4(0.31f, 0.33f, 0.89f, 1.0f)),
                    _ => ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f))
                };
                
                float squareSize =  ImGui.GetFontSize() - 9; 
                Vector2 cursorPos = ImGui.GetCursorScreenPos(); // 当前光标屏幕位置
                float squareY = cursorPos.Y + (ImGui.GetFontSize() - squareSize) / 2f; // 居中对齐
                
                // 绘制小圆角方块
                ImGui.GetWindowDrawList().AddRectFilled(
                    new Vector2(cursorPos.X, squareY),
                    new Vector2(cursorPos.X + squareSize, squareY + squareSize),
                    color,
                    squareSize / 2f
                );
                
                // 在颜色方块后面留出空隙
                ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + squareSize + 6f, cursorPos.Y));

                // 用Selectable作为拖拽源（必须在源内定义SetPayload）
                if (ImGui.Selectable($"{name}##song_{i}", false, ImGuiSelectableFlags.AllowDoubleClick, new Vector2(ImGui.GetFontSize() * 2, ImGui.GetFontSize())))
                { }
                
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("拖动以更改顺序");
                    ImGui.EndTooltip();
                }

                if (ImGui.BeginDragDropSource())
                {
                    unsafe
                    {
                        int index = i;
                        ImGui.SetDragDropPayload("DND_SONG_ORDER", new IntPtr(&index), sizeof(int));
                        ImGui.Text($"拖拽：{name}");
                        ImGui.EndDragDropSource();
                    }
                }

                if (ImGui.BeginDragDropTarget())
                {
                    unsafe
                    {
                        var payload = ImGui.AcceptDragDropPayload("DND_SONG_ORDER", ImGuiDragDropFlags.AcceptNoDrawDefaultRect);
                        if (payload.NativePtr != null && payload.Data != null)
                        {
                            int from = *(int*)payload.Data;
                            int to = i;

                            if (from != to && from >= 0 && to >= 0 &&
                                from < songOrder.Count && to <= songOrder.Count)
                            {
                                var moved = songOrder[from];
                                songOrder.RemoveAt(from);
                                songOrder.Insert(to, moved); // 无需额外判断
                                BardSettings.Instance.Save();
                            }
                        }
                        ImGui.EndDragDropTarget();
                    }
                }

                if (i < songOrder.Count - 1)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted("-> ");
                    ImGui.SameLine();
                }
            }

            /*for (int i = 0; i < BardSettings.Instance.SongOrderOnReset.Count; i++)
            {
                var song = BardSettings.Instance.SongOrderOnReset[i];
                string songName = song switch
                {
                    Song.Wanderer => "旅神",
                    Song.Mage => "贤者",
                    Song.Army => "军神",
                    _ => song.ToString()
                };

                ImGui.Text($"第{i + 1}首：{songName}");

                ImGui.SameLine();
                if (i > 0 && ImGui.SmallButton($"↑##{i}"))
                {
                    (BardSettings.Instance.SongOrderOnReset[i], BardSettings.Instance.SongOrderOnReset[i - 1]) = 
                        (BardSettings.Instance.SongOrderOnReset[i - 1], BardSettings.Instance.SongOrderOnReset[i]);
                }

                ImGui.SameLine();
                if (i < 2 && ImGui.SmallButton($"↓##{i}"))
                {
                    (BardSettings.Instance.SongOrderOnReset[i], BardSettings.Instance.SongOrderOnReset[i + 1]) =
                        (BardSettings.Instance.SongOrderOnReset[i + 1], BardSettings.Instance.SongOrderOnReset[i]);
                }
            }*/
            

            ImGui.Separator();
            var opener = "";
            switch (BardSettings.Instance.Opener)
            {
                case 0:
                    opener = "90-100级 3G团辅起手";
                    break;
                case 1:
                    opener = "90-100级 2G团辅起手";
                    break;
                case 2:
                    opener = "100级伊甸 3G团辅起手";
                    break;
                case 3:
                    opener = "70-80级 3G团辅起手";
                    break;
                case 4:
                    opener = "70级神兵 5G团辅起手";
                    break;
            }

            if (ImGui.BeginCombo("起手选择", opener))
            {
                if (ImGui.Selectable("90-100级 3G团辅起手"))
                    BardSettings.Instance.Opener = 0;
                if (ImGui.Selectable("90-100级 2G团辅起手"))
                    BardSettings.Instance.Opener = 1;
                if (ImGui.Selectable("100级伊甸 3G团辅起手"))
                    BardSettings.Instance.Opener = 2;
                if (ImGui.Selectable("70-80级 3G团辅起手"))
                    BardSettings.Instance.Opener = 3;
                if (ImGui.Selectable("70级神兵 5G团辅起手"))
                    BardSettings.Instance.Opener = 4;
                ImGui.EndCombo();
            }

            /*
            ImGuiHelper.LeftInputInt("倒计时提前使用起手  (毫秒)", ref BardSettings.Instance.OpenerTime, 0, 1000);
            */
            ImGui.Separator();
            ImGui.Text("爆发药设置：" + (BardSettings.Instance.UsePotionInOpener ? "起手吃" : "2分钟爆发吃"));
            if (!QT.GetQt("爆发药"))
                ImGui.TextColored(new Vector4(0.7f, 0.8f, 0.0f, 1.0000f), "如果你希望使用爆发药，请在QT面板中开启爆发药开关");
            ImGui.Checkbox("起手吃爆发药", ref BardSettings.Instance.UsePotionInOpener);
            ImGui.Separator();
            if (BardSettings.Instance.GambleTripleApex)
                ImGui.TextColored(new Vector4(1, 0.7f, 0, 1), "警告，你选择赌每120秒三根绝峰箭，可能会导致爆发期灵魂之声能量不足");
            ImGui.Checkbox("是否赌每120秒三根绝峰箭", ref BardSettings.Instance.GambleTripleApex);
            ImGui.Separator();
            /*if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
                ImGui.TextColored(new Vector4(1, 0.7f, 0, 1), "警告，你开启了全局能力技能不卡GCD，可能导致本ACR产生能力技插入问题，建议关闭");
            ImGui.Checkbox("全局能力技能不卡GCD", ref SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3);
            ImGui.Separator();*/
            ImGui.Text("大地神自动对齐：");
            if ((BardSettings.Instance.NaturesMinneWithRecitation || BardSettings.Instance.NaturesMinneWithZoe ||
                 BardSettings.Instance.NaturesMinneWithNeutralSect) && !QT.GetQt("大地神"))
                ImGui.TextColored(new Vector4(0.7f, 0.8f, 0.0f, 1.0000f), "如果你希望使用自动大地神，请在QT面板中开启大地神开关");
            ImGui.Checkbox("秘策", ref BardSettings.Instance.NaturesMinneWithRecitation);
            ImGui.SameLine();
            ImGui.Checkbox("活化", ref BardSettings.Instance.NaturesMinneWithZoe);
            ImGui.SameLine();
            ImGui.Checkbox("中间学派", ref BardSettings.Instance.NaturesMinneWithNeutralSect);
            ImGui.Separator();
            ImGui.Checkbox("##WelcomeVoice", ref BardSettings.Instance.WelcomeVoice);
            ImGui.SameLine();
            ImGui.Text("是否播放欢迎语音 - 依赖插件");
            ImGui.SameLine();
            var dailyRoutinesLink = new Hyperlink("Daily Routines", "https://github.com/AtmoOmen/DalamudPlugins");
            dailyRoutinesLink.Render();      
            ImGui.Separator();
            if (ImGui.Button("保存设置"))
                BardSettings.Instance.Save();
        }
        ImGui.Separator();
        if (ImGui.CollapsingHeader("   高级设置"))
        {
            ImGui.TextColored(new Vector4(1, 0.7f, 0, 1), "除非你明白你要做什么，不然请别动这几项\n建议仅在受网络延迟与动画锁影响，爆发期打不满或者卡GCD时，再做调整");
            UiHelper.RightInputInt("非起手的旅神歌在下个GCD前多久使用", ref BardSettings.Instance.WandererBeforeGcdTime,
                500, 2000, "(毫秒)");
            UiHelper.RightInputInt("爆发药水的动画持续时间",
                ref BardSettings.Instance.PotionBeforeGcdTime, 500, 2000, "(毫秒)");
            UiHelper.RightInputInt("猛者强击在下个GCD前多久使用",
                ref BardSettings.Instance.RagingStrikeBeforeGcdTime, 500, 2000, "(毫秒)");
            UiHelper.RightInputInt("战斗之声和光明神在下个GCD前多久使用",
                ref BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs, 500, 2000, "(毫秒)");
            /*BardUtil.RightInputInt("九天连箭最晚在下个GCD前多久使用",
                ref BardSettings.Instance.EmpyrealArrowNotBeforeGcdTime, 0, 2000, "(毫秒)");*/
            ImGui.Separator();
            ImGui.Checkbox("显示快速光阴神面板", ref BardSettings.Instance.ShowWardensPaeanPanel);
            ImGuiHelper.LeftInputInt("光阴神面板图标大小", ref BardSettings.Instance.WardensPaeanPanelIconSize, 10, 80);
            ImGui.Checkbox("锁定光阴神面板", ref BardSettings.Instance.IsWardensPanelLocked);
            ImGui.Separator();
            ImGui.Checkbox("模仿绿玩手打循环（实验性功能）", ref BardSettings.Instance.ImitateGreenPlayer);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("在旅神歌与猛者强击前插入伤腿与伤足");
            ImGui.Separator();
            if (ImGui.Button("保存高级设置"))
                BardSettings.Instance.Save();
            ImGui.SameLine();
            if (ImGui.Button("重置高级设置"))
            {
                BardSettings.Instance.WandererBeforeGcdTime = 600;
                BardSettings.Instance.PotionBeforeGcdTime = 800;
                BardSettings.Instance.RagingStrikeBeforeGcdTime = 600;
                BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs = 1300;
                //BardSettings.Instance.EmpyrealArrowNotBeforeGcdTime = 500;
                BardSettings.Instance.Save();
            }
        }
        
        /*ImGui.Separator();
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
                    bool selected = BardSettings.Instance.SelectedTimeLinesForUpdate.TryGetValue(timeline.Name, out bool isSelected) && isSelected;
                    if (ImGui.Checkbox(timeline.Name, ref selected))
                    {
                        BardSettings.Instance.SelectedTimeLinesForUpdate[timeline.Name] = selected;
                        BardSettings.Instance.Save();
                    }
                    ImGui.Separator();
                }
                if (ImGui.Button("保存时间轴设置"))
                    BardSettings.Instance.Save();
            }
            ImGui.Separator();
        }*/

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
        ImGui.Text("当前玩家 Entity ID: " + Core.Me.EntityId);
        ImGui.Text("当前目标 Entity ID: " + Core.Me.GetCurrTarget()?.EntityId);
        ImGui.Text("当前目标 Date ID: " + Core.Me.GetCurrTarget()?.DataId);

        ImGui.Separator();
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.4314f, 0.6667f, 0.5569f, 1));
        ImGui.Text("神秘代码：");
        if (ImGui.InputText("##UnlockPassword", ref BardSettings.Instance.UnlockPassword, 100))
        {
            BardSettings.Instance.Save();
        }
        ImGui.Text("请输入技能名，以搜索对应id：");
        if (ImGui.InputText("##SearchQuery", ref searchQuery, 100))
        {
            // 在输入框变化时实时更新匹配项
            matchingSkills = BardDefinesData.GetMatchingSkills(searchQuery);
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