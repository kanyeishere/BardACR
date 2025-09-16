using AEAssist.CombatRoutine.Trigger;
using Dalamud.Bindings.ImGui;
using Wotou.Bard.Data;

namespace Wotou.Bard.Triggers;

public class BardPitchPerfectMinCountAction : ITriggerAction
{
    public string DisplayName { get; } = "Bard/完美音调最少敌人数";
    public string Remark { get; set; } = "设置使用PitchPerfect（完美音调）所需的最少敌人数";

    public int minEnemyCount;

    public BardPitchPerfectMinCountAction()
    {
        minEnemyCount = BardBattleData.Instance.PitchPerfectMinEnemyCount;
    }

    public bool Draw()
    {
        ImGui.Text("设置使用完美音调所需的最少敌人数：");

        if (ImGui.InputInt("最少敌人数量", ref minEnemyCount))
        {
            if (minEnemyCount < 0)
                minEnemyCount = 0; // 防止负数
        }

        return true;
    }

    public bool Handle()
    {
        BardBattleData.Instance.PitchPerfectMinEnemyCount = minEnemyCount;
        return true;
    }
}