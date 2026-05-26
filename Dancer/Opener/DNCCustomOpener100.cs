using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Extension;
using AEAssist.JobApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.Opener;

public class DNCCustomOpener100 : IOpener
{
    public int StartCheck()
    {
        if (AI.Instance.BattleData.CurrBattleTimeInMs > 3000L) return -9;
        if (!DancerDefinesData.Spells.TechnicalStep.GetSpell().IsReadyWithCanCast()) return -4;
        return 0;
    }

    public int StopCheck() => -1;

    public List<Action<Slot>> Sequence { get; } = [Step0, Step1];

    public void InitCountDown(CountDownHandler countDownHandler)
    {
        if (DancerSettings.Instance.CustomOpenerUseStandardStepCountdown)
        {
            countDownHandler.AddAction(DancerSettings.Instance.OpenerStandardStepTime, DancerDefinesData.Spells.StandardStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerStandardStepTime - 1500, DancerUtil.GetStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerStandardStepTime - 2500, DancerUtil.GetStep);
        }

        if (DancerRotationEntry.QT.GetQt(QTKey.UsePotion) && DancerSettings.Instance.UsePotionInOpener && DancerSettings.Instance.CustomOpenerUsePotion)
            countDownHandler.AddPotionAction(1000);

        if (DancerSettings.Instance.CustomOpenerFinalActionType == 0)
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTime, DancerDefinesData.Spells.DoubleStandardFinish);
        else
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTime, DancerDefinesData.Spells.TechnicalStep);
    }

    private static void Step0(Slot slot)
    {
        if (Core.Resolve<JobApi_Dancer>().IsDancing &&
            Core.Resolve<JobApi_Dancer>().CompleteSteps == 2 &&
            !DancerDefinesData.Spells.DoubleStandardFinish.RecentlyUsed())
            slot.Add(DancerDefinesData.Spells.DoubleStandardFinish.GetSpell());
    }

    private static void Step1(Slot slot)
    {
        switch (DancerSettings.Instance.CustomOpenerFirstGcdActionType)
        {
            case 1:
                slot.Add(DancerDefinesData.Spells.Flourish.GetSpell());
                break;
            case 2:
                slot.Add(DancerDefinesData.Spells.StandardStep.GetSpell());
                break;
            default:
                slot.Add(DancerDefinesData.Spells.TechnicalStep.GetSpell());
                break;
        }
    }
}
