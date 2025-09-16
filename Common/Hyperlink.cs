using System.Diagnostics;
using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace Wotou.Common;

public class Hyperlink
{
    public string Label { get; }
    public string Url { get; }
    public Vector4 LinkColor { get; }
    public Vector4 HoverColor { get; }

    public Hyperlink(string label, string url, Vector4? linkColor = null, Vector4? hoverColor = null)
    {
        Label = label;
        Url = url;
        LinkColor = linkColor ?? new Vector4(0.0f, 0.5f, 1.0f, 1.0f); // 默认蓝色
        HoverColor = hoverColor ?? new Vector4(0.0f, 0.7f, 1.0f, 1.0f); // 悬浮更亮的蓝色
    }

    public void Render()
    {
        // 设置文本颜色为超链接颜色
        ImGui.PushStyleColor(ImGuiCol.Text, LinkColor);
        ImGui.Text(Label);
        ImGui.PopStyleColor();

        // 如果鼠标悬浮在文本上
        if (ImGui.IsItemHovered())
        {
            ImGui.PushStyleColor(ImGuiCol.Text, HoverColor); // 设置悬浮颜色
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand); // 设置鼠标为手型
            ImGui.PopStyleColor();

            // 如果点击
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                OpenUrl();
            }
        }
    }

    private void OpenUrl()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Url,
                UseShellExecute = true // 使用系统默认浏览器打开
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"无法打开链接：{ex.Message}");
        }
    }
}