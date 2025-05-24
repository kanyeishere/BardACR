using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou;

namespace ScriptTest;

public class CheckCurrJobPotion : ITriggerScript
{
    // Check返回True代表此节点执行完毕， 非检测模式下，返回false时一直重复执行（每帧）
    // 检测模式下， condParams必定为空
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        if (Core.Me.CurrentJob() == Jobs.Bard)
            return ItemHelper.CheckCurrJobPotion() && Wotou.Bard.BardRotationEntry.QT.GetQt(Wotou.Bard.QTKey.UsePotion);
        if (Core.Me.CurrentJob() == Jobs.Dancer)
            return ItemHelper.CheckCurrJobPotion() && Wotou.Dancer.DancerRotationEntry.QT.GetQt(Wotou.Dancer.QTKey.UsePotion);
        return false;
    }
}