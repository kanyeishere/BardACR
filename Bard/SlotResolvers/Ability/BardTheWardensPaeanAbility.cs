using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardTheWardensPaeanAbility : ISlotResolver
{
    private const uint TheWardensPaean = BardDefinesData.Spells.TheWardensPaean;
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt(QTKey.AutoWardensPaean))
            return -3;
        if (!TheWardensPaean.IsUnlockWithCDCheck())
            return -1;
        if (PartyHelper.CastableParty.Any(characterAgent => 
                characterAgent.HasCanDispel()))
            return 1;
        return -2;
    }
    public void Build(Slot slot)
    {
        var partyMember = PartyHelper.CastableParty.FirstOrDefault(characterAgent => 
            characterAgent.HasCanDispel());
        if (partyMember != null)
        {
            slot.Add(TheWardensPaean.GetSpell(partyMember));
        }
    }
}