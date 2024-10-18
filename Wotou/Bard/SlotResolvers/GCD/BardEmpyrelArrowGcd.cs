using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardEmpyrealArrowGcd : ISlotResolver
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    public int Check()
    {
        // 仅在开启强对齐时才启用
        if (BardRotationEntry.QT.GetQt("强对齐") && EmpyrealArrow.IsReady())
            return 1;
        return -1;
    }

    public void Build(Slot slot) => slot.Add(EmpyrealArrow.GetSpell());
}