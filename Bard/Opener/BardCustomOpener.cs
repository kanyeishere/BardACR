using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Opener;

public class BardCustomOpener : IOpener
{
    private readonly List<uint> _skills;

    public BardCustomOpener()
    {
        _skills = new List<uint>(BardSettings.Instance.CustomOpenerSkills);
        Sequence = BuildSequence(_skills);
    }

    public int StartCheck()
    {
        if (BardSettings.Instance.IsDailyMode)
            return -1;
        if (_skills.Count == 0)
            return -1;
        return AI.Instance.BattleData.CurrBattleTimeInMs > 3000L ? -9 : 0;
    }

    public int StopCheck() => -1;

    public List<Action<Slot>> Sequence { get; }

    public void InitCountDown(CountDownHandler countDownHandler)
    {
    }

    private static List<Action<Slot>> BuildSequence(IEnumerable<uint> skills)
    {
        var steps = new List<Action<Slot>>();
        foreach (var skillId in skills)
        {
            steps.Add(slot => AddSkill(slot, skillId));
        }

        return steps;
    }

    private static void AddSkill(Slot slot, uint skillId)
    {
        if (skillId == 0)
            return;
        if (IsPotionSkill(skillId))
        {
            slot.Add(Spell.CreatePotion());
            return;
        }
        var spell = Core.Resolve<MemApiSpell>().CheckActionChange(skillId).GetSpell();
        if (spell != null)
            slot.Add(spell);
    }
    
    private static bool IsPotionSkill(uint skillId)
    {
        return skillId is 49235 or 45996 or 44163 or 44158 or 39728 or 37841;
    }
}