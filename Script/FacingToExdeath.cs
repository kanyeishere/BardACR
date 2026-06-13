using System;
using System.Globalization;
using System.Linq;
using AEAssist;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;
using AEAssist.Helper;

namespace ScriptTest;

public class FacingToExdeath : ITriggerScript
{
    // ============================================================

    // 脚本最多运行时间，单位：毫秒
    // 9000 = 9 秒
    private const long RunDurationMs = 8500;

    // 艾克斯迪司 DataId
    private const uint ExdeathDataId = 19509;
    
    // BuffId: 1602
    // Name: 混沌之风
    // 处理方式：需要背对艾克斯迪司
    private const uint WindBuff = 1602;

    // BuffId: 1603
    // Name: 混沌之逆风
    // 处理方式：需要面对艾克斯迪司
    private const uint ReverseWindBuff = 1603;

    // ============================================================

    private const double TwoPi = Math.PI * 2;

    // 记录脚本开始运行的时间
    // -1 表示还没有初始化
    private long _startTimeMs = -1;

    public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
    {
        var nowMs = Environment.TickCount64;

        // 第一次运行时，记录开始时间
        if (_startTimeMs < 0)
        {
            _startTimeMs = nowMs;
        }

        // 运行超过 9 秒后，返回 true，脚本结束
        if (nowMs - _startTimeMs >= RunDurationMs)
        {
            return true;
        }

        // 如果自己死了，直接结束脚本
        if (Core.Me.IsDead)
        {
            return true;
        }

        var exdeath = TargetMgr.Instance.Enemys.Values
            .FirstOrDefault(e => e.DataId == ExdeathDataId);
        
        if (exdeath == null)
        {
            return true;
        }
        
        var hasWind = Core.Me.HasAura(WindBuff);
        var hasReverseWind = Core.Me.HasAura(ReverseWindBuff);
        
        if (!hasWind && !hasReverseWind)
        {
            return true;
        }

        var myX = Core.Me.Position.X;
        var myZ = Core.Me.Position.Z;

        var exdeathX = exdeath.Position.X;
        var exdeathZ = exdeath.Position.Z;

        // 正对艾克斯迪司需要的面向弧度
        var faceToExdeath = GetFacingRadians(myX, myZ, exdeathX, exdeathZ);

        // 背对艾克斯迪司需要的面向弧度
        var faceAwayFromExdeath = NormalizeRadians(faceToExdeath + Math.PI);

        // 有逆风：面对艾克斯迪司
        // 有风：背对艾克斯迪司
        var targetFacing = hasReverseWind ? faceToExdeath : faceAwayFromExdeath;
        
        var facingText = targetFacing.ToString("F3", CultureInfo.InvariantCulture);

        // 每帧发送指令
        ECommons.Automation.Chat.Instance.SendMessage($"/pdrface chara {facingText}");
        
        // false = 继续每帧检测，直到超过 9 秒
        return false;
    }
    
    private static double GetFacingRadians(float fromX, float fromZ, float targetX, float targetZ)
    {
        var dx = targetX - fromX;
        var dz = targetZ - fromZ;

        var radians = Math.Atan2(dx, dz);

        return NormalizeRadians(radians);
    }
    
    private static double NormalizeRadians(double radians)
    {
        radians %= TwoPi;

        if (radians < 0)
        {
            radians += TwoPi;
        }

        return radians;
    }
}