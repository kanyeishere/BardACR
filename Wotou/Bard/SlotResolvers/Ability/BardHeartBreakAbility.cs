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
    
    private static readonly float FirstSongDuration = BardSettings.Instance.WandererSongDuration * 1000;
    
    private static readonly uint Fist120SBuffId = BardBattleData.Instance.First120SBuffId;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).IsReady())
            return -1;
        /*if (Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).RecentlyUsed(2300))
            return -1;*/
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (Sidewinder.GetSpell().Cooldown.TotalMilliseconds < 1200)
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (Barrage.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        
        // 不和两层诗心以上的完美音调冲突，抢团辅最后一个能力技能
        if (!Core.Me.HasMyAuraWithTimeleft(Fist120SBuffId, 1200) && Core.Me.HasMyAuraWithTimeleft(Fist120SBuffId, 30) && Core.Resolve<JobApi_Bard>().Repertoire >= 2)
            return -1;
        
        // 不和切歌前最后一个完美音调冲突
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.WANDERER &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - FirstSongDuration && SecondSong.IsReady())
            return -100;
        
        /*if (EmpyrealArrow.RecentlyUsed())
            return -1;*/
        
        // 满三层碎心箭，使用
        if (Core.Resolve<MemApiSpell>().GetCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) >= Core.Resolve<MemApiSpell>().GetMaxCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) - 0.1)
            return 1;
        
        if (BardRotationEntry.QT.GetQt("攒碎心箭") && BardUtil.PartyBuffWillBeReadyIn(28000))
            return -1;
        
        // 旅神期间，不和三层诗心的完美音调冲突
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3 && Core.Resolve<JobApi_Bard>().ActiveSong == Song.WANDERER)
            return -1;
        
        // 设置保留碎心箭的层数
        if (Core.Resolve<MemApiSpell>().GetCharges(Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak)) <= BardSettings.Instance.HeartBreakSaveStack)
            return -1;
        return 1;
    }
    
    private static Spell GetHeartBreakSpell()
    {
        if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 8) > 1  && BardRotationEntry.QT.GetQt("AOE"))
            return Core.Resolve<MemApiSpell>().CheckActionChange(RainOfDeath).GetSpell();
        return Core.Resolve<MemApiSpell>().CheckActionChange(HeartBreak).GetSpell();
    }

    public void Build(Slot slot)
    {
        slot.Add(GetHeartBreakSpell());
    }
}