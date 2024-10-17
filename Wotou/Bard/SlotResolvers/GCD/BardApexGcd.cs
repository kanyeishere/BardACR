using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardApexGcd : ISlotResolver
{
    private const uint ApexArrow = BardDefinesData.Spells.ApexArrow;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;

    private static readonly bool GambleTripleApex = BardSettings.Instance.GambleTripleApex;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("绝峰箭"))
            return -1;
        
        // 非爆发期绝峰箭处理
        var partyBuffCountdown  = BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalSeconds;
        if (!Core.Me.HasLocalPlayerAura(BattleVoiceBuff) && !Core.Me.HasLocalPlayerAura(RagingStrikesBuff) &&
            !Core.Me.HasLocalPlayerAura(RadiantFinaleBuff))
        {
            if (GambleTripleApex)
            {
                if (Core.Resolve<JobApi_Bard>().SoulVoice >= 80 && partyBuffCountdown > 83)
                    return 1;
                if (Core.Resolve<JobApi_Bard>().SoulVoice >= 80 && partyBuffCountdown > 43)
                    return 1;
            }
            else
            {
                if (Core.Resolve<JobApi_Bard>().SoulVoice == 100 && 
                    partyBuffCountdown > 53 && 
                    !BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod)
                {
                    BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod = true;
                    return 1;
                }
                    
                if (Core.Resolve<JobApi_Bard>().SoulVoice >= 80 && 
                    partyBuffCountdown is > 43 and <= 53 &&
                    !BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod)
                {
                    BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod = true;
                    return 1;
                }
                // 非团辅期间，绝峰箭能量不足时的处理
                if (Core.Resolve<JobApi_Bard>().SoulVoice >= 40 && 
                    partyBuffCountdown is <= 43 and >= 39 &&
                    !BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod)
                {
                    BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod = true;
                    return 1;
                }
            }
        }
        
        // 爆发期绝峰箭处理
        if (Util.HasAllPartyBuff())
        {
            BardBattleData.Instance.HasUseApexArrowInCurrentNonBurstingPeriod = false;
            if (Core.Resolve<JobApi_Bard>().SoulVoice == 100)
                return 1;
            if (Core.Resolve<JobApi_Bard>().SoulVoice >= 80)
                return Core.Me.HasAura(RagingStrikesBuff, 8000) ? -1 : 1;
        }
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(ApexArrow.GetSpell());
    }
}