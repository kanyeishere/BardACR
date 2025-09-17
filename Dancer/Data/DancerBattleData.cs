namespace Wotou.Dancer.Data;

public class DancerBattleData
{
    public static DancerBattleData Instance = new();
    
    public int TechnicalStepCount = 0;
    public int DanceOfTheDawnCount = 0;
    public bool HotkeyUseHighPrioritySlot = false; // 热键使用高优先级队列
    public int LastWarningTime = 0;
    public int LastNotifyTime = 0;
    public int LastCountDownTime = 0;
}