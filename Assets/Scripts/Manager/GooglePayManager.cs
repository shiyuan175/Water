using QFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class GooglePayManager : MonoSingleton<GooglePayManager>, IDetailedStoreListener
{
    IStoreController m_StoreController; // The Unity Purchasing system.

    public override void OnSingletonInit()
    {
        //初始化产品之前还要初始化 UnityServices，否则会抛出警告，但不影响内购
        //UnityServices需要开启加入组织开启服务，用于数据分析
        InitializePurchasing();
    }

    /// <summary>
    /// 初始化产品
    /// </summary>
    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //初始化产品列表，这里要Google后台的产品列表一致
        builder.AddProduct("battlepass_vip", ProductType.Consumable);

        builder.AddProduct("dgpack_vip", ProductType.Consumable);

        builder.AddProduct("gift_1", ProductType.Consumable);
        builder.AddProduct("gift_2", ProductType.Consumable);
        builder.AddProduct("gift_3", ProductType.Consumable);
        builder.AddProduct("gift_4", ProductType.Consumable);
        builder.AddProduct("gift_5", ProductType.Consumable);
        builder.AddProduct("gift_6", ProductType.Consumable);

        builder.AddProduct("gold_1", ProductType.Consumable);
        builder.AddProduct("gold_2", ProductType.Consumable);
        builder.AddProduct("gold_3", ProductType.Consumable);
        builder.AddProduct("gold_4", ProductType.Consumable);
        builder.AddProduct("gold_5", ProductType.Consumable);
        builder.AddProduct("gold_6", ProductType.Consumable);

        builder.AddProduct("pg_gift1", ProductType.Consumable);
        builder.AddProduct("pg_gift2", ProductType.Consumable);
        builder.AddProduct("pg_gift3", ProductType.Consumable);
        builder.AddProduct("pg_gift4", ProductType.Consumable);
        builder.AddProduct("pg_gift5", ProductType.Consumable);
        builder.AddProduct("pg_gift6", ProductType.Consumable);
        builder.AddProduct("pg_gift7", ProductType.Consumable);
        builder.AddProduct("pg_gift8", ProductType.Consumable);
        builder.AddProduct("pg_gift9", ProductType.Consumable);

        builder.AddProduct("removeadpack_1", ProductType.Consumable);
        builder.AddProduct("sepecialoffer_1", ProductType.Consumable);


        UnityPurchasing.Initialize(this, builder);
    }

    //购买时调用的接口，外部只需调用这一个接口即可
    public void BuyProduct(string pruductid)
    {
        m_StoreController.InitiatePurchase(m_StoreController.products.WithID(pruductid));
    }

    /// <summary>
    /// 初始化成功
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        //初始化成功
        Debug.Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
    }

    /// <summary>
    /// 初始化失败(旧版)
    /// </summary>
    /// <param name="error"></param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        //初始化失败
        OnInitializeFailed(error, null);
    }

    /// <summary>
    /// 初始化失败(新版)
    /// </summary>
    /// <param name="error"></param>
    /// <param name="message"></param>
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        //初始化失败
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    /// <summary>
    /// 购买成功
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //Retrieve the purchased product
        var product = args.purchasedProduct;
        //Add the purchased product to the players inventory
        //付款成功，此处需要自行添加逻辑，
        StringEventSystem.Global.Send(product.definition.id);

        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// 购买失败(旧版)
    /// </summary>
    /// <param name="product"></param>
    /// <param name="failureReason"></param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        //付款失败
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    /// <summary>
    /// 购买失败(新版)
    /// </summary>
    /// <param name="product"></param>
    /// <param name="failureDescription"></param>
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        //付款失败
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }
}

