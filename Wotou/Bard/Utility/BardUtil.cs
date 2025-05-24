using System.Globalization;
using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Utility;

public static class BardUtil
{
    private const uint BattleVoiceBuff = BardDefinesData.Buffs.BattleVoice;
    private const uint RagingStrikesBuff = BardDefinesData.Buffs.RagingStrikes;
    private const uint RadiantFinaleBuff = BardDefinesData.Buffs.RadiantFinale;
    private const uint HawkEyeBuff = BardDefinesData.Buffs.HawksEye;
    private const uint BarrageBuff = BardDefinesData.Buffs.Barrage;
    
    private const uint BattleVoice = BardDefinesData.Spells.BattleVoice;
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;
    private const uint RadiantFinale = BardDefinesData.Spells.RadiantFinale;
    private const uint BurstShot = BardDefinesData.Spells.BurstShot;
    private const uint RefulgentArrow = BardDefinesData.Spells.RefulgentArrow;
    private const uint Ladonsbite = BardDefinesData.Spells.Ladonsbite;
    private const uint Shadowbite = BardDefinesData.Spells.Shadowbite;
    
    private static bool IsRadiantFinaleConditionMet()
    {
        // 检查技能是否已解锁：如果未解锁，直接返回true。如果已解锁，检查是否有Radiant Finale的Buff效果
        return !RadiantFinale.IsLevelEnough() || Core.Me.HasLocalPlayerAura(RadiantFinaleBuff);
    }
    
    private static bool IsBattleVoiceConditionMet()
    {
        return !BattleVoice.IsLevelEnough() || Core.Me.HasAura(BattleVoiceBuff);
    }
    
    private static bool IsRagingStrikesConditionMet()
    {
        return !RagingStrikes.IsLevelEnough() || Core.Me.HasAura(RagingStrikesBuff);
    }
    
    public static bool HasAllPartyBuff()
    {
        return IsBattleVoiceConditionMet() && IsRagingStrikesConditionMet() && IsRadiantFinaleConditionMet();
    }
    
    public static bool HasAnyPartyBuff()
    {
        return Core.Me.HasAura(BattleVoiceBuff) || Core.Me.HasAura(RagingStrikesBuff) || Core.Me.HasAura(RadiantFinaleBuff);
    }
    
    public static bool HasNoPartyBuff()
    {
        return !Core.Me.HasAura(BattleVoiceBuff) && !Core.Me.HasAura(RagingStrikesBuff) && !Core.Me.HasAura(RadiantFinaleBuff);
    }
    
    public static bool PartyBuffWillBeReadyIn(int ms)
    {
        return BattleVoice.GetSpell().Cooldown.TotalMilliseconds <= ms && RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <= ms;
    }
    
    public static Song GetSongBySpell(uint song)
    {
        return song switch
        {
            BardDefinesData.Spells.TheWanderersMinuet => Song.WANDERER,
            BardDefinesData.Spells.MagesBallad => Song.MAGE,
            BardDefinesData.Spells.ArmysPaeon => Song.ARMY,
            _ => Song.NONE
        };
    }
    
    public static uint GetSpellBySong(Song song)
    {
        return song switch
        {
            Song.WANDERER => BardDefinesData.Spells.TheWanderersMinuet,
            Song.MAGE => BardDefinesData.Spells.MagesBallad,
            Song.ARMY => BardDefinesData.Spells.ArmysPaeon,
            _ => 0
        };
    }
    
    public static float GetSongDuration(Song song)
    {
        return song switch
        {
            Song.WANDERER => BardSettings.Instance.WandererSongDuration,
            Song.MAGE => BardSettings.Instance.MageSongDuration,
            Song.ARMY => BardSettings.Instance.ArmySongDuration,
            _ => 0
        };
    }

    public static void LogDebug(string title, string message)
    {
        if (BardRotationEntry.QT.GetQt("Debug"))
            LogHelper.Print(title, message); 
    }
    
    public static bool IsSongOrderNormal() => BardSettings.Instance.FirstSong == Song.WANDERER && BardSettings.Instance.SecondSong == Song.MAGE && BardSettings.Instance.ThirdSong == Song.ARMY;
    
    public static Spell GetBaseGcd()
    {
        if (Core.Me.HasAura(BarrageBuff))
        {
            if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 5) > 3 &&
                BardRotationEntry.QT.GetQt("AOE") &&
                Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).IsUnlock())
                return GetSmartAoeSpell(Shadowbite, 4);
            return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
        }

        if (Core.Me.HasAura(HawkEyeBuff))
        {
            if (TargetHelper.GetNearbyEnemyCount(Core.Me.GetCurrTarget(), 25, 5) > 1 &&
                BardRotationEntry.QT.GetQt("AOE") &&
                Core.Resolve<MemApiSpell>().CheckActionChange(Shadowbite).IsUnlock())
                return GetSmartAoeSpell(Shadowbite, 2);
            return Core.Resolve<MemApiSpell>().CheckActionChange(RefulgentArrow).GetSpell();
        }

        if (TargetHelper.GetEnemyCountInsideSector(Core.Me, Core.Me.GetCurrTarget(), 12, 90) > 1 &&
            BardRotationEntry.QT.GetQt("AOE") &&
            Core.Me.GetCurrTarget().DistanceToPlayer() <= 12 &&
            Core.Resolve<MemApiSpell>().CheckActionChange(Ladonsbite).IsUnlock())
            return GetSmartAoeSpell(Ladonsbite, 2);
        return Core.Resolve<MemApiSpell>().CheckActionChange(BurstShot).GetSpell();
    }
    
    public static Spell GetSmartAoeSpell(uint spellId, int minTargetCount, float maxDistance = 25)
    {
        if (!BardRotationEntry.QT.GetQt(QTKey.SmartAoeTarget))
            return Core.Resolve<MemApiSpell>().CheckActionChange(spellId).GetSpell();

        var target = TargetHelper.GetMostCanTargetObjects(spellId, minTargetCount);
        if (target != null && target.IsValid() && target.DistanceToPlayer() <= maxDistance)
            return Core.Resolve<MemApiSpell>().CheckActionChange(spellId).GetSpell(target);

        return Core.Resolve<MemApiSpell>().CheckActionChange(spellId).GetSpell();
    }
}