using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.Opener
{
    public class DNCStdOpener100 : IOpener
    {
        public int StartCheck()
        {
            if (AI.Instance.BattleData.CurrBattleTimeInMs > 3000L)
            {
                return -9;
            }
            if (PartyHelper.Party.Count <= 4 && !Core.Me.GetCurrTarget().IsDummy() && !Core.Me.GetCurrTarget().IsBoss())
            {
                return -1;
            }
            if (!DancerDefinesData.Spells.TechnicalStep.GetSpell().IsReadyWithCanCast())
            {
                return -4;
            }
            if (!DancerRotationEntry.QT.GetQt("大舞") || !DancerRotationEntry.QT.GetQt("百花"))
            {
                return -10;
            }
            if (!DancerDefinesData.Spells.Devilment.GetSpell().IsReadyWithCanCast())
            {
                return -5;
            }
            if (!DancerDefinesData.Spells.Flourish.GetSpell().IsReadyWithCanCast())
            {
                return -6;
            }
            return 0;
        }

        public int StopCheck()
        {
            return -1;
        }

        public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>()
        {
            Step0, Step1
        };

        public void InitCountDown(CountDownHandler countDownHandler)
        {
            countDownHandler.AddAction(DancerSettings.Instance.OpenerStandardStepTime,DancerDefinesData.Spells.StandardStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerStandardStepTime - 1500, DancerUtil.GetStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerStandardStepTime - 2500, DancerUtil.GetStep);
            if (DancerRotationEntry.QT.GetQt(QTKey.UsePotion) && DancerSettings.Instance.UsePotionInOpener)
                countDownHandler.AddPotionAction(1000);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTime, DancerDefinesData.Spells.DoubleStandardFinish);
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
            //slot.Add(DancerDefinesData.Spells.Flourish.GetSpell());
            slot.Add(DancerDefinesData.Spells.TechnicalStep.GetSpell());
        }
    }
}
