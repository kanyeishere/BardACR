using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.GCD;

public class DancerTillanaGcd : ISlotResolver
{
    private const uint Tillana = DancerDefinesData.Spells.Tillana;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    private const uint FlourishingFinish = DancerDefinesData.Buffs.FlourishingFinish;

    public int Check()
    {
        if (!Tillana.GetSpell().IsReadyWithCanCast())
            return -1;
        // 不在小舞前1G使用 
        if  (Core.Resolve<MemApiSpell>().CheckActionChange(StandardStep).GetSpell().Cooldown.TotalMilliseconds < 
             2 * GCDHelper.GetGCDDuration() - 1000 )
            return -2;
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