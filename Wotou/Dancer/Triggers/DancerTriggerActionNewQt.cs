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
            ImGui.Text("点击按钮在三种状态间切换：未添加 / 已关闭 / 已启用 ：");
            ImGui.NewLine();
            int columns = 5;
            int count = 0;

            foreach (var qt in qtArray)
            {
                ImGui.PushID(qt);

                if (qtValues.TryGetValue(qt, out var isEnabled))
                {
                    ImGui.PushStyleColor(
                        ImGuiCol.Text,
                        isEnabled
                            ? new Vector4(0f, 1f, 0f, 1f)              // ✅ 启用：绿色
                            : new Vector4(1.0f, 0.4f, 0.7f, 1.0f)      // ❌ 未启用：粉红色
                    );
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1f)); // 🆕 未添加：默认白
                }

                if (ImGui.Button(qt))
                {
                    if (!qtValues.ContainsKey(qt))
                        qtValues[qt] = false;         // 🆕 → ❌
                    else if (!qtValues[qt])
                        qtValues[qt] = true;          // ❌ → ✅
                    else
                        qtValues.Remove(qt);          // ✅ → 🆕
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
            
            if (ImGui.Button("全部启用"))
            {
                foreach (var key in qtValues.Keys.ToList())
                    qtValues[key] = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("全部关闭"))
            {
                foreach (var key in qtValues.Keys.ToList())
                    qtValues[key] = false;
            }
            
            ImGui.SameLine();
            
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
