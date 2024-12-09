
using AEAssist;
using AEAssist.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable
namespace Wotou.Common
{
  public class TimeLineInfo
  {
    public string Name { get; set; }

    public string Version { get; set; }

    public string DownloadUrl { get; set; }

    public string UpdateInfo { get; set; }
  }
  public static class TimeLineUpdater
  {
    private static List<TimeLineInfo>? jsonData;
    private static string _jsonUrl;

    public static async Task UpdateFiles(string jsonUrl)
    {
      TimeLineUpdater._jsonUrl = jsonUrl;
      await TimeLineUpdater.DownloadJson();
      string baseDir;
      string triggerlinesDir;
      string[] files;
      if (TimeLineUpdater.jsonData == null)
      {
        baseDir = (string) null;
        triggerlinesDir = (string) null;
        files = (string[]) null;
      }
      else
      {
        baseDir = Share.CurrentDirectory;
        triggerlinesDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "Triggerlines"));
        if (!Directory.Exists(triggerlinesDir))
        {
          LogHelper.Error("找不到时间轴文件夹");
          LogHelper.Print("时间轴", "时间轴更新失败！请检查AE的时间轴文件夹是否存在！");
          baseDir = (string) null;
          triggerlinesDir = (string) null;
          files = (string[]) null;
        }
        else
        {
          files = Directory.GetFiles(triggerlinesDir, "*.json", SearchOption.TopDirectoryOnly);
          foreach (TimeLineInfo timeLine in TimeLineUpdater.jsonData)
          {
            string fileName = timeLine.Name + "v" + timeLine.Version;
            string filePath = Path.Combine(triggerlinesDir, fileName) + ".json";
            if (!File.Exists(filePath))
            {
              if (await TimeLineUpdater.DownloadTimeLine(timeLine.DownloadUrl, filePath))
              {
                LogHelper.Print("时间轴", fileName + " 更新成功");
                if (timeLine.UpdateInfo != "")
                  LogHelper.Print("时间轴", "更新日志：" + timeLine.UpdateInfo);
              }
              else
              {
                LogHelper.Print("时间轴", "时间轴更新失败！请检查github网络连接！");
                baseDir = (string) null;
                triggerlinesDir = (string) null;
                files = (string[]) null;
                return;
              }
            }
            else
            {
              if (await TimeLineUpdater.DownloadTimeLine(timeLine.DownloadUrl, filePath))
              {
                LogHelper.Print("时间轴", fileName + " 更新成功");
                if (timeLine.UpdateInfo != "")
                  LogHelper.Print("时间轴", "更新日志：" + timeLine.UpdateInfo);
              }
              else
              {
                LogHelper.Print("时间轴", "时间轴更新失败！请检查github网络连接！");
                baseDir = (string) null;
                triggerlinesDir = (string) null;
                files = (string[]) null;
                return;
              }
            }
            string[] strArray = files;
            for (int index = 0; index < strArray.Length; ++index)
            {
              string file = strArray[index];
              string name = Path.GetFileName(file);
              if (name.Contains(timeLine.Name) && !name.Contains("v" + timeLine.Version))
                File.Delete(file);
              name = (string) null;
              file = (string) null;
            }
            strArray = (string[]) null;
            fileName = (string) null;
            filePath = (string) null;
          }
          baseDir = (string) null;
          triggerlinesDir = (string) null;
          files = (string[]) null;
        }
      }
    }

    private static async Task DownloadJson()
    {
      HttpClient client = new HttpClient();
      try
      {
        string _jsonData = await client.GetStringAsync(TimeLineUpdater._jsonUrl);
        TimeLineUpdater.jsonData = JsonSerializer.Deserialize<List<TimeLineInfo>>(_jsonData);
        _jsonData = (string) null;
        client = (HttpClient) null;
      }
      catch (Exception ex1)
      {
        Exception ex = ex1;
        LogHelper.Print("时间轴", "时间轴更新失败！请检查github网络连接！" + _jsonUrl);
        client = (HttpClient) null;
      }
      finally
      {
        client?.Dispose();
      }
    }

    private static async Task<bool> DownloadTimeLine(string url, string path)
    {
      using (HttpClient client = new HttpClient())
      {
        try
        {
          string jsonData = await client.GetStringAsync(url);
          await File.WriteAllTextAsync(path, jsonData);
          return true;
        }
        catch (Exception ex1)
        {
          Exception ex = ex1;
          LogHelper.Error("acr下载时间轴文件时出错");
          return false;
        }
      }
    }
  }
}
