using System.Numerics;
using Dalamud.Interface;
using Dalamud.Bindings.ImGui;

namespace Wotou.Common;

public class UiHelper
{
    public static bool RightInputInt(string label, ref int value, int min, int max, string unitLabel = "", int step = 1, string tooltip = "")
    {
        // 获取标签的实际渲染宽度
        
        Vector2 labelSize = ImGui.CalcTextSize(label);
    
        // 固定输入框和单位标签之间的间距
        float labelWidth = labelSize.X; // 标签的实际宽度
        float globalFontSize = UiBuilder.DefaultFontSizePx;   // Get the global font size in pixels
        float globalFontScale = ImGui.GetIO().FontGlobalScale;
        float inputWidth = globalFontSize * globalFontScale * 5 + 25;  // 输入框的宽度
        float unitLabelWidth = !string.IsNullOrEmpty(unitLabel) ? ImGui.CalcTextSize(unitLabel).X : 0;
        float totalInputWidth = inputWidth + unitLabelWidth + 5; // 输入框和单位标签的总宽度（带间距）
    
        // 获取当前可用区域的宽度
        float contentRegionWidth = ImGui.GetContentRegionAvail().X;
    
        // 显示左对齐的标签
        ImGui.Text(label);
        if (tooltip != "")
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text(tooltip);
                ImGui.EndTooltip();
            }
        }
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