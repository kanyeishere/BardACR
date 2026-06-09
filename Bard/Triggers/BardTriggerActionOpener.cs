using AEAssist.CombatRoutine.Trigger;
using Dalamud.Bindings.ImGui;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.Triggers;

public class BardTriggerActionOpener : ITriggerAction
{
    public string DisplayName { get; } = "Bard/起手设置(含自定义编辑)";
    public string Remark { get; set; } = "";

    public int Opener;
    public int SelectedCustomOpenerIndex;
    private string _search = "";

    public BardTriggerActionOpener()
    {
        Opener = BardSettings.Instance.Opener;
        SelectedCustomOpenerIndex = BardSettings.Instance.SelectedCustomOpenerIndex;
    }
    
    private static readonly (string Label, int Value)[] OpenerOptions =
    [
        ("90-100级 3G团辅起手", 0),
        ("90-100级 2G团辅起手", 1),
        ("100级伊甸 3G团辅起手", 2),
        ("100级DM 1G团辅起手", 6),
        ("70-80级 3G团辅起手", 3),
        ("70级神兵 5G团辅起手", 4),
        ("自定义起手", 5)
    ];

    private static string GetOpenerLabel(int opener)
    {
        foreach (var option in OpenerOptions)
        {
            if (option.Value == opener)
                return option.Label;
        }

        return OpenerOptions[0].Label;
    }

    public bool Draw()
    {
        var currentLabel = GetOpenerLabel(Opener);
        if (ImGui.BeginCombo("起手选择", currentLabel))
        {
            foreach (var option in OpenerOptions)
            {
                var selected = Opener == option.Value;
                if (ImGui.Selectable(option.Label, selected))
                    Opener = option.Value;
                if (selected) ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }


        if (Opener == 5)
            DrawCustomOpenerEditor();

        return true;
    }

    public bool Handle()
    {
        if (Opener == 5 && BardSettings.Instance.CustomOpeners.Count > 0)
        {
            var idx = Math.Clamp(SelectedCustomOpenerIndex, 0, BardSettings.Instance.CustomOpeners.Count - 1);
            var currentSkills = BardSettings.Instance.CustomOpeners[idx].Skills;
            var duplicateIndex = FindSamePresetIndex(currentSkills, idx);
            if (duplicateIndex >= 0)
                idx = duplicateIndex;
            SelectedCustomOpenerIndex = idx;
        }

        BardSettings.Instance.Opener = Opener;
        BardSettings.Instance.SelectedCustomOpenerIndex = Math.Clamp(SelectedCustomOpenerIndex, 0, Math.Max(0, BardSettings.Instance.CustomOpeners.Count - 1));
        BardSettings.Instance.Save();
        return true;
    }

    private void DrawCustomOpenerEditor()
    {
        var allSkills = BardDefinesData.SkillDictionary.OrderBy(skill => skill.Key).ToList();
        var skillOptions = BardSkillDisplayHelper.BuildSkillOptions(allSkills);
        if (BardSettings.Instance.CustomOpeners.Count == 0)
            BardSettings.Instance.CustomOpeners.Add(new BardSettings.CustomOpenerPreset { Name = "自定义起手1" });

        if (SelectedCustomOpenerIndex < 0 || SelectedCustomOpenerIndex >= BardSettings.Instance.CustomOpeners.Count)
            SelectedCustomOpenerIndex = 0;

        ImGui.Separator();
        if (ImGui.SmallButton("+ 新增预设##trigger"))
        {
            var newSkills = new List<uint>();
            var sameIndex = FindSamePresetIndex(newSkills, -1);
            if (sameIndex >= 0)
            {
                SelectedCustomOpenerIndex = sameIndex;
            }
            else
            {
                BardSettings.Instance.CustomOpeners.Add(new BardSettings.CustomOpenerPreset
                {
                    Name = $"自定义起手{BardSettings.Instance.CustomOpeners.Count + 1}",
                    Skills = newSkills
                });
                SelectedCustomOpenerIndex = BardSettings.Instance.CustomOpeners.Count - 1;
            }
        }

        var names = BardSettings.Instance.CustomOpeners.Select((preset, idx) =>
            string.IsNullOrWhiteSpace(preset.Name) ? $"自定义起手{idx + 1}" : preset.Name).ToArray();
        ImGui.Combo("自定义起手预设", ref SelectedCustomOpenerIndex, names, names.Length);

        var presetRef = BardSettings.Instance.CustomOpeners[SelectedCustomOpenerIndex];
        ImGui.InputText("预设名称", ref presetRef.Name, 64);
        ImGui.SameLine();
        if (ImGui.SmallButton("删除当前预设##trigger") && BardSettings.Instance.CustomOpeners.Count > 1)
        {
            BardSettings.Instance.CustomOpeners.RemoveAt(SelectedCustomOpenerIndex);
            SelectedCustomOpenerIndex = Math.Clamp(SelectedCustomOpenerIndex, 0, BardSettings.Instance.CustomOpeners.Count - 1);
            return;
        }

        ImGui.Separator();
        if (ImGui.SmallButton("+ 添加技能##trigger"))
        {
            var preferredSkills = string.IsNullOrWhiteSpace(_search)
                ? skillOptions
                : skillOptions.Where(skill => skill.DisplayName.Contains(_search, StringComparison.OrdinalIgnoreCase)).ToList();
            var defaultSkill = preferredSkills.Count > 0 ? preferredSkills[0].Id : (skillOptions.Count > 0 ? skillOptions[0].Id : 0u);
            presetRef.Skills.Add(defaultSkill);
        }
        ImGui.SameLine();
        ImGui.Text("搜索：");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(220);
        ImGui.InputText("##TriggerCustomOpenerSearch", ref _search, 100);

        var filteredSkills = string.IsNullOrWhiteSpace(_search)
            ? skillOptions
            : skillOptions.Where(skill => skill.DisplayName.Contains(_search, StringComparison.OrdinalIgnoreCase)).ToList();

        for (var i = 0; i < presetRef.Skills.Count; i++)
        {
            var currentId = presetRef.Skills[i];
            var label = BardSkillDisplayHelper.GetPreferredSkillName(currentId);

            ImGui.PushID($"trigger_skill_{i}");
            ImGui.SetNextItemWidth(300);
            if (ImGui.BeginCombo("##skill", label))
            {
                foreach (var skill in filteredSkills)
                {
                    var isSelected = currentId == skill.Id;
                    if (ImGui.Selectable(skill.DisplayName, isSelected)) presetRef.Skills[i] = skill.Id;
                    if (isSelected) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            if (ImGui.SmallButton("删除"))
            {
                presetRef.Skills.RemoveAt(i);
                ImGui.PopID();
                i--;
                continue;
            }
            ImGui.PopID();
        }

    }

    private int FindSamePresetIndex(List<uint> skills, int excludeIndex)
    {
        for (var i = 0; i < BardSettings.Instance.CustomOpeners.Count; i++)
        {
            if (i == excludeIndex) continue;
            var target = BardSettings.Instance.CustomOpeners[i].Skills;
            if (target.Count != skills.Count) continue;

            var same = true;
            for (var j = 0; j < skills.Count; j++)
            {
                if (target[j] != skills[j])
                {
                    same = false;
                    break;
                }
            }
            if (same) return i;
        }
        return -1;
    }

}
