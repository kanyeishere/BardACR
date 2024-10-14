using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardPitchPerfectMaxAbility : ISlotResolver
{
    private const uint PitchPerfect = BardDefinesData.Spells.PitchPerfect;

    private const Song Wanderer = Song.WANDERER;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (Core.Resolve<JobApi_Bard>().ActiveSong != Wanderer)
            return -1;
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3  && Core.Resolve<JobApi_Bard>().SongTimer % 3000 < 900)
            return 1;
        
        return -1;
    }
    
    public void Build(Slot slot) => slot.Add(PitchPerfect.GetSpell());

}