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
    private const uint PerfectPitch = BardDefinesData.Spells.PitchPerfect;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    public int Check()
    {
        var wandererSongDuration = BardSettings.Instance.WandererSongDuration * 1000;
        var mageSongDuration = BardSettings.Instance.MageSongDuration * 1000;
        var armySongDuration = BardSettings.Instance.ArmySongDuration * 1000;
        
        // 此文件只处理歌曲顺序为 旅神-贤者-军神 的正常循环情况
        if (!BardSettings.Instance.IsSongOrderNormal())
            return -999;
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (!WanderersMinuet.IsReady() && !MagesBallad.IsReady() && !ArmysPaeon.IsReady())
            return -1;
        if (WanderersMinuet.RecentlyUsed() || MagesBallad.RecentlyUsed() || ArmysPaeon.RecentlyUsed())
            return -1;
        
        if (WanderersMinuet.IsReady() && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime && 
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 &&
            BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 * 2)
        {
            if (BardRotationEntry.QT.GetQt("Debug"))
                LogHelper.Print("切歌", "第一个开的120秒buff是战斗之声，爆发切歌条件满足");
            return 120;
        }
        
        if (WanderersMinuet.IsReady() && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime && 
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200
            )
        {
            if (BardRotationEntry.QT.GetQt("Debug"))
                LogHelper.Print("切歌", "第一个开的120秒buff是猛者强击，爆发切歌条件满足");
            return 120;
        }
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(WanderersMinuet) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - wandererSongDuration &&
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
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - mageSongDuration &&
            ArmysPaeon.IsReady()&&
            (GCDHelper.GetGCDCooldown() > 530))
            return 1;

        if (Core.Resolve<JobApi_Bard>().ActiveSong == GetSongBySpell(ArmysPaeon) && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - armySongDuration &&
            WanderersMinuet.IsReady() && 
            !BardRotationEntry.QT.GetQt("对齐旅神"))
            return 1;
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE && GCDHelper.GetGCDCooldown() > 530)
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        if (BardRotationEntry.QT.GetQt("Debug"))
            LogHelper.Print("切歌", $"当前歌曲：{Core.Resolve<JobApi_Bard>().ActiveSong}, 下一首歌曲：{GetSongBySpell(spell.Id)}, 当前歌曲剩余时间：{Core.Resolve<JobApi_Bard>().SongTimer}");
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
    public static Song GetSongBySpell(uint song)
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