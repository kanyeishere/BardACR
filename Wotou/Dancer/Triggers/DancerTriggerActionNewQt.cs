using AEAssist.CombatRoutine.Trigger;
using ImGuiNET;
using System.Numerics;

namespace Wotou.Dancer.Triggers
{
    public class DancerTriggerActionNewQt : ITriggerAction
    {
        public string DisplayName { get; } = "Dancer/New QT";
        public string Remark { get; set; }
        
        public Dictionary<string, bool> qtValues = new();
        
        private readonly string[] qtArray;

        public DancerTriggerActionNewQt()
        {
            qtArray = DancerRotationEntry.QT.GetQtArray();
        }

        public bool Draw()
        {
            ImGui.NewLine();
            ImGui.Separator();
            ImGui.Text("点击下方任意按钮添加 QT 项目 ↓");
            ImGui.NewLine();
            int columns = 5;
            int count = 0;

            foreach (var qt in qtArray)
            {
                ImGui.PushID(qt);

                ImGui.PushStyleColor(
                    ImGuiCol.Text,
                    qtValues.ContainsKey(qt)
                        ? new Vector4(0f, 1f, 0f, 1f) // 绿色：已添加
                        : new Vector4(1f, 1f, 1f, 1f) // 白色：未添加
                );

                if (ImGui.Button(qt))
                {
                    if (qtValues.ContainsKey(qt))
                        qtValues.Remove(qt); 
                    else
                        qtValues.TryAdd(qt, false); // C# 9+ 简写，已有则不添加
                }

                ImGui.PopStyleColor();
                ImGui.PopID();

                if (++count % columns != 0)
                    ImGui.SameLine();
            }

            ImGui.NewLine();

            ImGui.Separator(); ;
            
            if (qtValues.Count == 0)
            {
                return true;
            }

            List<string> toRemove = new();

            foreach (var kvp in qtValues)
            {
                string qt = kvp.Key;
                bool val = kvp.Value;

                ImGui.PushID(qt);

                if (ImGui.Checkbox(" ", ref val))
                {
                    qtValues[qt] = val;
                }

                ImGui.SameLine();
                ImGui.Text(qt);
                ImGui.SameLine();

                Vector4 color = val ? new Vector4(0f, 1f, 0f, 1f) : new Vector4(1f, 0f, 0f, 1f);
                string status = val ? "（已启用）" : "（已关闭）";
                ImGui.TextColored(color, status);

                ImGui.SameLine();
                if (ImGui.Button("删除"))
                {
                    toRemove.Add(qt);
                }

                ImGui.PopID();
            }

            // 删除被标记的项
            foreach (var qt in toRemove)
            {
                qtValues.Remove(qt);
            }

            ImGui.Separator();
            if (ImGui.Button("清除所有"))
            {
                qtValues.Clear();
            }

            return true;
        }

        public bool Handle()
        {
            foreach (var kvp in qtValues)
            {
                DancerRotationEntry.QT.SetQt(kvp.Key, kvp.Value);
            }
            return true;
        }
    }
}
