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
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const Song Wanderer = Song.WANDERER;
    
    public int Check()
    {
        if (Core.Resolve<JobApi_Bard>().ActiveSong != Wanderer)
            return -1;
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3)
            return 1;
        // 旅神歌最后一个技能
        if ((double) Core.Resolve<JobApi_Bard>().SongTimer <= 46000.0 - (double) BardSettings.Instance.WandererSongDuration * 1000.0 && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            return 1;
        // 团辅最后一个技能
        if (!Core.Me.HasMyAuraWithTimeleft(RagingStrikes, 600) && Core.Me.HasMyAuraWithTimeleft(RagingStrikes, 200) && Core.Resolve<JobApi_Bard>().Repertoire >= 1)
            return 1;
        return -1;
    }


    public void Build(Slot slot) => slot.Add(PitchPerfect.GetSpell());

}