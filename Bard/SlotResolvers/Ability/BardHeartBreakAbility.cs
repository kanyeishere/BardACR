using AEAssist;
using AEAssist.CombatRoutine;
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
    private const uint HeartBreak = BardDefinesData.Spells.HeartBreak;
    private const uint RainOfDeath = BardDefinesData.Spells.RainofDeath;
    private const uint SecondSong = BardDefinesData.Spells.MagesBallad;
    private const uint Sidewinder = BardDefinesData.Spells.Sidewinder;
    private const uint Barrage = BardDefinesData.Spells.Barrage;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint ArmysPaeon = BardDefinesData.Spells.ArmysPaeon;
    private const uint MagesBallad = BardDefinesData.Spells.MagesBallad;
    
    
    private static readonly uint Fist120SBuffId = BardBattleData.Instance.First120SBuffId;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell().IsReadyWithCanCast())
            return -2;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1000 &&
            BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) && 
            EmpyrealArrow.IsUnlock())
            return -3;
        if (EmpyrealArrow.RecentlyUsed(650))
            return -4;
        if (Sidewinder.GetSpell().Cooldown.TotalMilliseconds < 650 && 
            BardRotationEntry.QT.GetQt(QTKey.Sidewinder) &&
            Sidewinder.IsUnlock())
            return -5;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            RagingStrikes.IsUnlock())
            return -6;
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            BattleVoice.IsUnlock())
            return -7;
        if (Barrage.GetSpell().Cooldown.TotalMilliseconds < 650 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            Barrage.IsUnlock())
            return -8;
        
        // 不和两层诗心以上的完美音调冲突，抢团辅最后一个能力技能
        if (!Core.Me.HasMyAuraWithTimeleft(Fist120SBuffId, 1200) && Core.Me.HasLocalPlayerAura(Fist120SBuffId) && Core.Resolve<JobApi_Bard>().Repertoire >= 2)
            return -9;
        
        // 不和切贤者歌前最后一个完美音调冲突
        var wandererSongDuration = BardSettings.Instance.WandererSongDuration * 1000;
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.Wanderer &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45600.0 - wandererSongDuration && 
            MagesBallad.GetSpell().Cooldown.TotalMilliseconds <= 1200 &&
            BardRotationEntry.QT.GetQt(QTKey.Song))
            return -100;
        
        // 不和切军神歌冲突
        var mageSongDuration = BardSettings.Instance.MageSongDuration * 1000;
        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardUtil.GetSongBySpell(MagesBallad) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45600.0 - mageSongDuration &&
            ArmysPaeon.GetSpell().Cooldown.TotalMilliseconds <= 600 &&
            BardRotationEntry.QT.GetQt(QTKey.Song))
            return -10;
        
        // 满三层碎心箭，使用
        if (Core.Resolve<MemApiSpell>().GetCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) >= Core.Resolve<MemApiSpell>().GetMaxCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) - 0.1)
            return 1;
        
        // 团辅期，使用
        if (BardUtil.HasAllPartyBuff())
            return 2;
        
        if (BardRotationEntry.QT.GetQt("攒碎心箭") && BardUtil.PartyBuffWillBeReadyIn(28500) && Core.Me.Level > 80)
            return -11;
        
        if (BardRotationEntry.QT.GetQt("攒碎心箭") && BardUtil.PartyBuffWillBeReadyIn(13500) && Core.Me.Level <= 80)
            return -110;
        
        // 旅神期间，不和三层诗心的完美音调冲突
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3 && Core.Resolve<JobApi_Bard>().ActiveSong == Song.Wanderer)
            return -12;
        
        if (Core.Resolve<JobApi_Bard>().Repertoire == 2 && EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 2900 && !EmpyrealArrow.RecentlyUsed())
            return -15;
        
        // 设置保留碎心箭的层数 - 高难模式only
        if ((Core.Resolve<MemApiSpell>().GetCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) <=
             BardSettings.Instance.HeartBreakSaveStack) && 
            !BardSettings.Instance.IsDailyMode)
            return -13;
        
        return 1;
    }
    
    private static Spell GetHeartBreakSpell()
    {
        if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1  && 
            BardRotationEntry.QT.GetQt("AOE") && 
            Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).IsUnlock())
            return BardUtil.GetSmartAoeSpell(RainOfDeath, 1);
        return Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell();
    }

    public void Build(Slot slot)
    {
        slot.Add(GetHeartBreakSpell());
    }
}