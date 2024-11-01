using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.Ability;

public class DancerDevilmentAbility : ISlotResolver
{
    private const uint Devilment = DancerDefinesData.Spells.Devilment;
    private const uint QuadrupleTechnicalFinish = DancerDefinesData.Spells.QuadrupleTechnicalFinish;
    
    public int Check()
    {
        if (!Devilment.GetSpell().IsReadyWithCanCast())
            return -1;
        if (Core.Resolve<MemApiSpellCastSuccess>().LastGcd != QuadrupleTechnicalFinish)
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerDefinesData.Spells.Devilment.GetSpell());
        AI.Instance.BattleData.CurrGcdAbilityCount = 1;
    }
}