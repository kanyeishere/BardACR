using AEAssist;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
namespace ScriptTest;

public class P4Script : ITriggerScript
{
    // Check返回True代表此节点执行完毕， 非检测模式下，返回false时一直重复执行（每帧）
    // 检测模式下， condParams必定为空
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        var oracleOfDarknessList = TargetMgr.Instance.Enemys.Values
            .Where(e => e.DataId is 17835).ToList();
        var usurperOfFrostList = TargetMgr.Instance.Enemys.Values
            .Where(e => e.DataId is 17833).ToList();
        
        // 返回 true，脚本停止运行
        // 返回 false，脚本每帧运行
        
        if (oracleOfDarknessList.Count != 1 && usurperOfFrostList.Count != 1)
        {
            return true;
        }
        return false;
    }
}