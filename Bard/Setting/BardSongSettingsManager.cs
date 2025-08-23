using Dalamud.Game.ClientState.JobGauge.Enums;

namespace Wotou.Bard.Setting;

public class BardSongSettingsManager
{
    private static BardSongSettingsManager instance;
    public static BardSongSettingsManager Instance => instance ?? (instance = new BardSongSettingsManager());

    public List<SongSetting> SongSettings { get; private set; }

    private BardSongSettingsManager()
    {
        InitializeSongSettings();
    }

    public void ResetOrder()
    {
        BardSettings.Instance.FirstSong = Song.Wanderer;
        BardSettings.Instance.SecondSong = Song.Mage;
        BardSettings.Instance.ThirdSong = Song.Army;
        InitializeSongSettings();
    }

    public void InitializeSongSettings()
    {
        SongSettings = new List<SongSetting>();

        // 根据 BardSettings.Instance 的 FirstSong、SecondSong、ThirdSong 来确定顺序
        var songOrder = new List<Song>
        {
            BardSettings.Instance.FirstSong,
            BardSettings.Instance.SecondSong,
            BardSettings.Instance.ThirdSong
        };

        foreach (var song in songOrder)
        {
            switch (song)
            {
                case Song.Wanderer:
                    SongSettings.Add(new SongSetting
                    {
                        Label = "旅神歌时长",
                        Value = BardSettings.Instance.WandererSongDuration,
                        Min = 3f,
                        Max = 45f,
                        Song = Song.Wanderer
                    });
                    break;
                case Song.Mage:
                    SongSettings.Add(new SongSetting
                    {
                        Label = "贤者歌时长",
                        Value = BardSettings.Instance.MageSongDuration,
                        Min = 3f,
                        Max = 45f,
                        Song = Song.Mage
                    });
                    break;
                case Song.Army:
                    SongSettings.Add(new SongSetting
                    {
                        Label = "军神歌时长",
                        Value = BardSettings.Instance.ArmySongDuration,
                        Min = 3f,
                        Max = 45f,
                        Song = Song.Army
                    });
                    break;
            }
        }
    }
}


public class SongSetting
{
    public string Label;
    public float Max;
    public float Min;
    public float Value;
    public Song Song; 
}