using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;
using AEAssist.Helper;

namespace ScriptTest;

public class ExdeathChaosHpSumCheck : ITriggerScript
{

    private readonly double _hpSumThresholdPercent = AI.Instance.ExposeVarsGetValueOrDefault("P3关爆发阈值") == 0 ?
        // ============================================================
        // 血量百分比之和阈值的默认值（也就说导出变量设置为 0 时） =>
        10
        // 例如：
        // 10  = Exdeath + Chaos 血量百分比之和小于 10% 时触发
        // 20  = Exdeath + Chaos 血量百分比之和小于 20% 时触发
        // 5   = Exdeath + Chaos 血量百分比之和小于 5% 时触发
        // ============================================================
        : AI.Instance.ExposeVarsGetValueOrDefault("P3关爆发阈值");
    
    private const uint ExdeathDataId = 19509;
    
    private const uint ChaosDataId = 19508;


    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        var exdeath = TargetMgr.Instance.Enemys.Values
            .FirstOrDefault(e => e.DataId == ExdeathDataId);

        var chaos = TargetMgr.Instance.Enemys.Values
            .FirstOrDefault(e => e.DataId == ChaosDataId);

        // 如果没有同时找到 Exdeath 和 Chaos，则 True
        if (exdeath == null && chaos == null)
        {
            return true;
        }

        var hpSumPercent = 0f;
        
        // 如果找到了 Exdeath，就把 Exdeath 的血量百分比加进去
        if (exdeath != null)
        {
            hpSumPercent += exdeath.CurrentHpPercent() * 100;
        }

        // 如果找到了 Chaos，就把 Chaos 的血量百分比加进去
        if (chaos != null)
        {
            hpSumPercent += chaos.CurrentHpPercent() * 100;
        }
        
        var result = hpSumPercent < _hpSumThresholdPercent;
        
        ChatHelper.SendMessage(
            $"/e [血量判断] 血量: {hpSumPercent:F1}% / 阈值: {_hpSumThresholdPercent}%"
        );
        
        ChatHelper.SendMessage(
            $"/e [血量判断] 结果: {(result ? "小于阈值，关闭爆发 <se.1>" : "未小于阈值，开启爆发 <se.2>")}"
        );
        
        return hpSumPercent < _hpSumThresholdPercent;
    }
}