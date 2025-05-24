using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;

namespace ScriptTest;

public class FruP4SetRotPart3: ITriggerScript
{
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        ECommons.Automation.Chat.Instance.SendMessage($"/pdrface off");
        ECommons.Automation.Chat.Instance.SendMessage($"/pdr unload AutoFaceCameraDirection");
        return true;
    }
}