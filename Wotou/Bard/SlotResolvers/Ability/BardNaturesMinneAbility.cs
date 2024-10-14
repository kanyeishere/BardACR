using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardNaturesMinneAbility : ISlotResolver
{
    private const uint NaturesMinne = BardDefinesData.Spells.NaturesMinne;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("大地神"))
            return -3;
        if (!NaturesMinne.IsReady())
            return -1;
        if (GCDHelper.GetGCDCooldown() <= 650)
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