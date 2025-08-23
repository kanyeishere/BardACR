using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.Ability;

public class DancerFanDance4Ability : ISlotResolver
{
    private const uint FanDance4 = DancerDefinesData.Spells.FanDance4;
    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() <= 650)
            return -1;
        if (!FanDance4.GetSpell().IsReadyWithCanCast())
            return -1;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.GetSmartAoeSpell(FanDance4, angle:90));
        // slot.Add(FanDance4.GetSpell());
    }
}