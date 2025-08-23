using System.Numerics;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.Objects.Types;

namespace Wotou.Bard.Data;

// 存放战斗中的缓存数据 战斗重置后也会跟着清除
// 举例： 诗人需要记录上一次的双dot什么时候上的/吃了多少强化资源，来决定是否在恰当的时候立即刷新双dot
public class BardBattleData
{
    public static BardBattleData Instance = new();
    
    public Song LastSong = Song.None;
    
    public Vector3? TargetPosition = null;
    
    public IBattleChara? FollowingTarget = null;
    
    public bool IsFollowing = false;
    
    public long LastSongTime = 0;
    
    public int WandererTimes = 0;
    
    public bool UseCountdown = false;

    public int PitchPerfectMinEnemyCount = 0;
    
    public HashSet<uint> DotBlackList = new HashSet<uint>();
    
    public bool HasFirst120SBuff = false;
    
    public bool HasSecond120SBuff = false;
    
    public bool HasThird120SBuff = false;
    
    public uint First120SBuffSpellId = BardDefinesData.Spells.RagingStrikes;
    public uint First120SBuffId = BardDefinesData.Buffs.RagingStrikes;
    
    public uint Second120SBuffSpellId = BardDefinesData.Spells.BattleVoice;
    public uint Second120SBuffId = BardDefinesData.Buffs.BattleVoice;
    
    public uint Third120SBuffSpellId = BardDefinesData.Spells.RadiantFinale;
    public uint Third120SBuffId = BardDefinesData.Buffs.RadiantFinale;
    
    public bool HasUseIronJawsInCurrentBursting = false;
    public bool HasUseApexArrowInCurrentNonBurstingPeriod = false;
    public bool HotkeyUseHighPrioritySlot = false; // 热键使用高优先级队列
    
    public double TotalStopTime = 0; // 总共需要停手的时间（毫秒）
    public double RestStopTime = 0; // 剩余停手时间（毫秒）
    public int GcdCountDown = 4; // GCD 倒计时
}