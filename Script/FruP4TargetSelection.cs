using AEAssist;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;

namespace ScriptTest;

public class FruP4TargetSelection: ITriggerScript
{
    
    public uint StormBiteBuff = 1201;
    public uint CausticBiteBuff = 1200;
    
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        var oracleOfDarknessList = TargetMgr.Instance.Enemys.Values
            .Where(e => e.DataId is 17835).ToList();
        var usurperOfFrostList = TargetMgr.Instance.Enemys.Values
            .Where(e => e.DataId is 17833).ToList();
        
        // 返回 true，脚本停止运行
        // 返回 false，脚本每帧运行
        
        if (oracleOfDarknessList.Count != 1 || usurperOfFrostList.Count != 1)
        {
            return false;
        }
        
        var oracleOfDarkness = oracleOfDarknessList[0];
        var usurperOfFrost = usurperOfFrostList[0];
        
        // 检测 oracle of darkness 身上的 风dot 是否小于5s
        if (oracleOfDarkness.HasLocalPlayerAura(StormBiteBuff) && !oracleOfDarkness.HasMyAuraWithTimeleft(StormBiteBuff, 5000))
        {
            Core.Me.SetTarget(oracleOfDarkness);
            return false;
        }
        
        // 检测 oracle of darkness 身上的 毒dot 是否小于5s
        if (oracleOfDarkness.HasLocalPlayerAura(CausticBiteBuff) && !oracleOfDarkness.HasMyAuraWithTimeleft(CausticBiteBuff, 5000))
        {
            Core.Me.SetTarget(oracleOfDarkness);
            return false;
        }
        
        // 检测 usurper of frost 身上的 风dot 是否小于5s
        if (usurperOfFrost.HasLocalPlayerAura(StormBiteBuff) && !usurperOfFrost.HasMyAuraWithTimeleft(StormBiteBuff, 5000))
        {
            Core.Me.SetTarget(usurperOfFrost);
            return false;
        }
        
        // 检测 usurper of frost 身上的 毒dot 是否小于5s
        if (usurperOfFrost.HasLocalPlayerAura(CausticBiteBuff) && !usurperOfFrost.HasMyAuraWithTimeleft(CausticBiteBuff, 5000))
        {
            Core.Me.SetTarget(usurperOfFrost);
            return false;
        }
        
        //检测血量百分比的差值
        var hpDiff  = (oracleOfDarkness.CurrentHpPercent() - usurperOfFrost.CurrentHpPercent()) * 100;
        
        // 血量差 > 8% 关AOE 并且 切目标
        if (hpDiff >= 8)
        {
            Core.Me.SetTarget(oracleOfDarkness);
            // 关 AOE
            Wotou.Bard.BardRotationEntry.QT.SetQt("AOE", false);
            return false;
        }
        else if (hpDiff <= -8)
        {
            Core.Me.SetTarget(usurperOfFrost);
            // 关 AOE
            Wotou.Bard.BardRotationEntry.QT.SetQt("AOE", false);
            return false;
        }
        
        // 血量差 < 5% 开AOE
        else if (hpDiff is >= -5 and <= 5)
        {
            // 开 AOE
            Wotou.Bard.BardRotationEntry.QT.SetQt("AOE", true);   
        }
        
        
        // 当 oracleOfDarkness 血量比 usurperOfFrost 高 1.5%，切目标
        if (hpDiff >= 1.5)
        {
            Core.Me.SetTarget(oracleOfDarkness);
            return false;
        }
        
        // 当 usurperOfFrost 血量比 oracleOfDarkness 高 1.5%，切目标
        if (hpDiff <= -1.5)
        {
            Core.Me.SetTarget(usurperOfFrost);
            return false;
        }
        return false;
    }
}