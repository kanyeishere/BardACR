using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;

namespace ScriptTest;

public class 鱼有线: ITriggerScript
{
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        var maxDistance = 35f;
        var enemies = ECHelper.Objects
            .Where(obj => obj is IBattleChara)
            .Cast<IBattleChara>()
            .Where(c => c.DistanceToPlayer() < maxDistance && c.IsEnemy())
            .ToArray()
            .OrderBy(c => c.DistanceToPlayer());
        
        var 无目标鱼 = enemies.FirstOrDefault(c => c.DataId == 18346 && c.TargetObject == null);
        
        return 无目标鱼 == null;
    }
}