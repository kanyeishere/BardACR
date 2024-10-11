using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardDotGcd : ISlotResolver
{
    
    private static readonly uint[] 毒dotBuffs = [BardDefinesData.Buffs.VenomousBite, BardDefinesData.Buffs.CausticBite];
    private static readonly uint[] 风dotBuffs = [BardDefinesData.Buffs.Windbite, BardDefinesData.Buffs.Stormbite];
    private const uint VenomousBite = BardDefinesData.Spells.VenomousBite;
    private const uint WindBite = BardDefinesData.Spells.Windbite;

    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("DOT"))
            return -1;
        var target = Core.Me.GetCurrTarget();
        if (target == null)
            return -1;
        if (HasAnyDot(target, 风dotBuffs) && HasAnyDot(target, 毒dotBuffs))
            return -1;
        if (DotBlacklistHelper.IsBlackList(target))
            return -1;
        return 0;
    }
    
    public void Build(Slot slot)
    {
        slot.Add(this.GetSpell());
    }
    
    private bool HasAnyDot(IBattleChara? target, uint[] dots)
    {
        return dots.Any(target.HasLocalPlayerAura);
    }
    
    private Spell GetSpell()
    {
        var target = Core.Me.GetCurrTarget();
        return !HasAnyDot(target, 风dotBuffs) ? WindBite.GetSpell() : VenomousBite.GetSpell();
    }
}