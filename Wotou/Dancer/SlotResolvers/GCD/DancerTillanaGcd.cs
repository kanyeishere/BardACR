using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerTillanaGcd : ISlotResolver
{
    private const uint Tillana = DancerDefinesData.Spells.Tillana;
    public int Check()
    {
        if (!Tillana.IsReady())
            return -1;
        if (Core.Resolve<JobApi_Dancer>().Esprit > 35)
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(Tillana.GetSpell());
    }
}