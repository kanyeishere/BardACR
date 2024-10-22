using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardIronJawsGcd : ISlotResolver
{
    private const uint IronJaws = BardDefinesData.Spells.IronJaws;
    
    private const uint CausticBiteDot = BardDefinesData.Buffs.CausticBite;
    private const uint StormBiteDot = BardDefinesData.Buffs.Stormbite;
    private const uint VenomousBiteDot = BardDefinesData.Buffs.VenomousBite;
    private const uint WindBiteDot = BardDefinesData.Buffs.Windbite;
    
    private static readonly uint[] 毒dotBuffs = [VenomousBiteDot, CausticBiteDot];
    private static readonly uint[] 风dotBuffs = [WindBiteDot, StormBiteDot];
    
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;
    private const uint HawksEyeBuff = BardDefinesData.Buffs.HawksEye;
    
    public int Check()
    {
        if (IronJaws.RecentlyUsed(2500))
            return -1;
        if (!BardRotationEntry.QT.GetQt("DOT"))
            return -1;
        var target = Core.Me.GetCurrTarget();
        if (target == null)
            return -1;
        if (!HasAnyDot(target, 风dotBuffs) || !HasAnyDot(target, 毒dotBuffs))
            return -1;
        
        // 爆发期截毒
        if (BardUtil.HasAllPartyBuff() && !BardBattleData.Instance.HasUseIronJawsInCurrentBursting)
        {
            // 只剩3秒的时，强制截
            if (!Core.Me.HasMyAuraWithTimeleft(BattleVoiceBuff, 3000) ||
                !Core.Me.HasMyAuraWithTimeleft(RagingStrikesBuff, 3000) ||
                !Core.Me.HasMyAuraWithTimeleft(RadiantFinaleBuff, 3000))
            {
                BardBattleData.Instance.HasUseIronJawsInCurrentBursting = true;
                return 1;
            }
            
            //  如果还剩10秒，且没有鹰眼buff，截毒           
            if (!Core.Me.HasMyAuraWithTimeleft(BattleVoiceBuff, 10000) || !Core.Me.HasMyAuraWithTimeleft(RagingStrikesBuff, 10000) || !Core.Me.HasMyAuraWithTimeleft(RadiantFinaleBuff, 10000))
            {
                if (Core.Me.HasLocalPlayerAura(HawksEyeBuff))
                    return -1;
                BardBattleData.Instance.HasUseIronJawsInCurrentBursting = true;
                return 1;
            }
            return -1;
        }
        
        // 非爆发期续毒
        if (target.HasMyAuraWithTimeleft(CausticBiteDot, 5500) && target.HasMyAuraWithTimeleft(StormBiteDot, 5500))
            return -1;
        if (!Core.Me.HasLocalPlayerAura(HawksEyeBuff))
            return 1;
        
        if (target.HasMyAuraWithTimeleft(CausticBiteDot, 3000) && target.HasMyAuraWithTimeleft(StormBiteDot, 3000))
            return -1;
        return 1;
    }
    
    private static bool HasAnyDot(IBattleChara? target, uint[] dots)
    {
        return dots.Any(target.HasLocalPlayerAura);
    }

    public void Build(Slot slot)
    {
        slot.Add(IronJaws.GetSpell());
    }
}