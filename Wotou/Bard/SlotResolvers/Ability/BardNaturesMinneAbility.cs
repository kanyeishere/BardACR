using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardNaturesMinneAbility : ISlotResolver
{
    private const uint NaturesMinne = BardDefinesData.Spells.NaturesMinne;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("大地神"))
            return -3;
        if (!NaturesMinne.GetSpell().IsReadyWithCanCast())
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds < 700 &&
            BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow) && EmpyrealArrow.IsUnlock())
            return -1;
        //爆发期不三插
        if ((!BardRotationEntry.QT.GetQt("对齐旅神") || 
             !BardRotationEntry.QT.GetQt(QTKey.Song) || 
             Core.Resolve<JobApi_Bard>().ActiveSong == Song.WANDERER) &&
            !(BardBattleData.Instance.First120SBuffSpellId == RagingStrikes && 
              RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 2000) &&
            BardRotationEntry.QT.GetQt("爆发") &&
            BattleVoice.GetSpell().IsReadyWithCanCast() &&
            !(BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
              (BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds >
               GCDHelper.GetGCDDuration() + 
               BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs -
               BardSettings.Instance.RagingStrikeBeforeGcdTime || 
               BardBattleData.Instance.Second120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds >
               GCDHelper.GetGCDDuration() + 
               BardSettings.Instance.UseBattleVoiceBeforeGcdTimeInMs -
               BardSettings.Instance.RagingStrikeBeforeGcdTime)))
            return -1;
        
        if (PartyHelper.CastableParty.Any(characterAgent => 
                (characterAgent.HasAura(1896U) && BardSettings.Instance.NaturesMinneWithRecitation) ||  //秘策
                (characterAgent.HasAura(2611U) && BardSettings.Instance.NaturesMinneWithZoe) ||         //活化
                (characterAgent.HasAura(1892U) && BardSettings.Instance.NaturesMinneWithNeutralSect)))  //中间学派
            return 0;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(NaturesMinne.GetSpell());
    }
}