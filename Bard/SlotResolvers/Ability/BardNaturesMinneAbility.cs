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
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    
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
        if ((BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 2000 && 
             BardRotationEntry.QT.GetQt("爆发")) 
            || 
            (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 2000 && 
             BardRotationEntry.QT.GetQt("爆发") && 
             BardSettings.Instance.ImitateGreenPlayer)
            ||
            (WanderersMinuet.GetSpell().Cooldown.TotalMilliseconds <= 2000 && 
             GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime + 1500 && 
             BardSettings.Instance.ImitateGreenPlayer &&
             BardRotationEntry.QT.GetQt("爆发") && 
             BardRotationEntry.QT.GetQt("对齐旅神") &&
             BardBattleData.Instance.First120SBuffSpellId == BattleVoice &&
             BardBattleData.Instance.First120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 - 600 + 1500 &&
             BardBattleData.Instance.Third120SBuffSpellId.GetSpell().Cooldown.TotalMilliseconds <= 2200 * 2 + 1500)
            )
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