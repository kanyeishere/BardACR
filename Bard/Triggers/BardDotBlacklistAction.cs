using AEAssist.CombatRoutine.Trigger;
using Dalamud.Bindings.ImGui;
using Wotou.Bard.Data;

namespace Wotou.Bard.Triggers;

public class BardDotBlacklistAction : ITriggerAction
{
    public string DisplayName { get; } = "Bard/DoT黑名单管理";
    public string Remark { get; set; } = "管理不会上DoT的敌人ID (DataId)";

    public HashSet<uint> DotBlackListForAction;
    private string newIdInput = string.Empty;
    
    public BardDotBlacklistAction()
    {
        DotBlackListForAction = BardBattleData.Instance.DotBlackList;
    }

    public bool Draw()
    {
        ImGui.Text("添加或移除不希望上毒/风的目标DataId：");

        ImGui.InputText("添加ID", ref newIdInput, 20);
        ImGui.SameLine();
        if (ImGui.Button("添加"))
        {
            if (uint.TryParse(newIdInput, out uint id))
            {
                DotBlackListForAction.Add(id);
                newIdInput = string.Empty;
            }
        }

        ImGui.Separator();
        ImGui.Text("当前黑名单：");

        foreach (var id in DotBlackListForAction.ToList())
        {
            ImGui.Text($"ID: {id}");
            ImGui.SameLine();
            if (ImGui.Button($"移除##{id}"))
            {
                DotBlackListForAction.Remove(id);
                break;
            }
        }

        ImGui.Separator();
        if (ImGui.Button("清空所有黑名单"))
        {
            DotBlackListForAction.Clear();
        }

        return true;
    }

    public bool Handle()
    {
        BardBattleData.Instance.DotBlackList = DotBlackListForAction;
        // 不在这里实际处理逻辑，仅通过 UI 修改数据结构
        return true;
    }
}