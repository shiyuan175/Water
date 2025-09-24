using UnityEngine;
using System;
using System.Collections.Generic;
using QFramework;
using System.Globalization;
using NodaTime;

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

    /// <summary>
    /// 开启计时器(不超过美东0点)
    /// </summary>
    public void StartCountdownTimer(string id, float hour)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        if (PlayerPrefs.HasKey(key))
            return;

        DateTime endUtc = CalculateActivityEndTimeUtc(hour);
        PlayerPrefs.SetString(key, endUtc.ToString("o"));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 重启计时器(不超过美东0点)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hour"></param>
    public void ResetCountdownTimer(string id, float hour)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;

        DateTime endUtc = CalculateActivityEndTimeUtc(hour);
        PlayerPrefs.SetString(key, endUtc.ToString("o"));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 开启X天后美东 0 点计时器
    /// </summary>
    public void StartEasternMidnightTimer(string id, int value = 1)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        if (PlayerPrefs.HasKey(key))
            return;

        DateTime endUtc = GetEasternMidnightUtcAfterDays(value);
        PlayerPrefs.SetString(key, endUtc.ToString("o"));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 重置计时器为X天后美东 0 点
    /// </summary>
    public void ResetEasternMidnightTimer(string id ,int value = 1)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        DateTime endUtc = GetEasternMidnightUtcAfterDays(value);
        PlayerPrefs.SetString(key, endUtc.ToString("o"));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 获取今日美东 0 点(对应的 UTC 时间)
    /// </summary>
    private DateTime GetTodayEasternMidnightUtc()
    {
        // 当前 UTC 时间
        Instant now = SystemClock.Instance.GetCurrentInstant();
        // 美东时区
        DateTimeZone easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
        // 当前美东日期
        LocalDate easternDate = now.InZone(easternZone).Date;
        // 当日美东午夜
        LocalDateTime midnight = easternDate.AtMidnight();
        // 转换为 ZonedDateTime
        ZonedDateTime easternMidnight = midnight.InZoneStrictly(easternZone);
        // 转回 UTC DateTime
        return easternMidnight.ToDateTimeUtc();
    }

    /// <summary>
    /// 获取X天后的美东 0 点(对应的 UTC 时间)
    /// </summary>
    private DateTime GetEasternMidnightUtcAfterDays(int value)
    {
        return GetTodayEasternMidnightUtc().AddDays(value);
    }

    /// <summary>
    /// 计算活动结束时间，持续指定小时但不超过美东明天0点
    /// </summary>
    /// <param name="hour"></param>
    /// <returns></returns>
    private DateTime CalculateActivityEndTimeUtc(float hour)
    {
        // 当前 UTC 时间
        Instant now = SystemClock.Instance.GetCurrentInstant();

        // 美东时区
        DateTimeZone easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];

        // 当前美东时间
        ZonedDateTime easternNow = now.InZone(easternZone);

        // 活动持续时间后的时间点
        Duration duration = Duration.FromHours(hour);
        ZonedDateTime laterEastern = easternNow + duration;

        // 美东明天 0 点
        LocalDate easternTomorrow = easternNow.Date.PlusDays(1);
        LocalDateTime tomorrowMidnightLocal = easternTomorrow.AtMidnight();
        ZonedDateTime easternMidnight = tomorrowMidnightLocal.InZoneStrictly(easternZone);

        // 取较早时间点
        ZonedDateTime endEastern = laterEastern.ToInstant() < easternMidnight.ToInstant() ? laterEastern : easternMidnight;

        // 转回 UTC
        return endEastern.ToDateTimeUtc();
    }
    #endregion
}
