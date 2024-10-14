using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardSongAbility : ISlotResolver
{
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint MagesBallad = BardDefinesData.Spells.MagesBallad;
    private const uint ArmysPaeon = BardDefinesData.Spells.ArmysPaeon;
    private const uint Peloton = BardDefinesData.Spells.Peloton;
    private const uint PerfectPitch = BardDefinesData.Spells.PitchPerfect;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;

    private static readonly float WandererSongDuration = BardSettings.Instance.WandererSongDuration * 1000;
    private static readonly float MageSongDuration = BardSettings.Instance.MageSongDuration * 1000;
    private static readonly float ArmySongDuration = BardSettings.Instance.ArmySongDuration * 1000;


    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (!WanderersMinuet.IsReady() && !MagesBallad.IsReady() && !ArmysPaeon.IsReady())
            return -1;
        if (WanderersMinuet.RecentlyUsed() || MagesBallad.RecentlyUsed() || ArmysPaeon.RecentlyUsed())
            return -1;


        if (WanderersMinuet.IsReady() && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererGcdTime && 
            BardRotationEntry.QT.GetQt("爆发") && 
            RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < GCDHelper.GetGCDDuration())
            return 1;
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(WanderersMinuet) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - WandererSongDuration &&
            MagesBallad.IsReady() && 
            //Core.Resolve<JobApi_Bard>().Repertoire == 0 &&
            GCDHelper.GetGCDCooldown() > 530)
            return 1;
        
        /*if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(FirstSong) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45450.0 - FirstSongDuration && SecondSong.IsReady() && 
            Core.Resolve<JobApi_Bard>().Repertoire >= 1 &&
            GCDHelper.GetGCDCooldown() > 1200)
            return 1;*/

        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(MagesBallad) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - MageSongDuration &&
            ArmysPaeon.IsReady()&&
            (GCDHelper.GetGCDCooldown() > 530))
            return 1;

        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(ArmysPaeon) && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererGcdTime &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - ArmySongDuration &&
            WanderersMinuet.IsReady() && 
            RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < GCDHelper.GetGCDDuration())
            return 1;
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE && GCDHelper.GetGCDCooldown() > 530)
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        LogHelper.Print("Timer", $"ActiveSong: {Core.Resolve<JobApi_Bard>().ActiveSong}, SongTimer: {Core.Resolve<JobApi_Bard>().SongTimer}");
        /*if (spell.Id == SecondSong && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            slot.Add(PerfectPitch.GetSpell());*/
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