using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Data;
using Wotou.Bard.Setting;

namespace Wotou.Bard.SlotResolvers.Ability;

public class BardPotionAbility : ISlotResolver
{
    private const uint RagingStrikes = BardDefinesData.Spells.RagingStrikes;

    public int Check()
    {
        if (!BardRotationEntry.QT.GetQt("爆发"))
            return -1;
        if (BardRotationEntry.QT.GetQt("对齐旅神") && 
            Core.Resolve<JobApi_Bard>().ActiveSong != Song.Wanderer)
            return -1;
        // 90级以下，高难模式下，猛者与战斗之声一起使用，不在这里处理
        if (Core.Me.Level < 90 && !BardSettings.Instance.IsDailyMode)
            return -1;
        
        // 使用爆发药
        if (RagingStrikes.GetSpell().Cooldown.TotalMilliseconds <= BardSettings.Instance.PotionBeforeGcdTime && 
            BardRotationEntry.QT.GetQt("爆发药") &&
            ItemHelper.CheckCurrJobPotion() &&
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.RagingStrikeBeforeGcdTime + BardSettings.Instance.PotionBeforeGcdTime)
            return 1;

        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Spell.CreatePotion());

    }
}