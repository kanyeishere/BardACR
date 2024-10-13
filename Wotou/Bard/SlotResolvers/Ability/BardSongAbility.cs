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
    private const uint FirstSong = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint SecondSong = BardDefinesData.Spells.MagesBallad;
    private const uint ThirdSong = BardDefinesData.Spells.ArmysPaeon;
    private const uint Peloton = BardDefinesData.Spells.Peloton;
    private const uint PerfectPitch = BardDefinesData.Spells.PitchPerfect;

    private static readonly float FirstSongDuration = BardSettings.Instance.WandererSongDuration * 1000;
    private static readonly float SecondSongDuration = BardSettings.Instance.MageSongDuration * 1000;
    private static readonly float ThirdSongDuration = BardSettings.Instance.ArmySongDuration * 1000;


    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        if (!FirstSong.IsReady() && !SecondSong.IsReady() && !ThirdSong.IsReady())
            return -1;
        if (FirstSong.RecentlyUsed() || SecondSong.RecentlyUsed() || ThirdSong.RecentlyUsed())
            return -1;


        if (FirstSong.IsReady() && GCDHelper.GetGCDCooldown() <= 530 && BardRotationEntry.QT.GetQt("爆发"))
            return 1;
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(FirstSong) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - FirstSongDuration && SecondSong.IsReady() &&
            GCDHelper.GetGCDCooldown() > 530)
            return 1;

        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(SecondSong) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - SecondSongDuration && ThirdSong.IsReady()&&
            (GCDHelper.GetGCDCooldown() > 530))
            return 1;

        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(ThirdSong) && 
            GCDHelper.GetGCDCooldown() <= 550 &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - ThirdSongDuration && FirstSong.IsReady())
            return 1;
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE && GCDHelper.GetGCDCooldown() > 530)
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        LogHelper.Print("Timer", $"ActiveSong: {Core.Resolve<JobApi_Bard>().ActiveSong}, SongTimer: {Core.Resolve<JobApi_Bard>().SongTimer}");
        if (spell.Id == SecondSong && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            slot.Add(PerfectPitch.GetSpell());
        slot.Add(spell);
    }
    
    private Spell GetSpell()
    {
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(ThirdSong) || BardBattleData.Instance.LastSong == GetSongBySpell(ThirdSong)) && FirstSong.IsReady())
            return FirstSong.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(FirstSong) || BardBattleData.Instance.LastSong == GetSongBySpell(FirstSong)) && SecondSong.IsReady())
            return SecondSong.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(SecondSong) || BardBattleData.Instance.LastSong == GetSongBySpell(SecondSong)) && ThirdSong.IsReady())
            return ThirdSong.GetSpell();
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE )
            return FirstSong.GetSpell();
        return FirstSong.GetSpell();
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