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
    
    // 仅当身上没有歌时，且GCD大于530ms时，才会唱歌
    public int Check()
    {
        // 此文件只处理歌曲顺序为 旅神-贤者-军神 的正常循环情况
        if (!BardUtil.IsSongOrderNormal())
            return -999;
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
        BardUtil.LogDebug("没歌时切歌", $"LastSong: {BardBattleData.Instance.LastSong}, SongTimer: {Core.Resolve<JobApi_Bard>().SongTimer}");
        slot.Add(spell);
    }
    
    private Spell GetSpell()
    {
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(ArmysPaeon) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(ArmysPaeon)) && WanderersMinuet.IsReady())
            return WanderersMinuet.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(WanderersMinuet) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(WanderersMinuet)) && MagesBallad.IsReady())
            return MagesBallad.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(MagesBallad) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(MagesBallad)) && ArmysPaeon.IsReady())
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
}