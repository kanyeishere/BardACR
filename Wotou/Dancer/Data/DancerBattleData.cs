namespace Wotou.Dancer.Data;

public class DancerBattleData
{
    public static DancerBattleData Instance = new();
    
    public int TechnicalStepCount = 0;
    public bool HotkeyUseHighPrioritySlot = false; // 热键使用高优先级队列
}