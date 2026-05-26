using AEAssist;
using AEAssist.Helper;
using System.Text.Json;
using Wotou.Bard.Setting;
using Wotou.Dancer.Setting;

#nullable enable
namespace Wotou.Common
{
  public class TimeLineInfo
  {
    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string DownloadUrl { get; set; } = string.Empty;

    public string UpdateInfo { get; set; } = string.Empty;
  }

  public static class TimeLineUpdater
  {
    private static readonly HttpClient HttpClient = new();

    public static List<TimeLineInfo>? JsonData { get; private set; }

    public static async Task UpdateFiles(string jsonUrl, Dictionary<string, bool> selectedTimeLines)
    {
      JsonData = await DownloadJson(jsonUrl);
      if (JsonData == null)
      {
        return;
      }

      SyncTimelineSelection(jsonUrl, JsonData);

      var triggerlinesDir = GetTriggerlinesDirectory();
      if (triggerlinesDir == null)
      {
        return;
      }

      foreach (var timeline in JsonData)
      {
        if (!selectedTimeLines.TryGetValue(timeline.Name, out var isSelected) || !isSelected)
        {
          continue;
        }

        var fileName = $"{timeline.Name}v{timeline.Version}";
        var targetFilePath = Path.Combine(triggerlinesDir, fileName + ".json");

        if (!File.Exists(targetFilePath))
        {
          if (!await DownloadTimeLine(timeline.DownloadUrl, targetFilePath))
          {
            LogHelper.Print("时间轴", "时间轴更新失败！请检查github网络连接！");
            return;
          }

          LogHelper.Print("时间轴", fileName + " 更新成功");
          if (!string.IsNullOrWhiteSpace(timeline.UpdateInfo))
          {
            LogHelper.Print("时间轴", "更新日志：" + timeline.UpdateInfo);
          }
        }
        else
        {
          LogHelper.Print("时间轴", $"{fileName} 已是最新版本");
        }

        PruneOldVersions(triggerlinesDir, timeline);
      }
    }

    private static string? GetTriggerlinesDirectory()
    {
      var baseDir = Share.CurrentDirectory;
      var triggerlinesDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "Triggerlines"));
      if (!Directory.Exists(triggerlinesDir))
      {
        LogHelper.Error("找不到时间轴文件夹");
        LogHelper.Print("时间轴", "时间轴更新失败！请检查AE的时间轴文件夹是否存在！");
        return null;
      }

      return triggerlinesDir;
    }

    private static async Task<List<TimeLineInfo>?> DownloadJson(string jsonUrl)
    {
      try
      {
        var rawJson = await HttpClient.GetStringAsync(jsonUrl);
        return JsonSerializer.Deserialize<List<TimeLineInfo>>(rawJson);
      }
      catch (Exception)
      {
        LogHelper.Print("时间轴", "时间轴更新失败！请检查github网络连接！" + jsonUrl);
        return null;
      }
    }

    private static async Task<bool> DownloadTimeLine(string url, string path)
    {
      try
      {
        var jsonData = await HttpClient.GetStringAsync(url);
        await File.WriteAllTextAsync(path, jsonData);
        return true;
      }
      catch (Exception)
      {
        LogHelper.Error("acr下载时间轴文件时出错");
        return false;
      }
    }

    private static void PruneOldVersions(string triggerlinesDir, TimeLineInfo timeline)
    {
      var expectedVersionToken = "v" + timeline.Version;
      var files = Directory.GetFiles(triggerlinesDir, "*.json", SearchOption.TopDirectoryOnly);

      foreach (var file in files)
      {
        var name = Path.GetFileName(file);
        if (name.Contains(timeline.Name, StringComparison.Ordinal) &&
            !name.Contains(expectedVersionToken, StringComparison.Ordinal))
        {
          File.Delete(file);
        }
      }
    }

    private static void SyncTimelineSelection(string jsonUrl, IEnumerable<TimeLineInfo> timelines)
    {
      var lowerUrl = jsonUrl.ToLowerInvariant();
      if (lowerUrl.Contains("bard"))
      {
        UpdateSelectedMap(BardSettings.Instance.SelectedTimeLinesForUpdate, timelines);
        BardSettings.Instance.Save();
      }

      if (lowerUrl.Contains("dancer"))
      {
        UpdateSelectedMap(DancerSettings.Instance.SelectedTimeLinesForUpdate, timelines);
        DancerSettings.Instance.Save();
      }
    }

    private static void UpdateSelectedMap(Dictionary<string, bool> map, IEnumerable<TimeLineInfo> timelines)
    {
      foreach (var item in timelines)
      {
        if (!map.ContainsKey(item.Name))
        {
          map[item.Name] = true;
        }
      }
    }
  }
}
