using AEAssist.CombatRoutine.Module;
using Wotou.Bard.SlotResolvers.Ability;
using Wotou.Bard.SlotResolvers.GCD;
namespace Wotou.Bard.SlotResolvers;

public static class BardSlotResolverConfig
    {
        // SlotResolver的配置列表
        public static readonly List<SlotResolverData> SlotResolvers = new()
        {
            // 通用队列 不管是不是gcd 都会判断的逻辑
            new SlotResolverData(new BardEmpyrealArrowGcd(), SlotMode.Always),

            // gcd队列
            new SlotResolverData(new BardBlastArrowMaxGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardIronJawsGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardApexMaxGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardRadiantEncoreMaxGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardResonantArrowMaxGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardBarrageBuffMaxGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardDotGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardApexWithoutBurstingQtGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardRefulgentArrowMaxGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardApexGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardBlastArrowGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardRadiantEncoreGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardResonantArrowGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardEmpyrealArrowGcd(), SlotMode.Gcd),
            new SlotResolverData(new BardBaseGcd(), SlotMode.Gcd),


            // offGcd队列
            new SlotResolverData(new BardEmpyrealArrowAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardSongMaxAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardPotionAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardRagingStrikesAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardBattleVoiceAndRadiantFinaleAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardPitchPerfectMaxAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardHeartBreakMaxChargeAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardBarrageAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardSideWinderAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardPitchPerfectAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardSongAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardSongSpecialOrderAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardHeartBreakAbility(), SlotMode.OffGcd),
            new SlotResolverData(new BardNaturesMinneAbility(), SlotMode.OffGcd),   
            new SlotResolverData(new BardTheWardensPaeanAbility(), SlotMode.OffGcd),
        };
    }