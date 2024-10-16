using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardHeartBreakMaxChargeAbility : ISlotResolver
{
    private const uint HeartBreak = BardDefinesData.Spells.HeartBreak;
    private const uint RainOfDeath = BardDefinesData.Spells.RainofDeath;
    
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (Core.Resolve<MemApiSpell>().GetCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) >= Core.Resolve<MemApiSpell>().GetMaxCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) - 0.1)
            return 1;
        return -1;
    }

    private static Spell GetHeartBreakSpell()
    {
        if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1  && BardRotationEntry.QT.GetQt("AOE"))
            return Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).GetSpell();
        return Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell();
    }

    public void Build(Slot slot)
    {
        slot.Add(GetHeartBreakSpell());
    }
}