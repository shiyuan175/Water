using QFramework;

namespace Game.Water
{
    public class TenjinManager: MonoSingleton<TenjinManager>
    {
        BaseTenjin instance;

        public override void OnSingletonInit()
        {
            TenjinConnect();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                TenjinConnect();
            }
        }

        public void TenjinConnect()
        {
            //Debug.Log("Tenjin StartConnect0");
            instance = Tenjin.getInstance("MFAFMW4JY4QGETYG3BTDD7XOAG5WIW3F");
            //Debug.Log("Tenjin StartConnect1");

#if UNITY_ANDROID
            //Debug.Log("Tenjin StartConnect2");

            instance.SetAppStoreType(AppStoreType.googleplay);
            // Sends install/open event to Tenjin
            instance.Connect();
            instance.SubscribeTopOnImpressions();
#endif
        }
    }
}
