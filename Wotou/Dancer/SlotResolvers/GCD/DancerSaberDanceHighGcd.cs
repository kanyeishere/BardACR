using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.GCD;

public class DancerSaberDanceHighGcd : ISlotResolver
{
    private const uint SaberDance = DancerDefinesData.Spells.SaberDance;
    
    private const uint Devilment = DancerDefinesData.Buffs.Devilment;
    
    public int Check()
    {
        if (!DancerRotationEntry.QT.GetQt(QTKey.SaberDance))
            return -1;
        if (Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).GetSpell().IsReadyWithCanCast() &&
            Core.Me.HasLocalPlayerAura(Devilment) &&
            Core.Resolve<JobApi_Dancer>().Esprit >= 70)
            return 1;
        // 起手第一次剑舞
        if (Core.Me.HasLocalPlayerAura(Devilment) &&
            DancerBattleData.Instance.DanceOfTheDawnCount == 0 && 
            Core.Resolve<JobApi_Dancer>().Esprit >= 50)
            return 2;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Core
            .Resolve<MemApiSpell>()
            .CheckActionChange(
                DancerUtil.GetSmartAoeSpell(
                    Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance)).Id)
            .GetSpell());
    }
}