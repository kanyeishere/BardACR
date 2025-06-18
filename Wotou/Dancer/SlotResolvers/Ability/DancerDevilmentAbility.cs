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
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    
    private const uint TechnicalFinishBuff = DancerDefinesData.Buffs.TechnicalFinish;
    
    public int Check()
    {
        if (!Devilment.GetSpell().IsReadyWithCanCast())
            return -1;
        if (Core.Me.HasMyAuraWithTimeleft(TechnicalFinishBuff))
            return 1;
        if (Core.Resolve<MemApiSpellCastSuccess>().LastGcd != QuadrupleTechnicalFinish && 
            TechnicalStep.IsUnlock())
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerDefinesData.Spells.Devilment.GetSpell());
        if (Core.Resolve<MemApiSpellCastSuccess>().LastGcd == QuadrupleTechnicalFinish)
            AI.Instance.BattleData.CurrGcdAbilityCount = 1;
    }
}