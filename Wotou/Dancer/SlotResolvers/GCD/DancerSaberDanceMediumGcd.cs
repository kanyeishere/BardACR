using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.GCD;

public class DancerSaberDanceMediumGcd : ISlotResolver
{
    private const uint SaberDance = DancerDefinesData.Spells.SaberDance;
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    
    public int Check()
    {
        if (!Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).IsReady())
            return -1;
        if (!Core.Me.HasLocalPlayerAura(Devilment))
            return -2;
        if (Core.Resolve<JobApi_Dancer>().Esprit < 50)
            return -3;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).GetSpell());
    }
}