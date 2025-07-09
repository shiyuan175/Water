using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class OfflineNotificationManager : MonoBehaviour
{
    //通知ID,用于取消通知
    private int maxHpNotificationID = -111;
    private int offline24hNotificationID = -112;

    private const string FULL_HP_CHANNEL_ID = "FullHp";
    private const string FULL_HP_CHANNEL_NAME = "Water Sort - Magic World!";
    private const string FULL_HP_CHANNEL_DESCRIPTION = "You lives are full! Enjoy it now!";

    private const string OFFLINE_25H_CHANNEL_ID = "Offline25h";
    private const string OFFLINE_25H_CHANNEL_NAME = "Unlock the hardest levels!";
    private const string OFFLINE_25H_CHANNEL_DESCRIPTION = "Dive back into water Sort-Migic World now!";

    void Start()
    {
        RegisterNotificationChannel(FULL_HP_CHANNEL_ID, FULL_HP_CHANNEL_NAME, Importance.High, FULL_HP_CHANNEL_DESCRIPTION);
        RegisterNotificationChannel(OFFLINE_25H_CHANNEL_ID, OFFLINE_25H_CHANNEL_NAME, Importance.High, OFFLINE_25H_CHANNEL_DESCRIPTION);
    }

    //应用(暂停)切到后台时触发
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SendMaxHpNotification();
            SendOffLineNotification();
        }
        else
        {
            //或使用取消所有通知的API
            CancelMaxHpNotification();
            CancelOffLineNotification();
        }
    }

    private void RegisterNotificationChannel(string id, string name, Importance importance, string description)
    {
        //声明通道信息
        var hpChannel = new AndroidNotificationChannel()
        {
            Id = id,
            Name = name,
            Importance = importance,
            Description = description
        };
        //注册通知通道
        AndroidNotificationCenter.RegisterNotificationChannel(hpChannel);
    }

    private void SendMaxHpNotification()
    {
        if (!HealthManager.Instance.IsMaxHp)
        {
            CancelMaxHpNotification();

            //创建通知
            var notification = new AndroidNotification
            {
                Title = FULL_HP_CHANNEL_NAME,
                Text = FULL_HP_CHANNEL_DESCRIPTION,
                FireTime = HealthManager.Instance.RecoverEndTime,
                ShouldAutoCancel = true,
                SmallIcon = "icon_small0",
                LargeIcon = "icon_large0",
                Color = Color.red
            };
            maxHpNotificationID = AndroidNotificationCenter.SendNotification(notification, FULL_HP_CHANNEL_ID);
        }
    }

    private void CancelMaxHpNotification()
    {
        if (maxHpNotificationID != -111)
            AndroidNotificationCenter.CancelScheduledNotification(maxHpNotificationID);
        maxHpNotificationID = -111;
    }

    private void SendOffLineNotification()
    {
        CancelOffLineNotification();

        //创建通知
        var notification = new AndroidNotification
        {
            Title = OFFLINE_25H_CHANNEL_NAME,
            Text = OFFLINE_25H_CHANNEL_DESCRIPTION,

            FireTime = DateTime.Now.AddHours(25),
            RepeatInterval = TimeSpan.FromHours(25),

            ShouldAutoCancel = true,
            SmallIcon = "icon_small0",
            LargeIcon = "icon_large0",
            Color = Color.red
        };
        maxHpNotificationID = AndroidNotificationCenter.SendNotification(notification, OFFLINE_25H_CHANNEL_ID);
    }

    private void CancelOffLineNotification()
    {
        if (offline24hNotificationID != -112)
            AndroidNotificationCenter.CancelScheduledNotification(offline24hNotificationID);
        offline24hNotificationID = -112;
    }
}
