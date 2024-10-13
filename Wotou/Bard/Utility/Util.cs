using AEAssist;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.Utility;

public class Util
{
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;
    
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    
    private static bool IsRadiantFinaleConditionMet()
    {
        // 检查技能是否已解锁：如果未解锁，直接返回true。如果已解锁，检查是否有Radiant Finale的Buff效果
        return !RadiantFinale.IsUnlock() || Core.Me.HasLocalPlayerAura(RadiantFinaleBuff);
    }
    
    public static bool HasAllPartyBuff()
    {
        return Core.Me.HasAura(BattleVoiceBuff) && Core.Me.HasAura(RagingStrikesBuff) && IsRadiantFinaleConditionMet();
    }
    
    public static bool HasAnyPartyBuff()
    {
        return Core.Me.HasAura(BattleVoiceBuff) || Core.Me.HasAura(RagingStrikesBuff) || Core.Me.HasAura(RadiantFinaleBuff);
    }
    
    public static bool HasNoPartyBuff()
    {
        return !Core.Me.HasAura(BattleVoiceBuff) && !Core.Me.HasAura(RagingStrikesBuff) && !Core.Me.HasAura(RadiantFinaleBuff);
    }
    
    public static bool PartyBuffWillBeReadyIn(int ms)
    {
        return BattleVoice.GetSpell().Cooldown.TotalMilliseconds <= ms || RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <= ms;
    }
    
}