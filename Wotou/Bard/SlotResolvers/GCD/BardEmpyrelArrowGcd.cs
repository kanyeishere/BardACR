using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardEmpyrealArrowGcd : ISlotResolver
{
    private const uint EmpyrealArrow = BardDefinesData.Spells.EmpyrealArrow;
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;

    public int Check()
    {
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 1200 && BardRotationEntry.QT.GetQt("爆发"))
            return -2;
        if (!BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -3;
            // 当前无团辅buff
        if (//BardRotationEntry.QT.GetQt("强对齐") && 
            EmpyrealArrow.IsUnlockWithCDCheck() &&
            BardUtil.HasNoPartyBuff())
            return 1;
        return -4;
    }

    public void Build(Slot slot) => slot.Add(EmpyrealArrow.GetSpell());
}