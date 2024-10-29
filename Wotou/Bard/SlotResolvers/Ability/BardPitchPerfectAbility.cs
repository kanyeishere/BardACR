using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardPitchPerfectAbility : ISlotResolver
{
    private const uint PitchPerfect = BardDefinesData.Spells.PitchPerfect;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Spells.RagingStrikes;
    private static readonly uint Fist120SBuffId = BardBattleData.Instance.First120SBuffId;
    private const Song Wanderer = Song.WANDERER;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (Core.Resolve<JobApi_Bard>().ActiveSong != Wanderer)
            return -1;
        /*if (PitchPerfect.RecentlyUsed())
            return -1;*/
        if (EmpyrealArrow.RecentlyUsed() && GCDHelper.GetGCDCooldown() <= 750)
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 700 &&
           BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -1;
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3)
            return 1;
        // 旅神歌最后一跳诗心
        if ((double) Core.Resolve<JobApi_Bard>().SongTimer <= 45600.0 - (double) BardSettings.Instance.WandererSongDuration * 1000.0 && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            return 1;
        // 团辅最后一个技能
        if (!Core.Me.HasMyAuraWithTimeleft(Fist120SBuffId, 1000) && Core.Me.HasLocalPlayerAura(Fist120SBuffId) && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            return 1;
        if (Core.Resolve<JobApi_Bard>().Repertoire >= 1 && Core.Resolve<JobApi_Bard>().SongTimer < 1000)
            return 1;
        // 下一个窗口必须打EmpyrealArrow
        if (Core.Resolve<JobApi_Bard>().Repertoire == 2 && EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 2470 && EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds > 800 && !EmpyrealArrow.RecentlyUsed())
            return 1;
        return -1;
    }
    
    public void Build(Slot slot) => slot.Add(PitchPerfect.GetSpell());

}