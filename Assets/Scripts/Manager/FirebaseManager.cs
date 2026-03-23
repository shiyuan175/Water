using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Firebase.Crashlytics;
using Firebase.Analytics;
using Firebase.Extensions;
using System.Threading.Tasks;

public class FirebaseManager : MonoSingleton<FirebaseManager>
{
    private Firebase.FirebaseApp app;

    public override void OnSingletonInit()
    {
        //Init();
    }

    public Task Init()
    {
        return Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
#if UNITY_EDITOR
                app = Firebase.FirebaseApp.Create();
#else
                app = Firebase.FirebaseApp.DefaultInstance;
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
#endif
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventType">1 => start ,2 => complete</param>
    /// <param name="levleId"></param>
    public void SendEvent(int eventType, int levleId)
    {
        var eventName = eventType switch
        {
            1 => $"level_start_{levleId}",
            2 => $"level_complete_{levleId}",
            _ => null
        };

        if (eventName == null || levleId > 10)
            return;

        FirebaseAnalytics.LogEvent(eventName);
    }
}
