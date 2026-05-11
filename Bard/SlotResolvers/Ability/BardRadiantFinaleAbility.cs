using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardRadiantFinaleAbility: ISlotResolver
{
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    
    public int Check()
    {
        if (Core.Me.Level < 90 && !BardSettings.Instance.IsDailyMode)
            return -21;
        if (Core.Resolve<MemApiSpellCastSuccess>().LastAbility == BattleVoice 
            && RadiantFinale.IsUnlock()
            && RadiantFinale.GetSpell().IsReadyWithCanCast())
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(RadiantFinale.GetSpell());
    }
}