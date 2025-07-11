using UnityEngine;
using System;
using System.Collections.Generic;
using QFramework;
using System.Globalization;

public class CountDownTimerManager : MonoSingleton<CountDownTimerManager>
{
    private static readonly string COUNTDOWN_TIMER_SIGN = "CountDownTimer_";
    private readonly Dictionary<string, CountDownTimer> timers = new();

    public class CountDownTimer
    {
        private DateTime EndTime;
        public bool IsFinished { get; private set; }

        public CountDownTimer(DateTime endTime)
        {
            EndTime = endTime;
            IsFinished = false;
        }

        public TimeSpan GetRemainingTime()
        {
            return EndTime - DateTime.UtcNow;
        }

        public bool Update()
        {
            if (IsFinished) return false;

            var remaining = GetRemainingTime();
            if (remaining <= TimeSpan.Zero)
            {
                IsFinished = true;
                return false;
            }
            return true;
        }

        public void Reset(string id, DateTime newEndTime)
        {
            EndTime = newEndTime;
            IsFinished = false;

            string key = COUNTDOWN_TIMER_SIGN + id;
            PlayerPrefs.SetString(key, EndTime.ToString("o"));
            PlayerPrefs.Save();
        }
    }

    public override void OnSingletonInit()
    {
       
    }

    //开启计时器
    public void StartTimer(string id, double minutes)
    {
        //TimeSpan duration = TimeSpan.FromHours(hours);
        TimeSpan duration = TimeSpan.FromMinutes(minutes);
        timers[id] = CreateFromPrefs(id, duration);
    }

    //重置计时器
    public void ResetTimer(string id, double minutes)
    {
        //TimeSpan duration = TimeSpan.FromHours(hours);
        TimeSpan duration = TimeSpan.FromMinutes(minutes);
        string key = COUNTDOWN_TIMER_SIGN + id;

        if (PlayerPrefs.HasKey(key))
            PlayerPrefs.DeleteKey(key);

        timers[id] = CreateFromPrefs(id, duration);
    }

    //计时器添加时间
    public void AddTimer(string id, double minutes)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        TimeSpan duration = TimeSpan.FromMinutes(minutes);
        //无记录创建
        if (!PlayerPrefs.HasKey(key))
            timers[id] = CreateFromPrefs(id, duration);
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

    //删除计时器
    public void DeleteTimer(string id)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    //获取计时器是否结束
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

    //工厂方法
    private CountDownTimer CreateFromPrefs(string id, TimeSpan duration)
    {
        string key = COUNTDOWN_TIMER_SIGN + id;
        DateTime endTime;
        bool needSave = false;

        if (PlayerPrefs.HasKey(key))
        {
            var str = PlayerPrefs.GetString(key);
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

        return new CountDownTimer(endTime);
    }

    /*private void Update()
    {
        var removeKeys = new List<string>();
        foreach (var kv in timers)
        {
            var id = kv.Key;
            var timer = kv.Value;

            if (!timer.Update())
            {
                removeKeys.Add(id);
            }
        }

        // 移除结束的计时器
        foreach (var id in removeKeys)
        {
            timers.Remove(id);
        }
    }*/
}
