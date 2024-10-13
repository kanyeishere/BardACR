using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardHeartBreakAbility : ISlotResolver
{
    private const uint Bloodletter = BardDefinesData.Spells.Bloodletter;
    private const uint HeartBreak = BardDefinesData.Spells.HeartBreak;
    private const uint RainOfDeath = BardDefinesData.Spells.RainofDeath;
    private const uint FirstSong = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint SecondSong = BardDefinesData.Spells.MagesBallad;
    
    private static readonly float FirstSongDuration = BardSettings.Instance.WandererSongDuration * 1000;

    
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
        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(FirstSong) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - FirstSongDuration && SecondSong.IsReady())
            return -100;
        if (EmpyrealArrow.RecentlyUsed())
            return -1;
        if (Core.Resolve<MemApiSpell>().GetCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) >= Core.Resolve<MemApiSpell>().GetMaxCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) - 0.1)
            return 1;
        if (BardRotationEntry.QT.GetQt("攒碎心箭") && Util.PartyBuffWillBeReadyIn(28000))
            return -1;
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3 && Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(FirstSong))
            return -1;
        return 1;
    }

    private static Song GetSongBySpell(uint song)
    {
        return song switch
        {
            BardDefinesData.Spells.TheWanderersMinuet => Song.WANDERER,
            BardDefinesData.Spells.MagesBallad => Song.MAGE,
            BardDefinesData.Spells.ArmysPaeon => Song.ARMY,
            _ => throw new ArgumentOutOfRangeException(nameof(song), song, null)
        };
    }

    public void Build(Slot slot)
    {
        if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1  && BardRotationEntry.QT.GetQt("AOE"))
        {
            slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).GetSpell());
            return;
        }
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell());
    }
}