using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardRadiantEncoreGcd : ISlotResolver
{
    private const uint RadiantEncore = BardDefinesData.Spells.RadiantEncore;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    
    private const uint RadiantEncoreReady = BardDefinesData.Buffs.RadiantEncoreReady;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        
        if (Core.Me.HasLocalPlayerAura(RadiantEncoreReady) && !Core.Me.HasMyAuraWithTimeleft(RadiantEncoreReady, 3000))
            return 1;

        /*if (BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            RadiantEncore.GetSpell().IsReadyWithCanCast())
            return 1;*/
        
        if (BardBattleData.Instance.First120SBuffSpellId == RagingStrikes && 
            Core.Me.HasLocalPlayerAura(RadiantEncoreReady))
            return 1;
                
        /*if (BardUtil.HasAllPartyBuff() &&
            RadiantEncore.GetSpell().IsReadyWithCanCast())
            return 1;*/
        
        /*if (BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            RagingStrikes.RecentlyUsed(18000) &&
            RadiantEncore.GetSpell().IsReadyWithCanCast())
            return 1;*/
        
        if (BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            RagingStrikes.RecentlyUsed(18000)  &&
            Core.Me.HasLocalPlayerAura(RadiantEncoreReady))
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(RadiantEncore.GetSpell());
    }
}