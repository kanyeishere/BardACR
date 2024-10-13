

using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;

#nullable enable
namespace Wotou.Bard.Opener;

  public class BardOpener100 : IOpener
  {
    public int StartCheck()
    {
      if (AI.Instance.BattleData.CurrBattleTimeInMs > 3000L)
        return -9;
      if (!107U.IsReady() || !101U.IsReady() || !118U.IsReady() || Core.Resolve<MemApiSpell>().GetCooldown(3559U).TotalSeconds > 0.0)
        return -4;
      return !BardRotationEntry.QT.GetQt("爆发") || !BardRotationEntry.QT.GetQt("唱歌") ? -10 : 0;
    }

    public int StopCheck() => -1;

    public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>()
    {
      Step0,
      Step1,
      Step2,
      Step3
    };

    public void InitCountDown(CountDownHandler countDownHandler)
    {
      countDownHandler.AddAction(300, new Func<Spell>(this.PreCastSpell));
    }

    private Spell PreCastSpell()
    {
      if (Core.Me.GetCurrTarget() == null && TargetMgr.Instance.EnemysIn25.Count == 1)
        Core.Me.SetTarget((IGameObject) TargetMgr.Instance.EnemysIn25[0U]);
      return Core.Resolve<MemApiSpell>().CheckActionChange(7407U.GetSpell().Id).GetSpell(SpellTargetType.Target);
    }

    private static void Step0(Slot slot)
    {
      if (!Core.Me.GetCurrTarget().HasLocalPlayerAura(129U) && !Core.Me.GetCurrTarget().HasLocalPlayerAura(1201U) && !113U.RecentlyUsed() && !7407U.RecentlyUsed())
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(7407U.GetSpell().Id).GetSpell());
      slot.Add(3559U.GetSpell());
      slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(36975U.GetSpell().Id).GetSpell());
    }

    private static void Step1(Slot slot)
    {
      if (Core.Me.GetCurrTarget().HasLocalPlayerAura(124U) || Core.Me.GetCurrTarget().HasLocalPlayerAura(1200U))
      {
        if (Core.Me.HasLocalPlayerAura(3861U))
          slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(98U).GetSpell());
        else
          slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(97U).GetSpell());
      }
      else
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(7406U.GetSpell().Id).GetSpell());
      if (BardRotationEntry.QT.GetQt("爆发药"))
        slot.Add(Spell.CreatePotion());
      slot.Add2NdWindowAbility(101U.GetSpell());
    }

    private static void Step2(Slot slot)
    {
      if (Core.Me.HasLocalPlayerAura(3861U))
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(98U).GetSpell());
      else
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(97U).GetSpell());
      slot.Add(118U.GetSpell());
      if (!25785U.IsUnlock())
        return;
      slot.Add(25785U.GetSpell());
    }

    private static void Step3(Slot slot)
    {
      if (Core.Me.HasLocalPlayerAura(3861U))
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(98U).GetSpell());
      else
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(97U).GetSpell());
      slot.Add(3558U.GetSpell());
      slot.Add(107U.GetSpell());
    }
  }

