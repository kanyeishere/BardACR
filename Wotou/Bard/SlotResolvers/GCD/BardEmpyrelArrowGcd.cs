using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardEmpyrealArrowGcd : ISlotResolver
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    
    public int Check()
    {
        // 仅在开启强对齐时才启用, 且当前无团辅buff
        if (BardRotationEntry.QT.GetQt("强对齐") && 
            EmpyrealArrow.IsReady() &&
            BardUtil.HasNoPartyBuff())
            return 1;
        return -1;
    }

    public void Build(Slot slot) => slot.Add(EmpyrealArrow.GetSpell());
}