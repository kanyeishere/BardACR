using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardBaseGcd : ISlotResolver
{
    private const uint BurstShot = BardDefinesData.Spells.BurstShot;
    private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
    
    private const uint Ladonsbite = BardDefinesData.Spells.Ladonsbite;
    private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
    
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;
    
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check()
    {
        return 0;
    }
    
    
    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(GetBaseGcd());
    }

    private static Spell GetBaseGcd()
    {
        if (Core.Me.HasAura(HawkEyeBuff) || Core.Me.HasAura(BarrageBuff))
        {
            if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25,5) > 1 && BardRotationEntry.QT.GetQt("AOE"))
                return Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).GetSpell();
            return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
        }
        
        if (TargetHelper.GetEnemyCountInsideSector(Core.Me, Core.Me.GetCurrTarget(), 12, 90) > 1  && BardRotationEntry.QT.GetQt("AOE"))
            return Core.Resolve<MemApiSpell>().CheckActionChange(Ladonsbite).GetSpell();
        return Core.Resolve<MemApiSpell>().CheckActionChange(BurstShot).GetSpell();
    }
}