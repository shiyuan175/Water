using Google.MiniJSON;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenjinManager: MonoSingleton<TenjinManager>
{
    private BaseTenjin instance;
    private bool mSubscribe = false;

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
        instance = Tenjin.getInstance("C3AFW296ESTHECCHFBLL1BTSS6DQKYVA");

#if UNITY_ANDROID

        instance.SetAppStoreType(AppStoreType.googleplay);
        instance.Connect();

        if (!mSubscribe)
        {
            instance.SubscribeTopOnImpressions();
            mSubscribe = true;
        }
#endif
    }

    public void TopOnImpressionFromJSON(string json)
    {
        instance.TopOnImpressionFromJSON(json);
    }

    public void ReportIAPToTenjin(string productId, string currencyCode, decimal price, string receipt)
    {
        double unitPrice = decimal.ToDouble(price);

        var wrapper = Json.Deserialize(receipt) as Dictionary<string, object>;
        if (wrapper == null) return;

        var payload = (string)wrapper["Payload"];
        var googleDetails = Json.Deserialize(payload) as Dictionary<string, object>;

        var googleJson = (string)googleDetails["json"];
        var signature = (string)googleDetails["signature"];

        instance.Transaction(productId, currencyCode, 1, unitPrice, null, googleJson, signature);
    }
}
