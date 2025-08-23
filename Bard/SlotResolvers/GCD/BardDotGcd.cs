using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardDotGcd : ISlotResolver
{
    
    private static readonly uint[] 毒dotBuffs = [BardDefinesData.Buffs.VenomousBite, BardDefinesData.Buffs.CausticBite];
    private static readonly uint[] 风dotBuffs = [BardDefinesData.Buffs.Windbite, BardDefinesData.Buffs.Stormbite];
    private const uint VenomousBite = BardDefinesData.Spells.VenomousBite;
    private const uint WindBite = BardDefinesData.Spells.Windbite;
    private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
    private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("DOT"))
            return -1;
        var target = Core.Me.GetCurrTarget();
        if (target == null)
            return -1;
        if (DotBlacklistHelper.IsBlackList(target))
            return -1;
        if (BardBattleData.Instance.DotBlackList.Contains(target.DataId))
            return -1;
        if (!TargetHelper.IsBoss(target) && !BardSettings.Instance.ApplyDotOnTrashMobs && BardSettings.Instance.IsDailyMode)
            return -1;
        /*if (Core.Me.HasAura(HawkEyeBuff) || Core.Me.HasAura(BarrageBuff))
            return -25;*/
        if (!HasAnyDot(target, 风dotBuffs) && WindBite.IsUnlock())
            return 1;
        if (!HasAnyDot(target, 毒dotBuffs) && VenomousBite.IsUnlock())
            return 2;
        return -10;
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
        if (BardRotationEntry.QT.GetQt(QTKey.ClearHawkEyesBuffBeforeDots))
        {
            if (Core.Me.HasAura(HawkEyeBuff) &&
                !RagingStrikes.IsUnlockWithCDCheck() && 
                !BattleVoice.IsUnlockWithCDCheck())
            {
                if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25,5) > 1 && 
                    BardRotationEntry.QT.GetQt("AOE") && 
                    Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).IsUnlock())
                    return Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).GetSpell();
                return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
            }
        
            if (Core.Me.HasAura(HawkEyeBuff) &&
                !BardRotationEntry.QT.GetQt(QTKey.Burst))
            {
                if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25,5) > 1 && 
                    BardRotationEntry.QT.GetQt("AOE") && 
                    Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).IsUnlock())
                    return Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).GetSpell();
                return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
            }
        }
        
        var target = Core.Me.GetCurrTarget();
        return !HasAnyDot(target, 风dotBuffs) && WindBite.IsUnlock()? 
            Core.Resolve<MemApiSpell>().CheckActionChange(WindBite).GetSpell() : 
            Core.Resolve<MemApiSpell>().CheckActionChange(VenomousBite).GetSpell();
    }
}