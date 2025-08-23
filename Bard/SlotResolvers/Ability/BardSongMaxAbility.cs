using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardSongMaxAbility : ISlotResolver
{
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint MagesBallad = BardDefinesData.Spells.MagesBallad;
    private const uint ArmysPaeon = BardDefinesData.Spells.ArmysPaeon;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    // 仅当身上没有歌时，且GCD大于530ms时，才会唱歌
    public int Check()
    {
        // 此文件只处理歌曲顺序为 旅神-贤者-军神 的正常循环情况下，没有歌曲时切歌的情况
        if (!BardUtil.IsSongOrderNormal())
            return -999;
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        /*if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -1;*/
        if (!WanderersMinuet.GetSpell().IsReadyWithCanCast() && !MagesBallad.GetSpell().IsReadyWithCanCast() && !ArmysPaeon.GetSpell().IsReadyWithCanCast())
            return -1;
        if (WanderersMinuet.RecentlyUsed() || MagesBallad.RecentlyUsed() || ArmysPaeon.RecentlyUsed())
            return -1;
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.None && 
            (WanderersMinuet.IsUnlockWithCDCheck() || MagesBallad.IsUnlockWithCDCheck() || ArmysPaeon.IsUnlockWithCDCheck()) )
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        BardUtil.LogDebug("没歌时切歌", $"LastSong: {BardBattleData.Instance.LastSong}, SongTimer: {Core.Resolve<JobApi_Bard>().SongTimer}");
        slot.Add(spell);
    }
    
    private Spell GetSpell()
    {
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(ArmysPaeon) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(ArmysPaeon)) && WanderersMinuet.GetSpell().IsReadyWithCanCast())
            return WanderersMinuet.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(WanderersMinuet) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(WanderersMinuet)) && MagesBallad.GetSpell().IsReadyWithCanCast())
            return MagesBallad.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(MagesBallad) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(MagesBallad)) && ArmysPaeon.GetSpell().IsReadyWithCanCast())
            return ArmysPaeon.GetSpell();
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.None )
            if (WanderersMinuet.GetSpell().IsReadyWithCanCast())
                return WanderersMinuet.GetSpell();
            else if (MagesBallad.GetSpell().IsReadyWithCanCast())
                return MagesBallad.GetSpell();
            else if (ArmysPaeon.GetSpell().IsReadyWithCanCast())
                return ArmysPaeon.GetSpell();
        return WanderersMinuet.GetSpell();
    }
}