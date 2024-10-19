using Wotou.Bard.Setting;
using Wotou.Bard.Data;
using Wotou.Bard.SlotResolvers.Ability;
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
        if (!BardSettings.Instance.IsSongOrderNormal())
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
        
        if (!BardSettings.Instance.IsSongOrderNormal())
        {
            BardRotationEntry.QT.SetQt("对齐旅神", false);
            BardRotationEntry.QT.SetQt("强对齐", false);
        }
    }

    public async Task OnNoTarget()
    {
        var wanderersMinuet = BardDefinesData.Spells.TheWanderersMinuet;
        var magesBallad = BardDefinesData.Spells.MagesBallad;
        var armysPaeon = BardDefinesData.Spells.ArmysPaeon;
        var wandererSongDuration = BardSettings.Instance.WandererSongDuration * 1000;
        var mageSongDuration = BardSettings.Instance.MageSongDuration * 1000;
        var armySongDuration = BardSettings.Instance.ArmySongDuration * 1000;
        
        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardSongAbility.GetSongBySpell(wanderersMinuet) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45080.0 - wandererSongDuration &&
            magesBallad.IsReady() )
        {
            if (BardRotationEntry.QT.GetQt("Debug"))
                LogHelper.Print("无目标切歌", $"当前歌曲：{Core.Resolve<JobApi_Bard>().ActiveSong}, 下一首歌曲：{BardSongAbility.GetSongBySpell(wanderersMinuet)}, 当前歌曲剩余时间：{Core.Resolve<JobApi_Bard>().SongTimer}");
            await magesBallad.GetSpell().Cast();
        }

        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardSongAbility.GetSongBySpell(magesBallad) &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45080.0 - mageSongDuration &&
            armysPaeon.IsReady())
        {
            if (BardRotationEntry.QT.GetQt("Debug"))
                LogHelper.Print("无目标切歌", $"当前歌曲：{Core.Resolve<JobApi_Bard>().ActiveSong}, 下一首歌曲：{BardSongAbility.GetSongBySpell(magesBallad)}, 当前歌曲剩余时间：{Core.Resolve<JobApi_Bard>().SongTimer}");
            await armysPaeon.GetSpell().Cast();
        }

        if (Core.Resolve<JobApi_Bard>().ActiveSong == BardSongAbility.GetSongBySpell(armysPaeon) &&
            GCDHelper.GetGCDCooldown() <= BardSettings.Instance.WandererBeforeGcdTime &&
            (double)Core.Resolve<JobApi_Bard>().SongTimer < 45080.0 - armySongDuration &&
            wanderersMinuet.IsReady() &&
            !BardRotationEntry.QT.GetQt("对齐旅神"))
        {
            if (BardRotationEntry.QT.GetQt("Debug"))
                LogHelper.Print("无目标切歌", $"当前歌曲：{Core.Resolve<JobApi_Bard>().ActiveSong}, 下一首歌曲：{BardSongAbility.GetSongBySpell(armysPaeon)}, 当前歌曲剩余时间：{Core.Resolve<JobApi_Bard>().SongTimer}");
            await wanderersMinuet.GetSpell().Cast();
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
        
    }

    public void OnExitRotation()
    {
        
    }

    public void OnTerritoryChanged()
    {
        
    }
}