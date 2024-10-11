using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Wotou.Bard.Data;

namespace Wotou.Bard.SlotResolvers.GCD;

public class BardBaseGcd : ISlotResolver
{
    
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check()
    {
        return 0;
    }
    
    
    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(GetBaseGCD());
    }
    
    public static Spell GetHeavyShot()
    {
        // 如果burstShot没解锁 再使用HeavyShot
        var targetSpellId = BardDefinesData.Spells.BurstShot;
        if (!targetSpellId.IsUnlock())
        {
            targetSpellId = BardDefinesData.Spells.HeavyShot;
        }
        return targetSpellId.GetSpell();
    }
    
    // 获取辉煌/直线射击
    public static Spell GetStraightShot()
    {
        var targetSpellId = BardDefinesData.Spells.RefulgentArrow;
        if (!targetSpellId.IsUnlock())
        {
            targetSpellId = BardDefinesData.Spells.StraightShot;
        }
        return targetSpellId.GetSpell();
    }
    
    public static Spell GetBaseGCD()
    {
        if (Core.Me.HasAura(BardDefinesData.Buffs.HawksEye) || Core.Me.HasAura(BardDefinesData.Buffs.Barrage))
        {
            return GetStraightShot();
        }
        return GetHeavyShot();
    }
}