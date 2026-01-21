using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Analytics
{
    public enum EventTable
    {
        None = 0,
        Launch = 1,
        Level = 2
    }

    public class ReportApiResponse
    {
        public string code;
        public string msg;
        public string time;
    }

    public class PublicIPApiResponse
    {
        public string Ip;
        public string CountryCode;
        public string CityCode;
    }

    public class EventCache
    {
        public List<string> Events;
    }

    public class IdentityData
    {
        public string uid;
        public int firstLaunchTimeUtc;

        public string deviceId;
        public string deviceModel;
        public int platform;
        public string language;
        public string os;
    }

    public class LaunchEvent
    {
        public readonly string table;
        public string uid;
        public string deviceid;
        public int platform;

        public string ip;
        public string country;
        public string city;

        public int createtime;

        public LaunchEvent()
        {
            table = "l_line_log";
        }
    }

    public class LevelEvent
    {
        public readonly string table;
        public string uid;
        public string deviceid;
        public int stageid;
        public int type;    //1.进入关卡 2.关卡结束
        public int iswin;   //1.过关 2.失败
        public int createtime;
        public int coins;
        public List<Items> items;

        public LevelEvent() 
        {
            table = "l_stage_log";
        }
    }

    public class Items
    {
        //或者不传name，只传id，避免道具变更
        public string itemName;
        public int itemId;
        public int count;
    }

    public static class AnalyticsData
    {
        public static readonly string AnalyticsDir =
            Path.Combine(Application.persistentDataPath, "Analytics");

        public static readonly string IdentityPath =
            Path.Combine(AnalyticsDir, "identity.json");

        public static readonly string EventCachePath =
            Path.Combine(AnalyticsDir, "eventCache.json");

        public static T ReadFromJson<T>(string path) where T : class
        {
            return File.Exists(path) ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : null;
        }

        public static void WriteToJson<T>(string path, T data) 
        {
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
        }

        public static int NowSeconds()
        {
            return (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public static PublicIPApiResponse ParseIp(string mes)
        {
            if (string.IsNullOrEmpty(mes))
                return null;

            var response = new PublicIPApiResponse();

            var lines = mes.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var index = line.IndexOf('=');
                if (index <= 0) continue;

                var key = line[..index].Trim();
                var value = line[(index + 1)..].Trim();

                _ = key switch
                {
                    "ip" => response.Ip = value,
                    "loc" => response.CountryCode = value,
                    "colo" => response.CityCode = value,
                    _ => null
                };
            }

            return response;
        }
    }
}
