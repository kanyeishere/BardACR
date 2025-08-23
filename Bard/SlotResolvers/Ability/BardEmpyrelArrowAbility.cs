using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardEmpyrealArrowAbility : ISlotResolver
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const Song Wanderer = Song.Wanderer;
    
    public int Check()
    {
        if (AI.Instance.BattleData.CurrBattleTimeInMs < 3000)
            return -4;
        if (!BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -1;
        if (!EmpyrealArrow.GetSpell().IsReadyWithCanCast())
            return -1;
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            BattleVoice.IsUnlock())
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 3000 && 
            BardRotationEntry.QT.GetQt("爆发") &&
            RagingStrikes.IsUnlock())
            return -1;
        if (Core.Resolve<JobApi_Bard>().ActiveSong == Wanderer && Core.Resolve<JobApi_Bard>().Repertoire == 3)
            return -1;
        /*if (BardRotationEntry.QT.GetQt("强对齐"))
            return 1;
        if (GCDHelper.GetGCDCooldown() < BardSettings.Instance.EmpyrealArrowNotBeforeGcdTime)
            return -1;*/
        return 1;
    }

    public void Build(Slot slot) => slot.Add(EmpyrealArrow.GetSpell());
}