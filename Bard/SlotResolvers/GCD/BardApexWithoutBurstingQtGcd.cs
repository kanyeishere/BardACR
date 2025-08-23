using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardApexWithoutBurstingQtGcd : ISlotResolver
{
    private const uint ApexArrow = BardDefinesData.Spells.ApexArrow;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt(QTKey.Apex))
            return -1;
        if (ApexArrow.RecentlyUsed(5000))
            return -1;
        // 本文件只处理未开启爆发QT时，满能量的绝峰箭
        if (BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (Core.Resolve<JobApi_Bard>().SoulVoice == 100)
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BardUtil.GetSmartAoeSpell(ApexArrow, 1));
    }
}