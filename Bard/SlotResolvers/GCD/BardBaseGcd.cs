using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardBaseGcd : ISlotResolver
{
    private const uint BurstShot = BardDefinesData.Spells.BurstShot;
    private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
    private const uint Ladonsbite = BardDefinesData.Spells.Ladonsbite;
    private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;
    
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check()
    {
        var gcdAnimationTime = BardSettings.Instance.GcdAnimationTime;
        // 非团辅期间，高难模式下，九天不延后，延后gcd
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 400 &&
            BardRotationEntry.QT.GetQt("强对齐") &&
            BardUtil.HasNoPartyBuff() &&
            EmpyrealArrow.IsUnlock() &&
            BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) &&
            !BardSettings.Instance.IsDailyMode)
            return -1;
        
        // 新版强对齐
        if (GCDHelper.GetGCDDuration() > 2100) //军神结束后重置
        {
            BardBattleData.Instance.TotalStopTime = 0;
            BardBattleData.Instance.GcdCountDown = 3;
        }
        
        const int gcdDuration = 2080; // 每个 GCD 的持续时间（毫秒）

        // 计算未来 3 个 GCD 后的时间点
        int futureTime = gcdDuration * BardBattleData.Instance.GcdCountDown; // 3 个 GCD 后的时间（毫秒）
        double futureFirstBuffCooldown = BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds - futureTime;
        double futureEmpyrealArrowCooldown = EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds - futureTime;

        // 检查未来条件是否满足
        bool futureConditionsMet = BardRotationEntry.QT.GetQt("爆发") &&
                                   BardRotationEntry.QT.GetQt("对齐旅神") &&
                                   BardRotationEntry.QT.GetQt("强对齐") &&
                                   BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) &&
                                   Core.Resolve<JobApi_Bard>().ActiveSong == Song.Army &&
                                   futureFirstBuffCooldown < 2200 + 2020 * 3 + gcdAnimationTime &&
                                   futureEmpyrealArrowCooldown >= 2200 * 2 + 2080 + gcdAnimationTime &&
                                   futureEmpyrealArrowCooldown < 2200 * 2 + 2080 * 2 + gcdAnimationTime;

        if (futureConditionsMet && BardBattleData.Instance.TotalStopTime == 0)
        {
            BardBattleData.Instance.TotalStopTime = futureEmpyrealArrowCooldown - (2200 * 2 + 2080 + gcdAnimationTime) - 150;
        }
        if (BardBattleData.Instance.TotalStopTime > 0 && 
            BardBattleData.Instance.GcdCountDown <= 3)
        {
            var currentGcdIndex = BardBattleData.Instance.GcdCountDown + 2;
            var empyrealArrowCooldown = EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds;
            BardBattleData.Instance.RestStopTime = (empyrealArrowCooldown - 2200 * 2 - gcdAnimationTime) % 2080 - 150;
            var conditionsMet = BardBattleData.Instance.RestStopTime  >=
                                (BardBattleData.Instance.TotalStopTime) * (currentGcdIndex - 1)/5 &&
                                BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds < 15000;
            if (conditionsMet)
            {
                BardUtil.LogDebug("TotalStopTime","TotalStopTime: " + BardBattleData.Instance.TotalStopTime);
                BardUtil.LogDebug("RestStopTime","RestStopTime: " + BardBattleData.Instance.RestStopTime);
                return -1;
            }
        }
        
        // 当前歌曲为军神 且开启爆发 且开启爆发对齐旅神 且强对齐 且第一个开的120秒buffCD时间小于2200 + 2020 * 3  + GCD动画时间，
        // 且九天CD时间大于约7秒，停手
        /*if (BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") && 
            BardRotationEntry.QT.GetQt("强对齐") &&
            BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) &&
            Core.Resolve<JobApi_Bard>().ActiveSong == Song.Army &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <
            2200 + 2020 * 3  + gcdAnimationTime )
        {
            if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds >= 2200 * 2 + 2080 + gcdAnimationTime &&
                EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 2200 * 2 + 2080 * 2 + gcdAnimationTime )
                return -1;
        }*/
        
        return 1; // 正常继续攻击
    }
    
    
    
    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(GetBaseGcd());
    }

    private static Spell GetBaseGcd()
    {
        return BardUtil.GetBaseGcd();
    }
}