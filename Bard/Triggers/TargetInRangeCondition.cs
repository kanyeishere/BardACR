using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Bindings.ImGui;

namespace Wotou.Bard.Triggers;

public class TargetInRangeCondition : ITriggerBase, ITriggerCond
{
    public string DisplayName { get; } = "通用/判断目标是否在范围内";

    public string Remark { get; set; } = "判断指定 Data ID 的复数目标中是否有一个位于设定中心点与范围之内";

    public uint DataId = 0;
    public Vector3 Center = new Vector3(0, 0, 0);
    public float Radius = 2f;

    public bool Draw()
    {
        ImGui.Text("判断指定 Data ID 的复数目标中是否有一个位于设定中心点与范围之内");

        ImGuiHelper.LeftInputInt("目标Data ID", ref DataId);
        ImGuiHelper.LeftInputFloat("范围半径", ref Radius, 1f, 200f);
        ImGuiHelper.LeftInputFloat("中心点X", ref Center.X);
        ImGuiHelper.LeftInputFloat("中心点Y", ref Center.Y);
        ImGuiHelper.LeftInputFloat("中心点Z", ref Center.Z);

        return false;
    }

    public bool Handle(ITriggerCondParams condParams)
    {
        var nearbyGameObjects = new Dictionary<uint, IGameObject>();
        // 获取附近对象
        Core.Resolve<MemApiTarget>().GetNearbyGameObjects(50, nearbyGameObjects);
        foreach (var obj in  nearbyGameObjects.Values)
        {
            if (obj.DataId != DataId)
                continue;

            var pos = obj.Position;
            if (Vector3.Distance(new Vector3(Center.X, Center.Y, Center.Z), pos) <= Radius)
                return true;
        }

        return false;
    }
}