using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using AEAssist.Extension;
using AEAssist.GUI;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Dancer.Ability;
using Wotou.Dancer.Data;
using Wotou.Dancer.GCD;
using Wotou.Dancer.Opener;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Trigger;
using ImGuiNET;

namespace Wotou.Dancer;

public class DancerRotationEntry : IRotationEntry
{
    public void Dispose()
    {
    }
    
    public string AuthorName { get; set; } = "Wotou";

    private List<SlotResolverData> SlotResolvers = new()
    {
        new SlotResolverData(new DancerLastDanceHighGcd(), SlotMode.Gcd), //马上到期的终结舞步
        
        new SlotResolverData(new DancerFinishingMoveGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerStandardStepDancingGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerStandardStepGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerTechnicalStepDancingGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerTechnicalStepGcd(), SlotMode.Gcd),
        new SlotResolverData(new Dancer1GBeforeTechStepGcd(), SlotMode.Gcd),
        
        new SlotResolverData(new DancerProcHighGcd(), SlotMode.Gcd), // 快过期的触发
        new SlotResolverData(new DancerSaberDanceHighGcd(), SlotMode.Gcd), //团辅期高能量剑舞
        new SlotResolverData(new DancerTillanaGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerStarfallDanceGCD(), SlotMode.Gcd),
        new SlotResolverData(new DancerLastDanceGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerSaberDanceMediumGcd(), SlotMode.Gcd), //团辅期低能量剑舞

        new SlotResolverData(new DancerSaberDanceGcd(), SlotMode.Gcd), //普通剑舞
        new SlotResolverData(new DancerProcGcd(), SlotMode.Gcd),
        new SlotResolverData(new DancerBaseGcd(), SlotMode.Gcd),
        
        new SlotResolverData(new DancerDevilmentAbility(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFlourishAbility(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFanDance4Ability(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFanDance3Ability(), SlotMode.OffGcd),
        new SlotResolverData(new DancerFanDanceAbility(), SlotMode.OffGcd),
    };
    
    public Rotation Build(string settingFolder)
    {
        DancerSettings.Build(settingFolder);
        BuildQT(settingFolder);
        var rot = new Rotation(SlotResolvers)
        {
            TargetJob = Jobs.Dancer,
            AcrType = AcrType.Both,
            MinLevel = 100,
            MaxLevel = 100,
            Description = "100级高难舞者-测试版",
        };
        rot.SetRotationEventHandler(new DancerRotationEventHandler());
        rot.AddOpener(GetOpener);
        rot.AddTriggerAction(new DancerTriggerActionQt());
        return rot;
    }

    private static IOpener GetOpener(uint level)
    {
        return new DNCStdOpener100();
    }

    // 声明当前要使用的UI的实例 示例里使用QT
    public static JobViewWindow QT { get; private set; }
    
    public static HotkeyWindow? DancePartnerPanel { get; set; }
    
    // 如果你不想用QT 可以自行创建一个实现IRotationUI接口的类
    public IRotationUI GetRotationUI()
    {
        return DancerRotationEntry.QT;
    }
    
    // 构造函数里初始化QT
    public void BuildQT(string settingFolder)
    {
        // JobViewSave是AE底层提供的QT设置存档类 在你自己的设置里定义即可
        // 第二个参数是你设置文件的Save类 第三个参数是QT窗口标题
        QT = new JobViewWindow(DancerSettings.Instance.JobViewSave, DancerSettings.Instance.Save, "Wotou");
        
        var myJobViewSave = new JobViewSave();
        myJobViewSave.ShowHotkey = DancerSettings.Instance.ShowDancePartnerPanel;
        myJobViewSave.QtHotkeySize = new Vector2(DancerSettings.Instance.DancePartnerIconSize, DancerSettings.Instance.DancePartnerIconSize);
        DancePartnerPanel = new HotkeyWindow(myJobViewSave, "Custom DNC HotkeyWindow");

        // 创建 JobViewWindow 并传递回调
        //var QT2 = new JobViewWindow(DancerSettings.Instance.JobViewSave, DancerSettings.Instance.Save, "Wotou");

        // 为 JobViewWindow 设置 UpdateAction 来渲染 HotkeyWindow2
        QT.SetUpdateAction(() =>
        {   
            var myJobViewSave = new JobViewSave();
            myJobViewSave.QtHotkeySize = new Vector2(DancerSettings.Instance.DancePartnerIconSize, DancerSettings.Instance.DancePartnerIconSize);
            myJobViewSave.ShowHotkey = DancerSettings.Instance.ShowDancePartnerPanel;
            DancePartnerPanel.DrawHotkeyWindow(new QtStyle(DancerSettings.Instance.JobViewSave));
            DancePartnerPanel = new HotkeyWindow(myJobViewSave, "Custom DNC HotkeyWindow");
            DancePartnerPanel.HotkeyLineCount = 1;
            UpdateDancerPartnerPanel();
        });
        
        QT.AddTab("通用", DrawGeneral);
        QT.AddTab("Dev", DrawQtDev);
        
        QT.AddQt(QTKey.UsePotion, false, "是否使用爆发药");
        QT.AddQt(QTKey.Aoe, true, "是否使用AOE");
        QT.AddQt(QTKey.TechnicalStep, true, "是否使用技巧舞步与进攻之探戈");
        QT.AddQt(QTKey.StandardStep, true, "是否使用标准舞步与结束动作");
        QT.AddQt(QTKey.Flourish, true, "是否使用百花争艳");
        QT.AddQt(QTKey.FinalBurst, false, "是否倾泻资源");
        
        QT.AddHotkey("防击退", new HotKeyResolver_NormalSpell(DancerDefinesData.Spells.ArmsLength, SpellTargetType.Target));
        QT.AddHotkey("内丹", new HotKeyResolver_NormalSpell(DancerDefinesData.Spells.SecondWind, SpellTargetType.Self));
        QT.AddHotkey("桑巴", new HotKeyResolver_NormalSpell(DancerDefinesData.Spells.ShieldSamba, SpellTargetType.Self));
        QT.AddHotkey("华尔兹", new HotKeyResolver_NormalSpell(DancerDefinesData.Spells.CuringWaltz, SpellTargetType.Self));
        QT.AddHotkey("秒开即兴", new HotkeyResolver_General("../../ACR/Wotou/Dancer/Asset/Improvisation.png", Improvisation));
        QT.AddHotkey("疾跑", new HotKeyResolver_疾跑());
        QT.AddHotkey("前冲步", new HotKeyResolver_NormalSpell(DancerDefinesData.Spells.EnAvant, SpellTargetType.Target));
        QT.AddHotkey("爆发药", new HotKeyResolver_Potion());
        QT.AddHotkey("极限技", new HotKeyResolver_LB());
        DancerSettings.Instance.JobViewSave.HotkeyLineCount = 5;
    }
    
    public static void UpdateDancerPartnerPanel()
    {
        PartyHelper.UpdateAllies();
        for (var i = 1; i < PartyHelper.Party.Count; i++)
        {
            var index = i;
            DancePartnerPanel?.AddHotkey("切换舞伴: " + PartyHelper.Party[i].Name, new HotkeyResolver_General( "../../ACR/Wotou/Dancer/Asset/ClosedPosition.png", () => DancerRotationEntry.ClosedPosition(index)));
        }
    }

    private static void Improvisation()
    {
        if (!DancerDefinesData.Spells.Improvisation.CoolDownInGCDs(0) || 
            Core.Resolve<JobApi_Dancer>().IsDancing)
            return;
        if (AI.Instance.BattleData.NextSlot == null)
            AI.Instance.BattleData.NextSlot = new Slot();
        AI.Instance.BattleData.NextSlot.Add(DancerDefinesData.Spells.Improvisation.GetSpell());
        AI.Instance.BattleData.NextSlot.Add(DancerDefinesData.Spells.ImprovisationFinish.GetSpell());
    }
    
    private static void ClosedPosition(int index)
    {
        var partyMembers = PartyHelper.Party;
        if (partyMembers.Count < index + 1)
            return;
        if (!DancerDefinesData.Spells.ClosedPosition.CoolDownInGCDs(0) || 
            Core.Resolve<JobApi_Dancer>().IsDancing)
            return;
        if (AI.Instance.BattleData.NextSlot == null)
            AI.Instance.BattleData.NextSlot = new Slot();
        if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition))
        {
            AI.Instance.BattleData.NextSlot.Add(DancerDefinesData.Spells.Ending.GetSpell());
            AI.Instance.BattleData.NextSlot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition, partyMembers[index]));
        }
        else
        {
            AI.Instance.BattleData.NextSlot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition, partyMembers[index]));
        }
    }

    public void DrawGeneral(JobViewWindow jobViewWindow)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1f,0.36f,0.54f, 1));
        if (ImGui.CollapsingHeader("   重要说明"))
        {
            ImGui.Text("舞者ACR\n技速选择2.5，暂时只适配100级高难环境\n90级及以下副本时，本ACR目前所使用的是非最优循环");
        }

        ImGui.Separator();
        if (ImGui.CollapsingHeader("   基础设置"))
        {
            ImGuiHelper.LeftInputInt("倒计时提前使用小舞  (毫秒)", ref DancerSettings.Instance.OpenerStandardStepTime, 6500, 15000);
            ImGui.Separator();

            ImGuiHelper.LeftInputInt("倒计时提前使用起手  (毫秒)", ref DancerSettings.Instance.OpenerTime, 0, 1000);
            ImGui.Separator();
            ImGui.Text("爆发药设置：" + (DancerSettings.Instance.UsePotionInOpener ? "起手吃" : "2分钟爆发吃"));
            if (!QT.GetQt("爆发药"))
                ImGui.TextColored(new Vector4(0.7f, 0.8f, 0.0f, 1.0000f), "如果你希望使用爆发药，请在QT面板中开启爆发药开关");
            ImGui.Checkbox("起手吃爆发药", ref DancerSettings.Instance.UsePotionInOpener);
            ImGui.Separator();
            if (ImGui.Button("保存设置")) DancerSettings.Instance.Save();
            
        }
        ImGui.Separator();
        if (ImGui.CollapsingHeader("   界面设置"))
        {
            ImGui.Checkbox("显示快速舞伴切换面板", ref DancerSettings.Instance.ShowDancePartnerPanel);
            ImGuiHelper.LeftInputInt("舞伴面板图标大小", ref DancerSettings.Instance.DancePartnerIconSize, 10, 80);

            ImGui.Separator();
            if (ImGui.Button("保存界面设置")) DancerSettings.Instance.Save();
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
    }

    public void OnDrawSetting(){
        DancerSettingsUI.Instance.Draw();
    }
    
    public void DrawQtDev(JobViewWindow jobViewWindow)
    {
        ImGui.Text("Dev信息");
        foreach (var v in jobViewWindow.GetQtArray()) ImGui.Text($"Qt按钮: {v}");

        foreach (var v in jobViewWindow.GetHotkeyArray()) ImGui.Text($"Hotkey按钮: {v}");
    }
}