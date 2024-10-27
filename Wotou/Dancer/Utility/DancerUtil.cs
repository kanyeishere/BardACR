using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;

namespace Wotou.Dancer
{
    public static class DancerUtil
    {
        private const uint Windmill = DancerDefinesData.Spells.Windmill;
        private const uint BladeShower = DancerDefinesData.Spells.Bladeshower;
        private const uint Cascade = DancerDefinesData.Spells.Cascade;
        private const uint Fountain = DancerDefinesData.Spells.Fountain;
        private const uint ReverseCascade = DancerDefinesData.Spells.ReverseCascade;
        private const uint FountainFall = DancerDefinesData.Spells.Fountainfall;
        private const uint RisingWindmill = DancerDefinesData.Spells.RisingWindmill;
        private const uint BloodShower = DancerDefinesData.Spells.Bloodshower;
    
        private const uint FlourishingFlow = DancerDefinesData.Buffs.FlourshingFlow;
        private const uint FlourishingSymmetry = DancerDefinesData.Buffs.FlourishingSymmetry;
        private const uint SilkenFlow = DancerDefinesData.Buffs.SilkenFlow;
        private const uint SilkenSymmetry = DancerDefinesData.Buffs.SilkenSymmetry;

        public static Spell GetBaseGcdCombo()
        {
            if (Windmill.IsReady() &&
                DancerRotationEntry.QT.GetQt(QTKey.Aoe) &&
                ((TargetHelper.GetNearbyEnemyCount(5) > 2 && Core.Me.Level >= 94) ||
                 (TargetHelper.GetNearbyEnemyCount(5) > 1) && Core.Me.Level < 94))
                return GetAoeCombo();
            return GetSingleCombo();
        }
    
        private static Spell GetAoeCombo()
        {
            if (BladeShower.IsReady() &&
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Windmill &&
                Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds > 10.0)
                return BladeShower.GetSpell();
            return Windmill.GetSpell();
        }
    
        private static Spell GetSingleCombo()
        {
            if (Fountain.IsReady() &&
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Cascade && 
                Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds > 10.0)
                return Fountain.GetSpell();
            return Cascade.GetSpell();
        }
        
        
        public static Spell GetProcGcdCombo()
        {
            if (DancerRotationEntry.QT.GetQt(QTKey.Aoe) &&
                ((TargetHelper.GetNearbyEnemyCount(5) > 2 && Core.Me.Level >= 94) ||
                 (TargetHelper.GetNearbyEnemyCount(5) > 1) && Core.Me.Level < 94))
                return GetProcAoeCombo();
            return GetProcSingleCombo();
        }

        private static Spell GetProcAoeCombo()
        {
            if (Core.Me.HasAura(SilkenFlow))
                return BloodShower.GetSpell();
            
            if (Core.Me.HasAura(SilkenSymmetry))
                return RisingWindmill.GetSpell();
            
            if (BloodShower.IsReady())
                return BloodShower.GetSpell();
        
            if (RisingWindmill.IsReady())
                return RisingWindmill.GetSpell();
        
            return null;
        }


        private static Spell GetProcSingleCombo()
        {
            if (Core.Me.HasAura(SilkenFlow))
                return FountainFall.GetSpell();
            
            if (Core.Me.HasAura(SilkenSymmetry))
                return ReverseCascade.GetSpell();
            
            if (FountainFall.IsReady())
                return FountainFall.GetSpell();
        
            if (ReverseCascade.IsReady())
                return ReverseCascade.GetSpell();
            
            return null;
        }

        public static Spell? GetStep()
        {
            if (Core.Me.GetCurrTarget().CanAttack())
            {
                if (Core.Me.HasAura(DancerDefinesData.Buffs.StandardStep) && Core.Resolve<JobApi_Dancer>().CompleteSteps == 2)
                {
                    return DancerDefinesData.Spells.DoubleStandardFinish.GetSpell();
                }
                if (Core.Resolve<JobApi_Dancer>().IsDancing && Core.Resolve<JobApi_Dancer>().CompleteSteps == 4)
                {
                    return null;
                }
            }
            else
            {
                if (Core.Me.HasAura(DancerDefinesData.Buffs.StandardStep) && Core.Resolve<JobApi_Dancer>().CompleteSteps == 2)
                {
                    return null;
                }
                if (Core.Resolve<JobApi_Dancer>().CompleteSteps == 4)
                {
                    return null;
                }
            }
            return Core.Resolve<JobApi_Dancer>().NextStep.GetSpell();
        }
    }
}
