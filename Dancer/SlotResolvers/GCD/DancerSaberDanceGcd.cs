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

public class DancerSaberDanceGcd : ISlotResolver
{
    private const uint SaberDance = DancerDefinesData.Spells.SaberDance;
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    private const uint Tillana = DancerDefinesData.Spells.Tillana;
    
    private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
    private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
    private const uint FinishingMoveReady = DancerDefinesData.Buffs.FinishingMoveReady;

    
    public int Check()
    {
        if (!Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).GetSpell().IsReadyWithCanCast())
            return -1;
        if (!DancerRotationEntry.QT.GetQt(QTKey.SaberDance))
            return -1;
        
        /*if (Core.Me.HasAura(FlourishingSymmetry) || 
            Core.Me.HasAura(FlourishingFlow))
            return -4;*/
        if (DancerRotationEntry.QT.GetQt(QTKey.FinalBurst))
            return 1;
        if (Tillana.GetSpell().IsReadyWithCanCast())
            return 2;
        // 下一个技能是 需要跳舞的标准舞步 （而非结束动作）
        if (StandardStep.GetSpell().Cooldown.TotalMilliseconds < 3500 &&
            Core.Resolve<JobApi_Dancer>().Esprit >= 80 &&
            DancerRotationEntry.QT.GetQt(QTKey.StandardStep) &&
            !Core.Me.HasLocalPlayerAura(FinishingMoveReady))
            return 1;
        /*if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds < 30000 && 
            Core.Resolve<JobApi_Dancer>().Esprit < 80 &&
            DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return -2;*/
        if (Core.Resolve<JobApi_Dancer>().Esprit < DancerSettings.Instance.SaberDanceEspritThreshold)
            return -3;
        return 1;
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