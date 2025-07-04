using System.Globalization;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Utility;

namespace Wotou.Dancer.GCD;

public class Dancer1GBeforeTechStepGcd : ISlotResolver
{
    private const uint TechnicalStep = DancerDefinesData.Spells.TechnicalStep;
    private const uint Cascade = DancerDefinesData.Spells.Cascade;
    private const uint Fountain = DancerDefinesData.Spells.Fountain;
    private const uint SaberDance = DancerDefinesData.Spells.SaberDance;
    private const uint LastDance = DancerDefinesData.Spells.LastDance;
    private const uint Windmill = DancerDefinesData.Spells.Windmill;
    private const uint BladeShower = DancerDefinesData.Spells.Bladeshower;

    private const uint LastDanceReady = DancerDefinesData.Buffs.LastDanceReady;
    private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
    private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;
    
    public int Check()
    {
        if (TechnicalStep.GetSpell().Cooldown.TotalMilliseconds < 2 * GCDHelper.GetGCDDuration() - 200 &&
            DancerRotationEntry.QT.GetQt(QTKey.TechnicalStep) &&
            TechnicalStep.IsUnlock())
            return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
    
    private static Spell GetSpell()
    {
        if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Cascade)
            return Fountain.GetSpell();
        
        if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Windmill &&
            DancerUtil.CanUseAoeCombo())
            return BladeShower.GetSpell();
        
        if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Fountain)
        {
            if (Core.Me.HasAura(SilkenFlow) || Core.Me.HasAura(SilkenSymmetry))
                return DancerUtil.GetProcGcdCombo();
            if (Core.Resolve<JobApi_Dancer>().Esprit >= 50)
                return SaberDance.GetSpell();
            if (Core.Me.HasAura(LastDanceReady))
                return LastDance.GetSpell();
            return DancerUtil.GetBaseGcdCombo();
        }

        if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == BladeShower)
        {
            if (Core.Me.HasAura(SilkenFlow) || Core.Me.HasAura(SilkenSymmetry))
                return DancerUtil.GetProcGcdCombo();
            if (Core.Resolve<JobApi_Dancer>().Esprit >= 50)
                return SaberDance.GetSpell();
            if (Core.Me.HasAura(LastDanceReady))
                return LastDance.GetSpell();
            return DancerUtil.GetBaseGcdCombo();
        }
        
        return DancerUtil.GetBaseGcdCombo();
    }
}