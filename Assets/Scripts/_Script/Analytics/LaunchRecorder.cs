using System;
using System.IO;
using UnityEngine;
using Analytics;

public static class LaunchRecorder
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RecordIdentity()
    {
        try
        {
            if (!Directory.Exists(AnalyticsData.AnalyticsDir))
                Directory.CreateDirectory(AnalyticsData.AnalyticsDir);

            if (File.Exists(AnalyticsData.IdentityPath))
                return;

            IdentityData identity = new IdentityData
            {
                uid = Guid.NewGuid().ToString("N"),
                firstLaunchTimeUtc = AnalyticsData.NowSeconds(),

                deviceId = SystemInfo.deviceUniqueIdentifier,
                deviceModel = SystemInfo.deviceModel,
                platform = GetPlatform(),
                language = Application.systemLanguage.ToString(),
                os = SystemInfo.operatingSystem,
            };
            AnalyticsData.WriteToJson(AnalyticsData.IdentityPath, identity);
        }
        catch (Exception e)
        {
            //Debug.LogError($"[LaunchRecorder] RecordIdentity failed: {e}");
        }
    }

    private static int GetPlatform()
    {
        //#if UNITY_ANDROID
        //        return 1;
        //#elif UNITY_IOS
        //        return 2;
        //#else
        //        return 0;
        //#endif
        switch (Application.platform)
        {
            case RuntimePlatform.Android: return 1;
            case RuntimePlatform.IPhonePlayer: return 2;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
            default:
                return 0;
        }
    }
}
