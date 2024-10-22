using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardApexMaxGcd : ISlotResolver
{
    private const uint ApexArrow = BardDefinesData.Spells.ApexArrow;
    
    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("绝峰箭"))
            return -1;
        // 爆发期绝峰箭处理
        if (BardUtil.HasAllPartyBuff() &&(Core.Resolve<JobApi_Bard>().SoulVoice == 100))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(ApexArrow.GetSpell());
    }
}