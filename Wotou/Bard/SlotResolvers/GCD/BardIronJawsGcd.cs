using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;

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
        if (target.HasMyAuraWithTimeleft(CausticBiteDot, 3000) && target.HasMyAuraWithTimeleft(StormBiteDot, 3000))
            return -1;
        
        return 0;
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