using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardEmpyrealArrowAbility : ISlotResolver
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    public int Check()
    {
        if (!EmpyrealArrow.IsReady())
            return -1;
        if (EmpyrealArrow.RecentlyUsed())
            return -1;
        return 1;
    }

    public void Build(Slot slot) => slot.Add(EmpyrealArrow.GetSpell());
}