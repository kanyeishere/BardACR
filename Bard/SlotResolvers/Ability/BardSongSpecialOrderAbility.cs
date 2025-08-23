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
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardSongSpecialOrderAbility : ISlotResolver
{
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint MagesBallad = BardDefinesData.Spells.MagesBallad;
    private const uint ArmysPaeon = BardDefinesData.Spells.ArmysPaeon;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        // 此文件只处理歌曲顺序不为 旅神-贤者-军神 的特殊循环情况。 只要有一首歌不是默认的顺序，就不会执行这个文件
        if (BardUtil.IsSongOrderNormal())
            return -999;
        
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -1;
        if (!WanderersMinuet.GetSpell().IsReadyWithCanCast() && !MagesBallad.GetSpell().IsReadyWithCanCast() && !ArmysPaeon.GetSpell().IsReadyWithCanCast())
            return -1;
        if (WanderersMinuet.RecentlyUsed() || MagesBallad.RecentlyUsed() || ArmysPaeon.RecentlyUsed())
            return -1;
        
        if (BardSongSwitchUtil.CanSwitchFromFirstToSecond() && 
            GCDHelper.GetGCDCooldown() > 530)
            return 1;

        if (BardSongSwitchUtil.CanSwitchFromSecondToThird() &&
            (GCDHelper.GetGCDCooldown() > 530))
            return 1;

        if (BardSongSwitchUtil.CanSwitchFromThirdToFirst() &&
            GCDHelper.GetGCDCooldown() > 530)
            return 1;

        if (BardSongSwitchUtil.CanSwitchFromNone() &&
            GCDHelper.GetGCDCooldown() > 530)
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        BardUtil.LogDebug("特殊歌轴切歌", $"当前歌曲：{Core.Resolve<JobApi_Bard>().ActiveSong}, 下一首歌曲：{BardUtil.GetSongBySpell(spell.Id)}, 当前歌曲剩余时间：{Core.Resolve<JobApi_Bard>().SongTimer}");
        slot.Add(spell);
    }
    
    private Spell GetSpell()
    {
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.ThirdSong || BardBattleData.Instance.LastSong == BardSettings.Instance.ThirdSong) && 
            BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().IsReadyWithCanCast())
            return BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.FirstSong || BardBattleData.Instance.LastSong == BardSettings.Instance.FirstSong) && 
            BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().IsReadyWithCanCast())
            return BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.SecondSong || BardBattleData.Instance.LastSong == BardSettings.Instance.SecondSong) && 
            BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().IsReadyWithCanCast())
            return BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell();
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.None )
            if (BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().IsReadyWithCanCast())
                return BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell();
            else if (BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().IsReadyWithCanCast())
                return BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell();
            else if (BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().IsReadyWithCanCast())
                return BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell();
        return BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell();
    }
}