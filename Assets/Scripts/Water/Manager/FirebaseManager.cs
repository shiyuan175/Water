using System.Collections;
using System.Collections.Generic;
using Firebase.Crashlytics;
using UnityEngine;
using QFramework;
using Firebase.Extensions;

public class FirebaseManager : MonoSingleton<FirebaseManager>
{
    private Firebase.FirebaseApp app;

    public override void OnSingletonInit()
    {
        Init();
    }

    private void Init()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
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
}
