using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardSongMaxAbility : ISlotResolver
{
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint MagesBallad = BardDefinesData.Spells.MagesBallad;
    private const uint ArmysPaeon = BardDefinesData.Spells.ArmysPaeon;
    
    // 仅当身上没有歌时，且GCD大于530ms时，才会唱歌
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        if (!WanderersMinuet.IsReady() && !MagesBallad.IsReady() && !ArmysPaeon.IsReady())
            return -1;
        if (WanderersMinuet.RecentlyUsed() || MagesBallad.RecentlyUsed() || ArmysPaeon.RecentlyUsed())
            return -1;
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE && GCDHelper.GetGCDCooldown() > 530)
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        LogHelper.Print("Timer", $"ActiveSong: {Core.Resolve<JobApi_Bard>().ActiveSong}, SongTimer: {Core.Resolve<JobApi_Bard>().SongTimer}");
        slot.Add(spell);
    }
    
    private Spell GetSpell()
    {
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(ArmysPaeon) || BardBattleData.Instance.LastSong == GetSongBySpell(ArmysPaeon)) && WanderersMinuet.IsReady())
            return WanderersMinuet.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(WanderersMinuet) || BardBattleData.Instance.LastSong == GetSongBySpell(WanderersMinuet)) && MagesBallad.IsReady())
            return MagesBallad.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(MagesBallad) || BardBattleData.Instance.LastSong == GetSongBySpell(MagesBallad)) && ArmysPaeon.IsReady())
            return ArmysPaeon.GetSpell();
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE )
            if (WanderersMinuet.IsReady())
                return WanderersMinuet.GetSpell();
            else if (MagesBallad.IsReady())
                return MagesBallad.GetSpell();
            else if (ArmysPaeon.IsReady())
                return ArmysPaeon.GetSpell();
        return WanderersMinuet.GetSpell();
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
}