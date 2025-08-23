using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;

namespace ScriptTest;

public class CheckTargetInDotBlacklistScript : ITriggerScript
{
    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        var target = Core.Me.GetCurrTarget();

        if (target == null)
        {
            LogHelper.Print("当前没有选中目标。");
            return false;
        }

        uint dataId = target.DataId;

        if (BardBattleData.Instance.DotBlackList.Contains(dataId))
        {
            LogHelper.Print($"目标 ID {dataId} 在 DoT 黑名单中！");
        }
        else
        {
            LogHelper.Print($"目标 ID {dataId} 不在 DoT 黑名单中。");
        }

        return true; // 脚本执行完毕
    }
}