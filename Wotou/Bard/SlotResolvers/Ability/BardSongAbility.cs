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

    private static readonly int FirstSongDuration = BardSettings.Instance.WandererSongDuration * 10000;
    private static readonly int SecondSongDuration = BardSettings.Instance.MageSongDuration * 10000;
    private static readonly int ThirdSongDuration = BardSettings.Instance.ArmySongDuration * 10000;


    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        if (!FirstSong.IsReady() && !SecondSong.IsReady() && !ThirdSong.IsReady())
            return -1;
        if (FirstSong.RecentlyUsed() || SecondSong.RecentlyUsed() || ThirdSong.RecentlyUsed())
            return -1;
        
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE && (FirstSong.IsReady() || SecondSong.IsReady() || ThirdSong.IsReady()))
            return FirstSong.RecentlyUsed(5500) || SecondSong.RecentlyUsed(5500) || ThirdSong.RecentlyUsed(5500) ? -1 : 1;
        
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(FirstSong) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45500.0 - FirstSongDuration && SecondSong.IsReady())
        {
            return 1;
        }
        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(SecondSong) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45500.0 - SecondSongDuration && ThirdSong.IsReady())
        {
            return 1;
        }
        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(ThirdSong) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45500.0 - ThirdSongDuration && FirstSong.IsReady())
        {
            return 1;
        }
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        if (spell == null) return;
        
        //非第一次唱旅神，gcd后半插入
        if (Core.Resolve<JobApi_Bard>().ActiveSong != Song.NONE)
        {
            if (spell.Id == FirstSong)
            {
                slot.Add2NdWindowAbility(spell);
                return;
            }
        }
        slot.Add(spell);
    }
    
    private Spell? GetSpell()
    {
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE )
        {
            if (FirstSong.IsReady())
                return FirstSong.GetSpell(SpellTargetType.Target);
            if (SecondSong.IsReady())
                return SecondSong.GetSpell(SpellTargetType.Target);
            if (ThirdSong.IsReady())
                return ThirdSong.GetSpell(SpellTargetType.Target);
        }
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(ThirdSong) || BardBattleData.Instance.LastSong == GetSongBySpell(ThirdSong)) && FirstSong.IsReady())
        {
            if (FirstSong.IsReady())
                return FirstSong.GetSpell(SpellTargetType.Target);
            if (SecondSong.IsReady())
                return SecondSong.GetSpell(SpellTargetType.Target);
        }
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(FirstSong) || BardBattleData.Instance.LastSong == GetSongBySpell(FirstSong)) && SecondSong.IsReady())
        {
            if (SecondSong.IsReady())
                return SecondSong.GetSpell(SpellTargetType.Target);
            if (ThirdSong.IsReady())
                return ThirdSong.GetSpell(SpellTargetType.Target);
        }
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(SecondSong) || BardBattleData.Instance.LastSong == GetSongBySpell(SecondSong)) && ThirdSong.IsReady())
        {
            if (ThirdSong.IsReady())
                return ThirdSong.GetSpell(SpellTargetType.Target);
            if (FirstSong.IsReady())
                return FirstSong.GetSpell(SpellTargetType.Target);
        }
        
        return null;
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