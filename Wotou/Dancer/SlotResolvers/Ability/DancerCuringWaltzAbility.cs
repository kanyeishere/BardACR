using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Ability;

public class DancerCuringWaltzAbility : ISlotResolver
{
    private const uint CuringWaltz = DancerDefinesData.Spells.CuringWaltz;
    
    public int Check()
    {
        if (!CuringWaltz.IsUnlockWithCDCheck())
            return -1;
        if (!DancerRotationEntry.QT.GetQt(QTKey.AutoCuringWaltz))
            return -2;
        var skillTarget = PartyHelper.CastableAlliesWithin10.Count(r => r.CurrentHp > 0 &&
                                                                        (double)r.CurrentHp / r.MaxHp < 0.7 &&
                                                                        r.DistanceToPlayer() <= 5);
        if (skillTarget < 4)
            return -3;
        return 1;
    }

    public void Build(Slot slot) => slot.Add(CuringWaltz.GetSpell());
}