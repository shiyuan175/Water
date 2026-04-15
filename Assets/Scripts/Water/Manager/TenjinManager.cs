using QFramework;

namespace Game.Water
{
    public class TenjinManager: MonoSingleton<TenjinManager>
    {
        public enum TenjinCustomEvent
        {
            PassLevel
        }

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

        private void TenjinConnect()
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

        public void TopOnImpressionFromJSON(string json)
        {
            instance.TopOnImpressionFromJSON(json);
        }

        public void SendCustomEvent(string eventName, string eventValue)
        {
            instance.SendEvent(eventName, eventValue);
        }
    }
}
