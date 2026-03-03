using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;
public class OfflineNotificationManager : MonoBehaviour
{
    //֪ͨID,����ȡ��֪ͨ
    private int maxHpNotificationID = -111;
    private int offline24hNotificationID = -112;
    private string maxHpNotificationIDIOS = "-111";
    private string offlineNotificationIDIOS = "-112"; 
    private const string FULL_HP_CHANNEL_ID = "FullHp";
    private const string FULL_HP_CHANNEL_NAME = "Water Sort - Magic World!";
    private const string FULL_HP_CHANNEL_DESCRIPTION = "You lives are full! Enjoy it now!";

    private const string OFFLINE_25H_CHANNEL_ID = "Offline25h";
    private const string OFFLINE_25H_CHANNEL_NAME = "Unlock the hardest levels!";
    private const string OFFLINE_25H_CHANNEL_DESCRIPTION = "Dive back into water Sort-Migic World now!";

#if UNITY_ANDROID
    void Start()
    {
        RegisterNotificationChannel(FULL_HP_CHANNEL_ID, FULL_HP_CHANNEL_NAME, Importance.High, FULL_HP_CHANNEL_DESCRIPTION);
        RegisterNotificationChannel(OFFLINE_25H_CHANNEL_ID, OFFLINE_25H_CHANNEL_NAME, Importance.High, OFFLINE_25H_CHANNEL_DESCRIPTION);
    }

    //Ӧ��(��ͣ)�е���̨ʱ����
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SendMaxHpNotification();
            SendOffLineNotification();
        }
        else
        {
            //��ʹ��ȡ������֪ͨ��API
            CancelMaxHpNotification();
            CancelOffLineNotification();
        }
    }

    private void RegisterNotificationChannel(string id, string name, Importance importance, string description)
    {
        //����ͨ����Ϣ
        var hpChannel = new AndroidNotificationChannel()
        {
            Id = id,
            Name = name,
            Importance = importance,
            Description = description
        };
        //ע��֪ͨͨ��
        AndroidNotificationCenter.RegisterNotificationChannel(hpChannel);
    }

    private void SendMaxHpNotification()
    {
        if (!HealthManager.Instance.IsMaxHp)
        {
            CancelMaxHpNotification();

            //����֪ͨ
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

        //����֪ͨ
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
#elif UNITY_IOS
    private void Start()
    {
        // 开启ios权限请求的协程
        StartCoroutine(RequestIOSAuthorization());
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SendMaxHpNotification();
            SendOffLineNotification();
        }
        else
        {
            //��ʹ��ȡ������֪ͨ��API
            CancelMaxHpNotification();
            CancelOffLineNotification();
            // 玩家打开应用后清空送达通知
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }
    }

    private IEnumerator RequestIOSAuthorization()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        // 2. 创建授权请求
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            // 3. 等待用户响应（异步）
            while (!req.IsFinished) yield return null; // 每帧检查一次
            ;

            // 4. 输出结果
            var res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted; // true=允许, false=拒绝
            res += "\n error:  " + req.Error; // 如果有错误
            res += "\n deviceToken:  " + req.DeviceToken; // 远程推送用
            Debug.Log(res);
        }
    }

    private void SendMaxHpNotification()
    {
        if (!HealthManager.Instance.IsMaxHp)
        {
            CancelMaxHpNotification();
            var time = HealthManager.Instance.RecoverEndTime;
            var timeTrigger = new iOSNotificationCalendarTrigger
            {
                Hour = time.Hour,
                Minute = time.Minute,
                Second = time.Second,
                Repeats = false
            };

            var notification = new iOSNotification
            {
                Title = FULL_HP_CHANNEL_NAME,
                Body = FULL_HP_CHANNEL_DESCRIPTION,
                Trigger = timeTrigger
            };

            iOSNotificationCenter.ScheduleNotification(notification);
            maxHpNotificationIDIOS = notification.Identifier;
        }
    }

    private void CancelMaxHpNotification()
    {
        if (maxHpNotificationIDIOS != "-111")
            iOSNotificationCenter.RemoveScheduledNotification(maxHpNotificationID.ToString());
        maxHpNotificationIDIOS = "-111";
    }

    private void SendOffLineNotification()
    {
        CancelOffLineNotification();
        var timeTrigger = new iOSNotificationTimeIntervalTrigger
        {
            TimeInterval = new TimeSpan(25, 0, 0),
            Repeats = true
        };
        var notification = new iOSNotification
        {
            Title = OFFLINE_25H_CHANNEL_NAME,
            Body = OFFLINE_25H_CHANNEL_DESCRIPTION,
            Trigger = timeTrigger
        };
        iOSNotificationCenter.ScheduleNotification(notification);
        offlineNotificationIDIOS = notification.Identifier;
    }

    private void CancelOffLineNotification()
    {
        if (offlineNotificationIDIOS != "-112")
            iOSNotificationCenter.RemoveScheduledNotification(offlineNotificationIDIOS);
        offlineNotificationIDIOS = "-112";
    }
#endif
}
