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

public class BardSongAbility : ISlotResolver
{
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint MagesBallad = BardDefinesData.Spells.MagesBallad;
    private const uint ArmysPaeon = BardDefinesData.Spells.ArmysPaeon;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    private const uint FootGraze = BardDefinesData.Spells.FootGraze;
    
    public int Check()
    {
        var wandererSongDuration = BardSettings.Instance.WandererSongDuration * 1000;
        var mageSongDuration = BardSettings.Instance.MageSongDuration * 1000;
        var armySongDuration = BardSettings.Instance.ArmySongDuration * 1000;
        
        // 此文件只处理歌曲顺序为 旅神-贤者-军神 的正常循环情况
        if (!BardUtil.IsSongOrderNormal())
            return -999;
        if (!BardRotationEntry.QT.GetQt("唱歌"))
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1000 
            && Core.Me.Level >= 90
            && BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow)
            // && Core.Resolve<JobApi_Bard>().ActiveSong != Song.Mage
            ) //除了贤者切军神 
            return -1;
        if (!MagesBallad.GetSpell().IsReadyWithCanCast() && 
            !ArmysPaeon.GetSpell().IsReadyWithCanCast() &&
            ((!WanderersMinuet.GetSpell().IsReadyWithCanCast() && 
              !BardSettings.Instance.ImitateGreenPlayer) ||
             (WanderersMinuet.GetSpell().Cooldown.TotalMilliseconds > 640 && 
              BardSettings.Instance.ImitateGreenPlayer)))
            return -1;
        if (WanderersMinuet.RecentlyUsed() || MagesBallad.RecentlyUsed() || ArmysPaeon.RecentlyUsed())
            return -1;
        
        // 90级以下 使用爆发药时
        if (WanderersMinuet.GetSpell().Cooldown.TotalMilliseconds <= 640 && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime + 640 && 
            Core.Me.Level < 90 &&
            BardRotationEntry.QT.GetQt("爆发药") &&
            ItemHelper.CheckCurrJobPotion() &&
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BattleVoice.GetSpell().Cooldown.TotalMilliseconds <= 2200 - 600 + 640)
        {
            BardUtil.LogDebug("切歌", "90级以下使用爆发药，第一个开的120秒buff是战斗之声，爆发切歌条件满足");
            return 120;
        }
        
        if (WanderersMinuet.GetSpell().IsReadyWithCanCast() && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime && 
            Core.Me.Level < 90 &&
            !(BardRotationEntry.QT.GetQt("爆发药") &&
             ItemHelper.CheckCurrJobPotion()) &&
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BattleVoice.GetSpell().Cooldown.TotalMilliseconds <= 2200 - 600)
        {
            BardUtil.LogDebug("切歌", "90级以下不使用爆发药，第一个开的120秒buff是战斗之声，爆发切歌条件满足");
            return 120;
        }
        
        if (WanderersMinuet.GetSpell().Cooldown.TotalMilliseconds <= 640 && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime + 640 && 
            Core.Me.Level >= 90 &&
            BardSettings.Instance.ImitateGreenPlayer &&
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 - 600 + 640 &&
            BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 * 2 + 640)
        {
            BardUtil.LogDebug("切歌", "第一个开的120秒buff是战斗之声，爆发切歌条件满足");
            return 120;
        }
        
        if (WanderersMinuet.GetSpell().Cooldown.TotalMilliseconds <= 640 && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime + 640 && 
            BardSettings.Instance.ImitateGreenPlayer &&
            Core.Me.Level >= 90 &&
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 + 640 &&
            BardBattleData.Instance.Second120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 * 2 + 640 - 650 
           )
        {
            BardUtil.LogDebug("切歌", "第一个开的120秒buff是猛者强击，爆发切歌条件满足");
            return 120;
        }
        
        
        if (WanderersMinuet.GetSpell().IsReadyWithCanCast() && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime && 
            BardRotationEntry.QT.GetQt("爆发") && 
            Core.Me.Level >= 90 &&
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 - 600 &&
            BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 * 2)
        {
            BardUtil.LogDebug("切歌", "第一个开的120秒buff是战斗之声，爆发切歌条件满足");
            return 120;
        }
        
        if (WanderersMinuet.GetSpell().IsReadyWithCanCast() && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime && 
            BardRotationEntry.QT.GetQt("爆发") && 
            Core.Me.Level >= 90 &&
            BardRotationEntry.QT.GetQt("对齐旅神") &&
            BardBattleData.Instance.First120SBuffSpellId == RagingStrikes &&
            BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 &&
            BardBattleData.Instance.Second120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 * 2 - 650 
            )
        {
            BardUtil.LogDebug("切歌", "第一个开的120秒buff是猛者强击，爆发切歌条件满足");
            return 120;
        }
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(WanderersMinuet) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - wandererSongDuration &&
            MagesBallad.GetSpell().IsReadyWithCanCast() && 
            GCDHelper.GetGCDCooldown() > 530)
            return 1;

        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(MagesBallad) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - mageSongDuration &&
            ArmysPaeon.GetSpell().IsReadyWithCanCast()&&
            (GCDHelper.GetGCDCooldown() > 530))
            return 1;

        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(ArmysPaeon) && 
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - armySongDuration &&
            WanderersMinuet.GetSpell().IsReadyWithCanCast() && 
            !BardRotationEntry.QT.GetQt("对齐旅神"))
            return 1;
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.None && 
            GCDHelper.GetGCDCooldown() > 530 && 
            (WanderersMinuet.IsUnlockWithCDCheck() || MagesBallad.IsUnlockWithCDCheck() || ArmysPaeon.IsUnlockWithCDCheck()))
            return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        var spell = this.GetSpell();
        BardUtil.LogDebug("切歌", $"当前歌曲：{Core.Resolve<JobApi_Bard>().ActiveSong}, 下一首歌曲：{BardUtil.GetSongBySpell(spell.Id)}, 当前歌曲剩余时间：{Core.Resolve<JobApi_Bard>().SongTimer}");
        if (spell.Id == WanderersMinuet && 
            BardSettings.Instance.ImitateGreenPlayer &&
            Core.Me.Level >= 90 &&
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神"))
            slot.Add(FootGraze.GetSpell());
        if (spell.Id == WanderersMinuet && 
            Core.Me.Level < 90 &&
            BardRotationEntry.QT.GetQt("爆发药") &&
            ItemHelper.CheckCurrJobPotion() &&
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神"))
            slot.Add(Spell.CreatePotion());
        slot.Add(spell);
        if (spell.Id == WanderersMinuet && 
            BardRotationEntry.QT.GetQt("爆发") && 
            BardRotationEntry.QT.GetQt("对齐旅神"))
            AI.Instance.BattleData.CurrGcdAbilityCount = 0;
    }
    
    private Spell GetSpell()
    {
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(ArmysPaeon) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(ArmysPaeon)) && WanderersMinuet.GetSpell().IsReadyWithCanCast())
            return WanderersMinuet.GetSpell();
        if ((Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(ArmysPaeon) || BardBattleData.Instance.LastSong == BardUtil.GetSongBySpell(ArmysPaeon)) && 
            WanderersMinuet.GetSpell().Cooldown.TotalMilliseconds <= 640 && BardSettings.Instance.ImitateGreenPlayer)
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