using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Extensions;

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

    public void SendEvent(int eventType ,int levelID)
    {
        var eventName = eventType switch
        {
            1 => $"start_level_{levelID}",
            2 => $"start_complete_{levelID}",
            _ => null
        };

        if (eventName == null || levelID > 10)
            return;

        FirebaseAnalytics.LogEvent(eventName);
    }
}
