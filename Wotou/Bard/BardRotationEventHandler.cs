using Wotou.Bard.Setting;
using Wotou.Bard.Data;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Utility;

namespace Wotou.Bard;

/// <summary>
/// 事件回调处理类 参考接口里的方法注释
/// </summary>
public class BardRotationEventHandler : IRotationEventHandler
{

    private bool _originalValueForNoClipGCD3;
    
    public async Task OnPreCombat()
    {
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
    }

    public void OnResetBattle()
    {
        // QT的设置重置为默认值
        BardRotationEntry.QT.Reset();
        
        // 重置战斗中缓存的数据
        BardBattleData.Instance = new BardBattleData();
        
        // 重置碎心箭保留层数
        BardSettings.Instance.HeartBreakSaveStack = 0;
        
        if (!BardUtil.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
    }

    public async Task OnNoTarget()
    {
        // 无目标切歌
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.FirstSong &&
            BardRotationEntry.QT.GetQt(QTKey.Song) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - BardUtil.GetSongDuration(BardSettings.Instance.FirstSong) * 1000 &&
            BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsReady())
            await BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().Cast();

        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.SecondSong &&
            BardRotationEntry.QT.GetQt(QTKey.Song) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - BardUtil.GetSongDuration(BardSettings.Instance.SecondSong) * 1000 &&
            BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsReady())
            await BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().Cast();

        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.ThirdSong && 
            BardRotationEntry.QT.GetQt(QTKey.Song) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45000.0 - BardUtil.GetSongDuration(BardSettings.Instance.ThirdSong) * 1000 &&
            BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsReady() )
            await BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().Cast();

        if (Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE &&
            BardRotationEntry.QT.GetQt(QTKey.Song) &&
            (BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsReady() ||
             BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsReady() ||
             BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsReady()))
        {
            if (BardBattleData.Instance.LastSong == BardSettings.Instance.FirstSong && BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsReady())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().Cast();
                return;
            }
            if (BardBattleData.Instance.LastSong == BardSettings.Instance.SecondSong && BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsReady())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().Cast();
                return;
            }
            if (BardBattleData.Instance.LastSong == BardSettings.Instance.ThirdSong && BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsReady())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().Cast();
                return;
            }
            
            if (BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsReady())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).GetSpell().Cast();
                return;
            }
            if (BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsReady())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).GetSpell().Cast();
                return;
            }
            if (BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsReady())
            {
                await BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).GetSpell().Cast();
                return;
            }
        }
        
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
    }

    public void OnBattleUpdate(int currTimeInMs)
    {
    }

    public void OnEnterRotation()
    {
        // 处理全局能力技不卡GCD
        _originalValueForNoClipGCD3 = SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3;
        if (SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3)
        {
            SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3 = false;
        }
    }

    public void OnExitRotation()
    {
        SettingMgr.GetSetting<GeneralSettings>().NoClipGCD3 = _originalValueForNoClipGCD3;
    }

    public void OnTerritoryChanged()
    {
        
    }
}