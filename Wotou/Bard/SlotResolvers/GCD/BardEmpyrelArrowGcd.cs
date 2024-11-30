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
        if (BattleVoice.GetSpell().Cooldown.TotalMilliseconds < 3000 && BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds < 3000 && BardRotationEntry.QT.GetQt("爆发"))
            return -2;
        if (AI.Instance.BattleData.CurrBattleTimeInMs < 3000)
            return -4;
        if (!BardRotationEntry.QT.GetQt(QTKey.EmpyrealArrow))
            return -3;
        if (!BardRotationEntry.QT.GetQt("强对齐"))
            return -5;
            // 当前无团辅buff
        if (//BardRotationEntry.QT.GetQt("强对齐") && 
            EmpyrealArrow.IsUnlockWithCDCheck() &&
            BardUtil.HasNoPartyBuff())
            return 1;
        if (//BardRotationEntry.QT.GetQt("强对齐") && 
            EmpyrealArrow.IsUnlock() &&
            EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds <= 400 &&
            GCDHelper.GetGCDCooldown() < 400 &&
            EmpyrealArrow.GetSpell().Cooldown.TotalMilliseconds - GCDHelper.GetGCDCooldown() < 350 &&
            BardUtil.HasNoPartyBuff())
            return 1;
        return -5;
    }

    public void Build(Slot slot) => slot.Add(EmpyrealArrow.GetSpell());
}