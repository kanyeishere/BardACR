using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.Sequence;

public class TechnicalStandardStepSequence: ISlotSequence
{
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    private const uint StandardStep = DancerDefinesData.Spells.StandardStep;
    private const uint QuadrupleTechnicalFinish = DancerDefinesData.Spells.QuadrupleTechnicalFinish;
    private const uint Devilment = DancerDefinesData.Spells.Devilment;
    private const uint LastDance = DancerDefinesData.Spells.LastDance;
    private const uint SaberDance = DancerDefinesData.Spells.SaberDance;
    private const uint StarfallDance = DancerDefinesData.Spells.StarfallDance;
    private const uint ReverseCascade = DancerDefinesData.Spells.ReverseCascade;
    private const uint FountainFall = DancerDefinesData.Spells.Fountainfall;
    private const uint RisingWindmill = DancerDefinesData.Spells.RisingWindmill;
    private const uint BloodShower = DancerDefinesData.Spells.Bloodshower;
    private const uint Tillana = DancerDefinesData.Spells.Tillana;
    private const uint Flourish = DancerDefinesData.Spells.Flourish;
    private const uint FinishingMove = DancerDefinesData.Spells.FinishingMove;
    private const uint FanDance3 = DancerDefinesData.Spells.FanDance3;
    private static bool _hasUsedFanDance3 = false;
    
    public int StartCheck()
    {
        if (AI.Instance.BattleData.CurrBattleTimeInMs < 5000)
            return -9;
        if (GCDHelper.GetGCDCooldown() > 0)
            return -10;
        if (!TechnicalStep.IsUnlockWithCDCheck())
            return -1;
        if (!StandardStep.IsUnlockWithCDCheck())
            return -2;
        if (!DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep))
            return -3;
        if (!DancerRotationEntry.QT.GetQt(QTKey.StandardStep))
            return -4;
        return 1;
    }
    
    public int StopCheck(int index)
    { 
        if (!Core.Resolve<MemApiSpell>().CheckActionChange(StandardStep).IsUnlockWithCDCheck())
            return 1;
        return -1;
    }
    
    public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>
    {
        Step0,
        Step1,
        Step2,
        Step3,
        Step4,
        Step5,
        Step6,
        Step7
    };
    
    private static void Step0(Slot slot)
    {
        slot.Add(TechnicalStep.GetSpell());
    }

    private static void Step1(Slot slot)
    {
        slot.Wait2NextGcd = true;
        slot.Add(Core.Resolve<JobApi_Dancer>().NextStep.GetSpell());
    }
    
    private static void Step2(Slot slot)
    {
        slot.Wait2NextGcd = true;
        slot.Add(Core.Resolve<JobApi_Dancer>().NextStep.GetSpell());
    }
    
    private static void Step3(Slot slot)
    {
        slot.Wait2NextGcd = true;
        slot.Add(Core.Resolve<JobApi_Dancer>().NextStep.GetSpell());
    }
    
    private static void Step4(Slot slot)
    {
        slot.Wait2NextGcd = true;
        slot.Add(Core.Resolve<JobApi_Dancer>().NextStep.GetSpell());
    }

    private static void Step5(Slot slot)
    {
        slot.Add(QuadrupleTechnicalFinish.GetSpell());
        if (Devilment.IsUnlockWithCDCheck())
            slot.Add(Devilment.GetSpell());
    }
    
     

    private static void Step6(Slot slot)
    {
        if (LastDance.GetSpell().IsReadyWithCanCast())
            slot.Add(LastDance.GetSpell());
        else if (Core.Resolve<JobApi_Dancer>().Esprit >= 50)
            slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).GetSpell());
        else if (Core.Resolve<JobApi_Dancer>().Esprit <= 20)
            slot.Add(Tillana.GetSpell());
        else if (StarfallDance.GetSpell().IsReadyWithCanCast())
            slot.Add(StarfallDance.GetSpell());
        else if (ReverseCascade.GetSpell().IsReadyWithCanCast() ||
                 FountainFall.GetSpell().IsReadyWithCanCast() || 
                 RisingWindmill.GetSpell().IsReadyWithCanCast() ||
                 BloodShower.GetSpell().IsReadyWithCanCast())
            slot.Add(DancerUtil.GetProcGcdCombo());
        else
            slot.Add(DancerUtil.GetBaseGcdCombo());
        if (FanDance3.GetSpell().IsReadyWithCanCast())
        {
            slot.Add(FanDance3.GetSpell());
            _hasUsedFanDance3 = true;
        }
        if (Flourish.GetSpell().IsReadyWithCanCast() && DancerRotationEntry.QT.GetQt(QTKey.Flourish))
            slot.Add(Flourish.GetSpell());
    }

    private static void Step7(Slot slot)
    {
        if (!_hasUsedFanDance3 && FanDance3.GetSpell().IsReadyWithCanCast())
            slot.Add(FanDance3.GetSpell());
        _hasUsedFanDance3 = false;
        if (Core.Resolve<JobApi_Dancer>().Esprit >= 70)
            slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(SaberDance).GetSpell());
        else if (Core.Me.Level >= 100)
            slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(FinishingMove).GetSpell());
        else
            slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(StandardStep).GetSpell());
    }
}