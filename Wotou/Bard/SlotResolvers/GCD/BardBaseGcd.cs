using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
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
        const int gcdAnimationTime = 620;
        // 非团辅期间，九天不延后，延后gcd
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < gcdAnimationTime &&
            //BardRotationEntry.QT.GetQt("强对齐") &&
            BardUtil.HasNoPartyBuff())
            return -1;
        
        // 当前歌曲为军神 且开启爆发 且开启爆发对齐旅神 且强对齐 且第一个开的120秒buffCD时间小于2200 + 2020 * 3  + GCD动画时间，
        // 且九天CD时间大于约7秒，停手
        if (BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") && 
            BardRotationEntry.QT.GetQt("强对齐") &&
            Core.Resolve<JobApi_Bard>().ActiveSong == Song.ARMY &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <
            2200 + 2020 * 3  + gcdAnimationTime )
        {
            if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds >= 2200 * 2 + 2100 + gcdAnimationTime &&
                EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 2200 * 2 + 2080 * 2 + gcdAnimationTime )
                return -1;
        }
        return 1;
    }
    
    
    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(GetBaseGcd());
    }

    private static Spell GetBaseGcd()
    {
        if (Core.Me.HasAura(HawkEyeBuff) || Core.Me.HasAura(BarrageBuff))
        {
            if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25,5) > 1 && BardRotationEntry.QT.GetQt("AOE"))
                return Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).GetSpell();
            return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
        }
        
        if (TargetHelper.GetEnemyCountInsideSector(Core.Me, Core.Me.GetCurrTarget(), 12, 90) > 1  && BardRotationEntry.QT.GetQt("AOE"))
            return Core.Resolve<MemApiSpell>().CheckActionChange(Ladonsbite).GetSpell();
        return Core.Resolve<MemApiSpell>().CheckActionChange(BurstShot).GetSpell();
    }
}