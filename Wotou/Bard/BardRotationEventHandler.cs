using Wotou.Bard.Setting;
using Wotou.Bard.Data;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;

namespace Wotou.Bard;

/// <summary>
/// 事件回调处理类 参考接口里的方法注释
/// </summary>
public class BardRotationEventHandler : IRotationEventHandler
{

    public async Task OnPreCombat()
    {
    }

    public void OnResetBattle()
    {
        // QT的设置重置为默认值
        BardRotationEntry.QT.Reset();
        
        // 重置战斗中缓存的数据
        BardBattleData.Instance = new BardBattleData();
    }

    public async Task OnNoTarget()
    {
        await Task.CompletedTask;
    }

    public void OnSpellCastSuccess(Slot slot, Spell spell)
    {
       
    }

    public void AfterSpell(Slot slot, Spell spell)
    {
        // 没有记录到第一个120秒的buff，就记录下来
        if (!BardBattleData.Instance.HasFirst120SBuff)
        {
            switch (spell.Id)
            {
                case BardDefinesData.Spells.RagingStrikes:
                    BardBattleData.Instance.HasFirst120SBuff = true;
                    BardBattleData.Instance.First120SBuffSpellId = BardDefinesData.Spells.RagingStrikes;
                    BardBattleData.Instance.First120SBuffId = BardDefinesData.Buffs.RagingStrikes;
                    break;
                case BardDefinesData.Spells.BattleVoice:
                    BardBattleData.Instance.HasFirst120SBuff = true;
                    BardBattleData.Instance.First120SBuffSpellId = BardDefinesData.Spells.BattleVoice;
                    BardBattleData.Instance.First120SBuffId = BardDefinesData.Buffs.BattleVoice;
                    break;
                case BardDefinesData.Spells.RadiantFinale:
                    BardBattleData.Instance.HasFirst120SBuff = true;
                    BardBattleData.Instance.First120SBuffSpellId = BardDefinesData.Spells.RadiantFinale;
                    BardBattleData.Instance.First120SBuffId = BardDefinesData.Buffs.RadiantFinale;
                    break;
            }
        } else if (!BardBattleData.Instance.HasSecond120SBuff)
        {
            switch (spell.Id)
            {
                case BardDefinesData.Spells.RagingStrikes:
                    BardBattleData.Instance.HasSecond120SBuff = true;
                    BardBattleData.Instance.Second120SBuffSpellId = BardDefinesData.Spells.RagingStrikes;
                    BardBattleData.Instance.Second120SBuffId = BardDefinesData.Buffs.RagingStrikes;
                    break;
                case BardDefinesData.Spells.BattleVoice:
                    BardBattleData.Instance.HasSecond120SBuff = true;
                    BardBattleData.Instance.Second120SBuffSpellId = BardDefinesData.Spells.BattleVoice;
                    BardBattleData.Instance.Second120SBuffId = BardDefinesData.Buffs.BattleVoice;
                    break;
                case BardDefinesData.Spells.RadiantFinale:
                    BardBattleData.Instance.HasSecond120SBuff = true;
                    BardBattleData.Instance.Second120SBuffSpellId = BardDefinesData.Spells.RadiantFinale;
                    BardBattleData.Instance.Second120SBuffId = BardDefinesData.Buffs.RadiantFinale;
                    break;
            }
        } else if (!BardBattleData.Instance.HasThird120SBuff)
        {
            switch (spell.Id)
            {
                case BardDefinesData.Spells.RagingStrikes:
                    BardBattleData.Instance.HasThird120SBuff = true;
                    BardBattleData.Instance.Third120SBuffSpellId = BardDefinesData.Spells.RagingStrikes;
                    BardBattleData.Instance.Third120SBuffId = BardDefinesData.Buffs.RagingStrikes;
                    break;
                case BardDefinesData.Spells.BattleVoice:
                    BardBattleData.Instance.HasThird120SBuff = true;
                    BardBattleData.Instance.Third120SBuffSpellId = BardDefinesData.Spells.BattleVoice;
                    BardBattleData.Instance.Third120SBuffId = BardDefinesData.Buffs.BattleVoice;
                    break;
                case BardDefinesData.Spells.RadiantFinale:
                    BardBattleData.Instance.HasThird120SBuff = true;
                    BardBattleData.Instance.Third120SBuffSpellId = BardDefinesData.Spells.RadiantFinale;
                    BardBattleData.Instance.Third120SBuffId = BardDefinesData.Buffs.RadiantFinale;
                    break;
            }
        }
        
        BardBattleData.Instance.LastSong = spell.Id switch
        {
            BardDefinesData.Spells.TheWanderersMinuet => Song.WANDERER,
            BardDefinesData.Spells.MagesBallad => Song.MAGE,
            BardDefinesData.Spells.ArmysPaeon => Song.ARMY,
            _ => BardBattleData.Instance.LastSong
        };
        
        if (spell.Id == BardDefinesData.Spells.TheWanderersMinuet)
        {
            BardBattleData.Instance.WandererTimes++;
        }
        
        if (spell.Id == BardDefinesData.Spells.RagingStrikes)
        {
            BardBattleData.Instance.HasUseIronJawsInCurrentBursting = false;
        }
        
        if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
            ChatHelper.SendMessage("/e 警告，严重错误，你开启了全局能力技能不卡GCD，可能导致本ACR产生能力技插入问题，建议关闭 <se.1>");
        
        if (BardDefinesData.Spells.TheWanderersMinuet.IsReady() && BardRotationEntry.QT.GetQt("Debug"))
        {
            LogHelper.Print("第一个使用的120秒buff:" + BardBattleData.Instance.HasFirst120SBuff + ", " + BardBattleData.Instance.First120SBuffSpellId.GetSpell().Name);
            LogHelper.Print("战斗之声CD:" + BardDefinesData.Spells.BattleVoice.GetSpell().Cooldown.TotalMilliseconds);
            LogHelper.Print("猛者强击CD:" + BardDefinesData.Spells.RagingStrikes.GetSpell().Cooldown.TotalMilliseconds);
        }
    }

    public void OnBattleUpdate(int currTimeInMs)
    {
    }

    public void OnEnterRotation()
    {
        
    }

    public void OnExitRotation()
    {
        
    }

    public void OnTerritoryChanged()
    {
        
    }
}