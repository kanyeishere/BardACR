using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Extension;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.Opener;

public class DNCSimpleOpener100 : IOpener
{
    public int StartCheck()
    {
        if (AI.Instance.BattleData.CurrBattleTimeInMs > 3000L) return -9;
        if (!DancerDefinesData.Spells.TechnicalStep.GetSpell().IsReadyWithCanCast()) return -4;
        return 0;
    }

    public int StopCheck() => -1;

    public List<Action<Slot>> Sequence { get; } = [Step0];

    public void InitCountDown(CountDownHandler countDownHandler)
    {
    }

    private static void Step0(Slot slot)
    {
        slot.Add(DancerDefinesData.Spells.TechnicalStep.GetSpell());
    }
}
