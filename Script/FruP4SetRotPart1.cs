using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;

namespace ScriptTest;

public class FruP4SetRotPart1: ITriggerScript
{
    private Vector3 _lightPosition = Vector3.Zero;

    private readonly List<Vector3> _lightPositions = [];
    
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        scriptEnv.KV.Remove("lightPosition");

        if (condParams is not TetherCondParams { Args0: 133 } tether)
            return false;

        var pos = tether.Left.Position;

        if (!_lightPositions.Contains(pos))
        {
            _lightPositions.Add(pos);
        }

        if (_lightPositions.Count != 5)
            return false;


        _lightPosition = FindTopOfDShape(_lightPositions);
        
        scriptEnv.KV["lightPosition"] = _lightPosition;
        ECommons.Automation.Chat.Instance.SendMessage($"/e light position ({_lightPosition.X}, {_lightPosition.Z})");
        return true;
    }

    private static Vector3 FindTopOfDShape(List<Vector3> points)
    {
        if (points.Count != 5)
            throw new ArgumentException("必须恰好提供5个点");

        Vector3 topPoint = points[0];
        float maxClosestDist = float.MinValue;

        foreach (var p in points)
        {
            float closestDist = float.MaxValue;

            foreach (var other in points)
            {
                if (other == p) continue;

                float dist = DistanceXZ(p, other);
                if (dist < closestDist)
                    closestDist = dist;
            }

            if (closestDist > maxClosestDist)
            {
                maxClosestDist = closestDist;
                topPoint = p;
            }
        }

        return topPoint;
    }
    
    private static float DistanceXZ(Vector3 a, Vector3 b)
    {
        float dx = a.X - b.X;
        float dz = a.Z - b.Z;
        return MathF.Sqrt(dx * dx + dz * dz);
    }
}