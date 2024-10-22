using AEAssist;
using AEAssist.Helper;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Wotou.Bard.Setting;

namespace Wotou.Bard.Utility;

public static class BardSongSwitchUtil
{
    public static bool CanSwitchFromFirstToSecond()
    {
        return Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.FirstSong &&
               BardRotationEntry.QT.GetQt(QTKey.Song) &&
               Core.Resolve<JobApi_Bard>().SongTimer <= 45000.0 - BardUtil.GetSongDuration(BardSettings.Instance.FirstSong) * 1000 &&
               BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsReady();
    }
    
    public static bool CanSwitchFromSecondToThird()
    {
        return Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.SecondSong &&
               BardRotationEntry.QT.GetQt(QTKey.Song) &&
               Core.Resolve<JobApi_Bard>().SongTimer <= 45000.0 - BardUtil.GetSongDuration(BardSettings.Instance.SecondSong) * 1000 &&
               BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsReady();
    }
    
    public static bool CanSwitchFromThirdToFirst()
    {
        return Core.Resolve<JobApi_Bard>().ActiveSong == BardSettings.Instance.ThirdSong && 
               BardRotationEntry.QT.GetQt(QTKey.Song) &&
               Core.Resolve<JobApi_Bard>().SongTimer <= 45000.0 - BardUtil.GetSongDuration(BardSettings.Instance.ThirdSong) * 1000 &&
               BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsReady();
    }
    
    public static bool CanSwitchFromNone()
    {
        return Core.Resolve<JobApi_Bard>().ActiveSong == Song.NONE &&
               BardRotationEntry.QT.GetQt(QTKey.Song) &&
               (BardUtil.GetSpellBySong(BardSettings.Instance.FirstSong).IsReady() ||
                BardUtil.GetSpellBySong(BardSettings.Instance.SecondSong).IsReady() ||
                BardUtil.GetSpellBySong(BardSettings.Instance.ThirdSong).IsReady());
    }
}