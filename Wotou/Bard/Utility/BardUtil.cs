using System.Globalization;
using System.Numerics;
using AEAssist;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Interface;
using ImGuiNET;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Utility;

public static class BardUtil
{
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;
    
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    
    private static bool IsRadiantFinaleConditionMet()
    {
        // 检查技能是否已解锁：如果未解锁，直接返回true。如果已解锁，检查是否有Radiant Finale的Buff效果
        return !RadiantFinale.IsUnlock() || Core.Me.HasLocalPlayerAura(RadiantFinaleBuff);
    }
    
    public static bool HasAllPartyBuff()
    {
        return Core.Me.HasAura(BattleVoiceBuff) && Core.Me.HasAura(RagingStrikesBuff) && IsRadiantFinaleConditionMet();
    }
    
    public static bool HasAnyPartyBuff()
    {
        return Core.Me.HasAura(BattleVoiceBuff) || Core.Me.HasAura(RagingStrikesBuff) || Core.Me.HasAura(RadiantFinaleBuff);
    }
    
    public static bool HasNoPartyBuff()
    {
        return !Core.Me.HasAura(BattleVoiceBuff) && !Core.Me.HasAura(RagingStrikesBuff) && !Core.Me.HasAura(RadiantFinaleBuff);
    }
    
    public static bool PartyBuffWillBeReadyIn(int ms)
    {
        return BattleVoice.GetSpell().Cooldown.TotalMilliseconds <= ms || RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <= ms;
    }
    
    public static Song GetSongBySpell(uint song)
    {
        return song switch
        {
            BardDefinesData.Spells.TheWanderersMinuet => Song.WANDERER,
            BardDefinesData.Spells.MagesBallad => Song.MAGE,
            BardDefinesData.Spells.ArmysPaeon => Song.ARMY,
            _ => Song.NONE
        };
    }
    
    public static uint GetSpellBySong(Song song)
    {
        return song switch
        {
            Song.WANDERER => BardDefinesData.Spells.TheWanderersMinuet,
            Song.MAGE => BardDefinesData.Spells.MagesBallad,
            Song.ARMY => BardDefinesData.Spells.ArmysPaeon,
            _ => 0
        };
    }
    
    public static float GetSongDuration(Song song)
    {
        return song switch
        {
            Song.WANDERER => BardSettings.Instance.WandererSongDuration,
            Song.MAGE => BardSettings.Instance.MageSongDuration,
            Song.ARMY => BardSettings.Instance.ArmySongDuration,
            _ => 0
        };
    }

    public static void LogDebug(string title, string message)
    {
        if (BardRotationEntry.QT.GetQt("Debug"))
            LogHelper.Print(title, message); 
    }
    
    public static bool IsSongOrderNormal() => BardSettings.Instance.FirstSong == Song.WANDERER && BardSettings.Instance.SecondSong == Song.MAGE && BardSettings.Instance.ThirdSong == Song.ARMY;
    
    public static bool RightInputInt(string label, ref int value, int min, int max, string unitLabel = "", int step = 1)
    {
        // 获取标签的实际渲染宽度
        Vector2 labelSize = ImGui.CalcTextSize(label);
    
        // 固定输入框和单位标签之间的间距
        float labelWidth = labelSize.X; // 标签的实际宽度
        float globalFontSize = UiBuilder.DefaultFontSizePx;   // Get the global font size in pixels
        float globalFontScale = ImGui.GetIO().FontGlobalScale;
        float inputWidth = globalFontSize * globalFontScale * 5 + 20;  // 输入框的宽度
        float unitLabelWidth = !string.IsNullOrEmpty(unitLabel) ? ImGui.CalcTextSize(unitLabel).X : 0;
        float totalInputWidth = inputWidth + unitLabelWidth + 5; // 输入框和单位标签的总宽度（带间距）
    
        // 获取当前可用区域的宽度
        float contentRegionWidth = ImGui.GetContentRegionAvail().X;
    
        // 显示左对齐的标签
        ImGui.Text(label);
        ImGui.SameLine();
    
        // 根据标签的宽度和输入框的总宽度来调整偏移，确保输入框和单位标签都右对齐
        ImGui.SameLine(contentRegionWidth - totalInputWidth-4);
           
        // 如果有单位标签
        if (!string.IsNullOrEmpty(unitLabel))
        {
            // 保证单位标签紧跟在输入框后面
            ImGui.Text(unitLabel); // 显示单位标签
        }
        // 设置输入框的宽度
        ImGui.SameLine();  
        ImGui.SetNextItemWidth(inputWidth);

        // 输入框
        ImGui.PushID("##" + label);
        bool flag = ImGui.InputInt("", ref value, step);
        ImGui.PopID();

        // 限制输入的最小值和最大值
        value = Math.Clamp(value, min, max);
        
        return flag;
    }
}