using UnityEngine;
using System;
using System.Collections.Generic;
using QFramework;
using System.Globalization;

public class CountDownTimerManager : MonoSingleton<CountDownTimerManager>
{
    public static readonly string COUNTDOWN_TIMER_SIGN = "CountDownTimer_";

    #region UTC计时器
    public override void OnSingletonInit()
    {
       
    }

    public void StartTimer(string id, double minutes)
    {
        //TimeSpan duration = TimeSpan.FromHours(hours);
        TimeSpan duration = TimeSpan.FromMinutes(minutes);
        CreateFromPrefs(id, duration);
    }
    public void ResetTimer(string id, double minutes)
    {
        //TimeSpan duration = TimeSpan.FromHours(hours);
        TimeSpan duration = TimeSpan.FromMinutes(minutes);
        string key = COUNTDOWN_TIMER_SIGN + id;

        if (PlayerPrefs.HasKey(key))
            PlayerPrefs.DeleteKey(key);

        CreateFromPrefs(id, duration);
    }
    public void AddTimer(string id, double minutes)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        TimeSpan duration = TimeSpan.FromMinutes(minutes);
        //无记录创建
        if (!PlayerPrefs.HasKey(key))
            CreateFromPrefs(id, duration);
        else
        {
            if (DateTime.TryParse(PlayerPrefs.GetString(key), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var endTime))
            {
                //过期重新计时
                if ((endTime - DateTime.UtcNow) <= TimeSpan.Zero)
                {
                    ResetTimer(id, minutes);
                }
                //未过期增加时长
                else
                {
                    var remaining = endTime - DateTime.UtcNow;
                    var newMinutes = remaining.TotalMinutes + minutes;
                    ResetTimer(id, newMinutes);
                }
            }
        }
    }
    public void DeleteTimer(string id)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        if (PlayerPrefs.HasKey(key))
            PlayerPrefs.DeleteKey(key);
    }

    /// <summary>
    /// True is Ended
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsTimerFinished(string id)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        if (PlayerPrefs.HasKey(key))
        {
            if (DateTime.TryParse(PlayerPrefs.GetString(key), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var endTime))
            {
                return (endTime - DateTime.UtcNow) <= TimeSpan.Zero;
            }
        }
        
        return true;
    }

    //获取剩余时间文本
    public string GetRemainingTimeText(string id)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;

        if (PlayerPrefs.HasKey(key))
        {
            if (DateTime.TryParse(PlayerPrefs.GetString(key), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var endTime))
            {
                var remaining = endTime - DateTime.UtcNow;

                if (remaining < TimeSpan.Zero)
                    remaining = TimeSpan.Zero;

                return string.Format("{0:D2}:{1:D2}:{2:D2}",
                    (int)remaining.TotalHours,
                    remaining.Minutes,
                    remaining.Seconds);
            }
        }

        return "00:00:00";
    }

    private void CreateFromPrefs(string id, TimeSpan duration)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        DateTime endTime;
        bool needSave = false;

        if (PlayerPrefs.HasKey(key))
        {
            var str = PlayerPrefs.GetString(key);
            //解析现有的结束时间
            if (!DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out endTime))
            {
                endTime = DateTime.UtcNow + duration;
                needSave = true;
            }
        }
        else
        {
            endTime = DateTime.UtcNow + duration;
            needSave = true;
        }

        if (needSave)
        {
            PlayerPrefs.SetString(key, endTime.ToString("o"));
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region 美东0点
    // 美东时区信息
    private readonly TimeZoneInfo EasternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    /// <summary>
    /// 开启美东 0 点结束的计时器
    /// </summary>
    public void StartEasternMidnightTimer(string id)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        if (PlayerPrefs.HasKey(key))
            return;

        DateTime endUtc = GetTomorrowEasternMidnightUtc();
        PlayerPrefs.SetString(key, endUtc.ToString("o"));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 重置计时器为下一次美东 0 点
    /// </summary>
    public void ResetEasternMidnightTimer(string id)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        DateTime endUtc = GetTomorrowEasternMidnightUtc();
        PlayerPrefs.SetString(key, endUtc.ToString("o"));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 获取今日美东 0 点(对应的 UTC 时间)
    /// </summary>
    private DateTime GetTodayEasternMidnightUtc()
    {
        // 当前 UTC 时间
        DateTime utcNow = DateTime.UtcNow;
        // 转换为美东当前时间
        DateTime easternNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, EasternTimeZone);
        // 取当天 0 点
        DateTime easternMidnight = easternNow.Date;
        // 转回 UTC 存储
        return TimeZoneInfo.ConvertTimeToUtc(easternMidnight, EasternTimeZone);
    }

    /// <summary>
    /// 获取明天美东 0 点(对应的 UTC 时间)
    /// </summary>
    private DateTime GetTomorrowEasternMidnightUtc()
    {
        return GetTodayEasternMidnightUtc().AddDays(1);
    }

    #endregion
}
