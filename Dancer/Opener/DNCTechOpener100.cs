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
    public class DNCTechOpener100 : IOpener
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
            bool hasTechnicalCountdownState =
                Core.Resolve<JobApi_Dancer>().IsDancing && Core.Me.HasAura(DancerDefinesData.Buffs.TechnicalStep) ||
                DancerDefinesData.Spells.QuadrupleTechnicalFinish.RecentlyUsed(5000);
            if (!DancerDefinesData.Spells.TechnicalStep.GetSpell().IsReadyWithCanCast() && !hasTechnicalCountdownState)
            {
                return -4;
            }
            if (!DancerRotationEntry.QT.GetQt("大舞") || !DancerRotationEntry.QT.GetQt("百花"))
            {
                return -10;
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
            Step0
        };

        public void InitCountDown(CountDownHandler countDownHandler)
        {
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTechnicalStepTime, DancerDefinesData.Spells.TechnicalStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTechnicalStepTime - 1000, DancerUtil.GetStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTechnicalStepTime - 2500, DancerUtil.GetStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTechnicalStepTime - 4000, DancerUtil.GetStep);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTechnicalStepTime - 5500, DancerUtil.GetStep);
            if (DancerRotationEntry.QT.GetQt(QTKey.UsePotion) && DancerSettings.Instance.UsePotionInOpener)
                countDownHandler.AddPotionAction(1000);
            countDownHandler.AddAction(DancerSettings.Instance.OpenerTime, DancerDefinesData.Spells.QuadrupleTechnicalFinish);
        }

        private static void Step0(Slot slot)
        {
            slot.Add(DancerDefinesData.Spells.Flourish.GetSpell());
        }
    }
}
