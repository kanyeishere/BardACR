using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;
using Wotou.Bard.Utility;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardBlastArrowGcd : ISlotResolver
{
    private const uint BlastArrow = BardDefinesData.Spells.BlastArrow;
    
    private const uint BlastArrowReady = BardDefinesData.Buffs.BlastArrowReady;
    private const uint HawksEye = BardDefinesData.Buffs.HawksEye;
    private const uint CausticBiteDot = BardDefinesData.Buffs.CausticBite;
    private const uint StormBiteDot = BardDefinesData.Buffs.Stormbite;
    private const uint VenomousBiteDot = BardDefinesData.Buffs.VenomousBite;
    private const uint WindBiteDot = BardDefinesData.Buffs.Windbite;
    /*private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;*/


    public int Check()
    {
        var target = Core.Me.GetCurrTarget();
        
        // 有鹰眼Buff时，且马上要续毒时，不使用爆炸箭
        if (Core.Me.HasLocalPlayerAura(HawksEye) &&
            BardRotationEntry.QT.GetQt("DOT") &&
            target != null &&
            ((target.HasLocalPlayerAura(CausticBiteDot) && !target.HasMyAuraWithTimeleft(CausticBiteDot, 8000)) ||
             (target.HasLocalPlayerAura(StormBiteDot) && !target.HasMyAuraWithTimeleft(StormBiteDot, 8000)) ||
             (target.HasLocalPlayerAura(VenomousBiteDot) && !target.HasMyAuraWithTimeleft(VenomousBiteDot, 8000)) ||
             (target.HasLocalPlayerAura(WindBiteDot) && !target.HasMyAuraWithTimeleft(WindBiteDot, 8000))))
            return -1;
        
        // 有鹰眼Buff或弩炮Buff时，且目标dot不全时，不使用爆炸箭
        /*if ((Core.Me.HasAura(HawkEyeBuff) || Core.Me.HasAura(BarrageBuff)) &&
            BardRotationEntry.QT.GetQt("DOT") &&
            target != null &&
            ((!target.HasLocalPlayerAura(CausticBiteDot) && !target.HasLocalPlayerAura(VenomousBiteDot)) ||
             (!target.HasLocalPlayerAura(WindBiteDot) && !target.HasLocalPlayerAura(StormBiteDot))))
            return -25;*/
        
        // 有爆炸箭Buff时，直接使用爆炸箭
        if (Core.Me.HasLocalPlayerAura(BlastArrowReady))
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(BardUtil.GetSmartAoeSpell(BlastArrow, 1));
    }
    
}