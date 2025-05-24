using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;

namespace ScriptTest;

public class IsDeadScript : ITriggerScript
{
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        if (Core.Me.IsDead)
        {
            ECommons.Automation.Chat.Instance.SendMessage($"/pdr tts 你好菜啊，又死了！");
            return true;
        }
        return false;
    }
}