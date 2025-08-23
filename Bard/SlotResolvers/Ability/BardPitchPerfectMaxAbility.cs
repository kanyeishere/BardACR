using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardPitchPerfectMaxAbility : ISlotResolver
{
    private const uint PitchPerfect = BardDefinesData.Spells.PitchPerfect;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;

    private const Song Wanderer = Song.Wanderer;
    
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!PitchPerfect.IsUnlock())
            return -2;
        if (Core.Resolve<JobApi_Bard>().ActiveSong != Wanderer)
            return -1;
        if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25,5) < BardBattleData.Instance.PitchPerfectMinEnemyCount)
            return -1;
        if (Core.Resolve<JobApi_Bard>().Repertoire == 3  && Core.Resolve<JobApi_Bard>().SongTimer % 3000 < 2600)
            return 1;
        
        return -1;
    }
    
    public void Build(Slot slot) => slot.Add(BardUtil.GetSmartAoeSpell(PitchPerfect, 1));

}