using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardHeartBreakAbility : ISlotResolver
{
    private const uint Bloodletter = BardDefinesData.Spells.Bloodletter;
    private const uint HeartBreak = BardDefinesData.Spells.HeartBreak;
    private const uint RainOfDeath = BardDefinesData.Spells.RainofDeath;
    
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    public int Check()
    {
        var spell = Bloodletter;
        if (HeartBreak.IsUnlock())
            spell = HeartBreak;
        
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!spell.IsReady())
            return -1;
        if (Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).RecentlyUsed(2300))
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (EmpyrealArrow.RecentlyUsed())
            return -1;
        if (Core.Resolve<MemApiSpell>().GetCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) >= Core.Resolve<MemApiSpell>().GetMaxCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) - 0.1)
            return 1;
        if (BardRotationEntry.QT.GetQt("攒碎心箭") && Util.PartyBuffWillBeReadyIn(28000))
            return -1;
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3)
            return -1;
        return 1;
    }

    public void Build(Slot slot)
    {
        if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1)
            slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).GetSpell());
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell());
    }
}