using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardApexMaxGcd : ISlotResolver
{
    private const uint ApexArrow = BardDefinesData.Spells.ApexArrow;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("绝峰箭"))
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        
        // 本文件只处理满能量的绝峰箭
        if (BardUtil.HasAllPartyBuff() &&
            Core.Resolve<JobApi_Bard>().SoulVoice == 100)
            return 1;
        
        var partyBuffCountdown  = BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalSeconds;
        if (!BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod &&
            Core.Resolve<JobApi_Bard>().SoulVoice == 100 && 
            partyBuffCountdown >= 43)
            return 1;
        
        // 或者爆发buff本地还未生效，但是确实已经使用过buff技能了
        if (RagingStrikes.RecentlyUsed(10000) &&
            BattleVoice.RecentlyUsed(10000) &&
            Core.Resolve<JobApi_Bard>().SoulVoice == 100)
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(ApexArrow.GetSpell());
    }
}