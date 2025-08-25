using UnityEngine;
using UnityEngine.Android;

public static class PermissionAutoRequester
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RequestPermission()
    {
        //Notification permission application
        string permission = "android.permission.POST_NOTIFICATIONS";
        if (!Permission.HasUserAuthorizedPermission(permission))
        {
            Permission.RequestUserPermission(permission);
        }
    }
}
