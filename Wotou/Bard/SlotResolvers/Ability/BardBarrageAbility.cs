using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardBarrageAbility : ISlotResolver
{
    private const uint Barrage = BardDefinesData.Spells.Barrage;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;   
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 620)
            return -1;
        if (!Barrage.GetSpell().IsReadyWithCanCast())
            return -1;
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 650 && BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) && EmpyrealArrow.IsUnlock())
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            RagingStrikes.IsUnlock())
            return -6;
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            BattleVoice.IsUnlock())
            return -7;
        if (BardUtil.HasAllPartyBuff())
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Barrage.GetSpell());
    }
}