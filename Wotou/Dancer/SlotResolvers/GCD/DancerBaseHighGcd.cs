using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.GCD;

public class DancerBaseHighGcd : ISlotResolver
{
    public int Check()
    {
        if (Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds < 3000)
            return -1;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.GetBaseGcdCombo());
    }
}