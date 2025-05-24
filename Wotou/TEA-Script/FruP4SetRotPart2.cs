using System.Numerics;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;

namespace ScriptTest;

public class FruP4SetRotPart2: ITriggerScript
{
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        if (scriptEnv.KV.TryGetValue("lightPosition", out var lightPositionObj))
        {
            string direction = "";

            Vector3 lightPosition = (Vector3)lightPositionObj;
            
            float angle = MathF.Atan2(-(lightPosition.X - 100), -(lightPosition.Z - 100)); // 北为0，逆时针为正
            angle = NormalizeAngle(angle); // 归一化到 [-π, π]

            if (angle > -MathF.PI / 8 && angle <= MathF.PI / 8)
                direction = "north";
            else if (angle > MathF.PI / 8 && angle <= 3 * MathF.PI / 8)
                direction = "northwest";
            else if (angle > 3 * MathF.PI / 8 && angle <= 5 * MathF.PI / 8)
                direction = "west";
            else if (angle > 5 * MathF.PI / 8 && angle <= 7 * MathF.PI / 8)
                direction = "southwest";
            else if (angle > 7 * MathF.PI / 8 || angle <= -7 * MathF.PI / 8)
                direction = "south";
            else if (angle > -7 * MathF.PI / 8 && angle <= -5 * MathF.PI / 8)
                direction = "southeast";
            else if (angle > -5 * MathF.PI / 8 && angle <= -3 * MathF.PI / 8)
                direction = "east";
            else if (angle > -3 * MathF.PI / 8 && angle <= -MathF.PI / 8)
                direction = "northeast";
        
            ECommons.Automation.Chat.Instance.SendMessage($"/pdr load AutoFaceCameraDirection");
            ECommons.Automation.Chat.Instance.SendMessage($"/pdrface ground {direction}");
            ECommons.Automation.Chat.Instance.SendMessage($"/e face direction: {direction}");
            return true;
        }
        return false;
    }
    
    private static float NormalizeAngle(float angle)
    {
        while (angle <= -MathF.PI) angle += 2 * MathF.PI;
        while (angle > MathF.PI) angle -= 2 * MathF.PI;
        return angle;
    }
}