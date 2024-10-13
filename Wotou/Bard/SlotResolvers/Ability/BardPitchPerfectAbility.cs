using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardPitchPerfectAbility : ISlotResolver
{
    private const uint PitchPerfect = BardDefinesData.Spells.PitchPerfect;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const Song Wanderer = Song.WANDERER;
    
    public int Check()
    {
        
        // 旅神歌最后一跳诗心，放在BardSongAbility里面处理
        /*if ((double) Core.Resolve<JobApi_Bard>().SongTimer <= 45000.0 - (double) BardSettings.Instance.WandererSongDuration * 1000.0 && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            return 1;*/
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (Core.Resolve<JobApi_Bard>().ActiveSong != Wanderer)
            return -1;
        // 团辅最后一个技能，不管卡不卡gcd
        if (!Core.Me.HasMyAuraWithTimeleft(BattleVoiceBuff, 1300) && Core.Me.HasMyAuraWithTimeleft(BattleVoiceBuff, 100) && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            return 1;
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3)
            return 1;
        // 下一个窗口必须打EmpyrealArrow
        if (Core.Resolve<JobApi_Bard>().Repertoire == 2 && EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 2470 && EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds > 800 && !EmpyrealArrow.RecentlyUsed())
            return 1;
        return -1;
    }
    
    public void Build(Slot slot) => slot.Add(PitchPerfect.GetSpell());

}