using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Utility
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
        
        public static bool CanUseAoeCombo()
        {
            return DancerRotationEntry.QT.GetQt(QTKey.Aoe) && TargetHelper.GetNearbyEnemyCount(5) > 1;
        }
        
        public static Spell GetBaseGcdCombo()
        {
            if (Fountain.GetSpell().IsReadyWithCanCast() &&
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Cascade && 
                Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds > 10.0)
                return Fountain.GetSpell();
            
            if (BladeShower.GetSpell().IsReadyWithCanCast() &&
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Windmill &&
                Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds > 10.0 &&
                CanUseAoeCombo())
                return BladeShower.GetSpell();
            
            if (Windmill.GetSpell().IsReadyWithCanCast() &&
                CanUseAoeCombo())
                return GetAoeCombo();
            return GetSingleCombo();
        }
    
        private static Spell GetAoeCombo()
        {
            if (BladeShower.GetSpell().IsReadyWithCanCast() &&
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Windmill &&
                Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds > 10.0)
                return BladeShower.GetSpell();
            return Windmill.GetSpell();
        }
    
        private static Spell GetSingleCombo()
        {
            if (Fountain.GetSpell().IsReadyWithCanCast() &&
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == Cascade && 
                Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds > 10.0)
                return Fountain.GetSpell();
            return Cascade.GetSpell();
        }
        
        
        public static Spell GetProcGcdCombo()
        {
            if (DancerRotationEntry.QT.GetQt(QTKey.Aoe) &&
                CanUseAoeCombo())
                return GetProcAoeCombo();
            return GetProcSingleCombo();
        }

        private static Spell GetProcAoeCombo()
        {
            if (Core.Me.HasAura(SilkenFlow) &&
                BloodShower.IsUnlockWithCDCheck())
                return BloodShower.GetSpell();
            
            if (Core.Me.HasAura(FlourishingFlow) &&
                BloodShower.GetSpell().IsReadyWithCanCast())
                return BloodShower.GetSpell();
            
            if (BloodShower.GetSpell().IsReadyWithCanCast())
                return BloodShower.GetSpell();
            
            if (Core.Me.HasAura(SilkenSymmetry) && 
                RisingWindmill.IsUnlockWithCDCheck())
                return RisingWindmill.GetSpell();
            
            if (Core.Me.HasAura(FlourishingSymmetry) &&
                RisingWindmill.GetSpell().IsReadyWithCanCast())
                return RisingWindmill.GetSpell();
        
            if (RisingWindmill.GetSpell().IsReadyWithCanCast())
                return RisingWindmill.GetSpell();
            
            if (Core.Me.HasAura(SilkenFlow))
                return  FountainFall.GetSpell();
            
            if (FountainFall.GetSpell().IsReadyWithCanCast())
                return FountainFall.GetSpell();
            
            if (Core.Me.HasAura(FlourishingFlow) &&
                FountainFall.GetSpell().IsReadyWithCanCast())
                return FountainFall.GetSpell();
            
            if (Core.Me.HasAura(SilkenSymmetry))
                return ReverseCascade.GetSpell();
            
            if (Core.Me.HasAura(FlourishingSymmetry) &&
                ReverseCascade.GetSpell().IsReadyWithCanCast())
                return ReverseCascade.GetSpell();
            
            if (ReverseCascade.GetSpell().IsReadyWithCanCast())
                return ReverseCascade.GetSpell();
        
            return GetAoeCombo();
        }


        private static Spell GetProcSingleCombo()
        {
            if (Core.Me.HasAura(SilkenFlow))
                return FountainFall.GetSpell();
            
            if (Core.Me.HasAura(FlourishingFlow) &&
                FountainFall.GetSpell().IsReadyWithCanCast())
                return FountainFall.GetSpell();
            
            if (FountainFall.GetSpell().IsReadyWithCanCast())
                return FountainFall.GetSpell();
            
            if (Core.Me.HasAura(SilkenSymmetry))
                return ReverseCascade.GetSpell();
            
            if (Core.Me.HasAura(FlourishingSymmetry) &&
                ReverseCascade.GetSpell().IsReadyWithCanCast())
                return ReverseCascade.GetSpell();
        
            if (ReverseCascade.GetSpell().IsReadyWithCanCast())
                return ReverseCascade.GetSpell();
            
            return GetSingleCombo();
        }

        public static Spell GetStep()
        {
            return Core.Resolve<JobApi_Dancer>().NextStep.GetSpell();
        }
        
        public static Spell GetSmartAoeSpell(uint spellId, int minTargetCount = 1, float maxDistance = 25, float? angle = null)
        {
            if (!DancerRotationEntry.QT.GetQt(QTKey.SmartAoeTarget))
                return Core.Resolve<MemApiSpell>().CheckActionChange(spellId).GetSpell();

            var target = angle != null ? 
                TargetHelper.GetMostCanTargetObjects(spellId, minTargetCount, angle.Value) : 
                TargetHelper.GetMostCanTargetObjects(spellId, minTargetCount);
            
            if (target != null && target.IsValid() && target.DistanceToPlayer() <= maxDistance)
                return Core.Resolve<MemApiSpell>().CheckActionChange(spellId).GetSpell(target);

            return Core.Resolve<MemApiSpell>().CheckActionChange(spellId).GetSpell();
        }
    }
}
