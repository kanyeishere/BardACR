using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardDMTripleWeaveAbility : ISlotResolver
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
        if (Core.Resolve<MemApiZoneInfo>().GetCurrTerrId() != 1363) // DM Map ID
            return -4;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(WanderersMinuet.GetSpell());
        slot.Add(BattleVoice.GetSpell());
        slot.Add(RadiantFinale.GetSpell());
    }
}