using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardTripleWeaveDM : ISlotResolver
{
    private const uint WanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    
    public int Check()
    {
        if (!BardSettings.Instance.EnableTripleWeaveForDM)
            return -1;
        if (!WanderersMinuet.GetSpell().IsReadyWithCanCast())
            return -2;
        if (BattleVoice.GetSpell().Cooldown.TotalSeconds > 800)
            return -3;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(WanderersMinuet.GetSpell());
        slot.Add(BattleVoice.GetSpell());
        slot.Add(RadiantFinale.GetSpell());
    }
}