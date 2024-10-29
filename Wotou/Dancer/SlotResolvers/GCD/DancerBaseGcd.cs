using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.GCD;

public class DancerBaseGcd : ISlotResolver
{
    
    public int Check()
    {
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(DancerUtil.GetBaseGcdCombo());
    }
}