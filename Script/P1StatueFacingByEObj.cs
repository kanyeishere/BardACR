using System;
using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;

namespace ScriptTest;

public class P1StatueFacingByEObj : ITriggerScript
{
    // ============================================================
    // EObjAnimation 参数
    // 对应日志：
    // BaseId: 2015166 / 2015167
    // arg1: 64
    // arg2: 128
    // ============================================================

    private const uint Arg1 = 64;
    private const uint Arg2 = 128;

    // 2015166 = 背对 = 面向南方
    private const uint BackFacingBaseId = 2015166;

    // 2015167 = 正对 = 面向北方
    private const uint FrontFacingBaseId = 2015167;

    // 先载入面向模块
    private const string LoadFaceModuleCommand = "/pdr load AutoFaceCameraDirection";

    // 命中后等待多久再设置面向
    // 1000 = 1 秒
    private const long DelayAfterLoadMs = 1000;

    // ============================================================
    // 内部状态，不需要用户改
    // ============================================================

    private bool _hasLoadedFaceModule = false;
    private long _loadFaceModuleTimeMs = -1;
    private string _pendingFacingCommand = "";

    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        var nowMs = Environment.TickCount64;

        // ========================================================
        // 第二阶段：
        // 已经发送过 /pdr load AutoFaceCameraDirection
        // 等待 1 秒后，再发送真正的面向指令
        // ========================================================
        if (_hasLoadedFaceModule)
        {
            if (nowMs - _loadFaceModuleTimeMs < DelayAfterLoadMs)
            {
                return false;
            }

            ECommons.Automation.Chat.Instance.SendMessage(_pendingFacingCommand);

            // true = 面向设置完成，脚本结束
            return true;
        }

        // ========================================================
        // 第一阶段：
        // 监听 EObjAnimation 事件
        // ========================================================

        // 必须是 EObjAnimation 事件才处理
        if (condParams is not AEAssist.CombatRoutine.Trigger.EObjAnimationCondParams eobj)
        {
            return false;
        }

        // 对应日志里的：
        // arg1: 64
        // arg2: 128
        if (eobj.arg1 != Arg1 || eobj.arg2 != Arg2)
        {
            return false;
        }

        // BaseId 2015166：背对，也就是面向南方
        if (eobj.BaseId == BackFacingBaseId)
        {
            _pendingFacingCommand = "/pdrface ground south";
        }
        // BaseId 2015167：正对，也就是面向北方
        else if (eobj.BaseId == FrontFacingBaseId)
        {
            _pendingFacingCommand = "/pdrface ground north";
        }
        else
        {
            return false;
        }

        // 命中机制后，先载入面向模块
        ECommons.Automation.Chat.Instance.SendMessage(LoadFaceModuleCommand);

        _hasLoadedFaceModule = true;
        _loadFaceModuleTimeMs = nowMs;

        // false = 不结束，继续运行，等 1 秒后设置面向
        return false;
    }
}