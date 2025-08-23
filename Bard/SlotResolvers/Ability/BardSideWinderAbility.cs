using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardSideWinderAbility: ISlotResolver
{
    private const uint SideWinder = BardDefinesData.Spells.Sidewinder;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt(QTKey.Sidewinder))
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -2;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 650 && BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) && EmpyrealArrow.IsUnlock())
            return -3;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            RagingStrikes.IsUnlock())
            return -6;
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            BattleVoice.IsUnlock())
            return -7;
        if (!SideWinder.GetSpell().IsReadyWithCanCast())
            return -4;
        return 1;
    }

    public void Build(Slot slot) => slot.Add(SideWinder.GetSpell());
}