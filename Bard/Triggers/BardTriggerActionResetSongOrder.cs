using AEAssist.CombatRoutine.Trigger;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using AEAssist;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Triggers
{
    public class BardTriggerActionResetSongOrder : ITriggerAction
    {
        public string DisplayName => "Bard/重置歌曲顺序";
        public string Remark { get; set; } = "";

        // 是否启用自动重置
        public bool EnableReset = true;

        // 设置的顺序（默认顺序）
        public List<Song> SongOrder = new() { Song.Wanderer, Song.Mage, Song.Army };

        public bool Draw()
        {
            ImGui.Checkbox("战斗开始时重置顺序", ref EnableReset);

            ImGui.Separator();
            ImGui.Text("拖动图标设置顺序：");
            ImGui.NewLine();

            for (int i = 0; i < SongOrder.Count; i++)
            {
                var song = SongOrder[i];
                string name = song switch
                {
                    Song.Wanderer => "旅神",
                    Song.Mage => "贤者",
                    Song.Army => "军神",
                    _ => song.ToString()
                };

                uint color = song switch
                {
                    Song.Wanderer => ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.9f, 0.35f, 1f)),
                    Song.Mage => ImGui.ColorConvertFloat4ToU32(new Vector4(0.31f, 0.33f, 0.89f, 1f)),
                    Song.Army => ImGui.ColorConvertFloat4ToU32(new Vector4(0.88f, 0.55f, 0.03f, 1f)),
                    _ => ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f))
                };

                float squareSize = ImGui.GetFontSize() - 6f;
                Vector2 cursorPos = ImGui.GetCursorScreenPos();
                float squareY = cursorPos.Y + (ImGui.GetFontSize() - squareSize) / 2f;

                ImGui.GetWindowDrawList().AddRectFilled(
                    new Vector2(cursorPos.X, squareY),
                    new Vector2(cursorPos.X + squareSize, squareY + squareSize),
                    color,
                    squareSize / 2f
                );

                ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + squareSize + 6f, cursorPos.Y));

                if (ImGui.Selectable($"{name}##song_{i}", false, ImGuiSelectableFlags.AllowDoubleClick, new Vector2(ImGui.GetFontSize() * 2.5f, ImGui.GetFontSize())))
                { }

                if (ImGui.BeginDragDropSource())
                {
                    unsafe
                    {
                        int index = i;
                        ImGui.SetDragDropPayload("DND_SONG_RESET_ORDER", new IntPtr(&index), sizeof(int));
                        ImGui.Text($"拖拽：{name}");
                        ImGui.EndDragDropSource();
                    }
                }

                if (ImGui.BeginDragDropTarget())
                {
                    unsafe
                    {
                        var payload = ImGui.AcceptDragDropPayload("DND_SONG_RESET_ORDER");
                        if (payload.NativePtr != null && payload.Data != null)
                        {
                            int from = *(int*)payload.Data;
                            int to = i;

                            if (from != to && from >= 0 && to >= 0 && from < SongOrder.Count && to <= SongOrder.Count)
                            {
                                var moved = SongOrder[from];
                                SongOrder.RemoveAt(from);
                                SongOrder.Insert(to, moved);
                            }
                        }
                        ImGui.EndDragDropTarget();
                    }
                }

                if (i < SongOrder.Count - 1)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted("-> ");
                    ImGui.SameLine();
                }
            }

            return true;
        }

        public bool Handle()
        {
            BardSettings.Instance.ResetSongOrder = EnableReset;
            BardSettings.Instance.SongOrderOnReset = SongOrder.ToList(); // 深拷贝
            return true;
        }
    }
}
