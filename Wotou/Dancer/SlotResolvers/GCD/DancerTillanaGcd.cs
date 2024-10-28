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
    
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint FlourishingFinish = DancerDefinesData.Buffs.FlourishingFinish;

    public int Check()
    {
        if (!Tillana.IsReady())
            return -1;
        if (Core.Resolve<JobApi_Dancer>().Esprit <= 35 && Core.Me.HasLocalPlayerAura(Devilment))
            return 1;
        if (Core.Resolve<JobApi_Dancer>().Esprit <= 40 && !Core.Me.HasLocalPlayerAura(Devilment))
            return 2;
        if (Core.Me.HasLocalPlayerAura(FlourishingFinish) && !Core.Me.HasMyAuraWithTimeleft(FlourishingFinish, 3000))
            return 3;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Tillana.GetSpell());
    }
}