using QFramework;

namespace Game.Water
{
    public class TenjinManager: MonoSingleton<TenjinManager>
    {
        private BaseTenjin instance;
        private bool mSubscribed = false;

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
            instance = Tenjin.getInstance("MFAFMW4JY4QGETYG3BTDD7XOAG5WIW3F");

#if UNITY_ANDROID

            instance.SetAppStoreType(AppStoreType.googleplay);
            instance.Connect();

            if (!mSubscribed)
            {
                instance.SubscribeTopOnImpressions();
                mSubscribed = true;
            }
#endif
        }
    }
}
