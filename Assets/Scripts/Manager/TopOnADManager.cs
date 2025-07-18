using AnyThinkAds.Api;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MonoSingletonPath("[Analytics]/TopOnADManager")]
public class TopOnADManager: MonoSingleton<TopOnADManager>
{
    /*#region 原手动加载
    string mPlacementId_rewardvideo_all = "b1gfnjso5u3hej";
    string mPlacementId_interstitial_all = "b1gfnjso5u3p46";
    string mPlacementId_native_all = "b66da9af8356bb";
    string mPlacementId_splash_all = "b6604dcd293d1c";
    string showingScenario = "f65f152173a5c1";

    public delegate void RewardAction();
    public delegate void SplashCompleteAction();

    public RewardAction rewardAction;
    public SplashCompleteAction splashCompleteAction;

    public bool beginLoadSplash, isLoadBanner;
    public int AdBannerWidth = 640, AdBannerHeight = 100;

    public override void OnSingletonInit()
    {
        
    }

    private void Start()
    {
        AddAutoLoadAdPlacementID();;
    }

    public void LoadAD()
    {
        Debug.Log("加载广告");
        LoadRewardAD();
        LoadInterstitialAd();
        LoadBannerAd();
        //LoadSplashAD();
    }

    public void LoadRewardAD()
    {
        Debug.Log("加载奖励广告");

        ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "placement_custom_key", "placement_custom" } }, mPlacementId_rewardvideo_all);

        Dictionary<string, string> jsonmap = new Dictionary<string, string>();
        jsonmap.Add(ATConst.USERID_KEY, "test_user_id");
        jsonmap.Add(ATConst.USER_EXTRA_DATA, "test_user_extra_data");


        ATRewardedVideo.Instance.loadVideoAd(mPlacementId_rewardvideo_all, jsonmap);
    }

    public void LoadInterstitialAd()
    {

        Dictionary<string, object> jsonmap = new Dictionary<string, object>();

        ATInterstitialAd.Instance.loadInterstitialAd(mPlacementId_interstitial_all, jsonmap);
    }

    public void LoadBannerAd()
    {
        Dictionary<string, object> jsonmap = new Dictionary<string, object>();
        //配置Banner要展示的宽度，高度，是否使用pixel为单位（只针对iOS有效，Android 使用pixel为单位），注意不同平台的横幅广告有一定限制，例如配置的穿山甲横幅广告640*100，为了能填充完屏幕宽，计算高度H = (屏幕宽 *100)/640；那么在load的extra的size为（屏幕宽：H）。
        //ATSize bannerSize = new ATSize(1080, 900, true);
        ATSize bannerSize = new ATSize(AdBannerWidth, AdBannerHeight, true);
        jsonmap.Add(ATBannerAdLoadingExtra.kATBannerAdLoadingExtraBannerAdSizeStruct, bannerSize);
        jsonmap.Add(ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveWidth, bannerSize.width);
        jsonmap.Add(ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveOrientation, ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveOrientationCurrent);

        ATBannerAd.Instance.loadBannerAd(mPlacementId_native_all, jsonmap);

        
    }

    public void LoadSplashAD()
    {
        ATSplashAd.Instance.loadSplashAd(mPlacementId_splash_all, new Dictionary<string, object>());
        //ATSplashAd.Instance.client.onAdLoadFailureEvent += OnAdLoadSplashFailed;
        //ATSplashAd.Instance.client.onAdCloseEvent += onAdClose;
    }

    public void AddAutoLoadAdPlacementID()
    {
        ATRewardedVideo.Instance.client.onAdLoadEvent += OnAdLoad;
        ATRewardedVideo.Instance.client.onAdLoadFailureEvent += OnAdLoadFailure;
        ATRewardedVideo.Instance.client.onRewardEvent += OnReward;
        ATRewardedVideo.Instance.client.onAdVideoStartEvent += OnStart;
        ATRewardedVideo.Instance.client.onAdVideoCloseEvent += OnClosed;

        ATInterstitialAd.Instance.client.onAdLoadEvent += OnAdLoad;
        ATInterstitialAd.Instance.client.onAdLoadFailureEvent += OnInterstitialAdLoadFailure;
        ATInterstitialAd.Instance.client.onAdVideoEndEvent += OnReward;
        ATInterstitialAd.Instance.client.onAdVideoFailureEvent += OnReward;
        ATInterstitialAd.Instance.client.onAdVideoStartEvent += OnStart;
        ATInterstitialAd.Instance.client.onAdShowEvent += OnInterstitialClosed;

        ATBannerAd.Instance.client.onAdLoadEvent += OnAdLoad;
        ATBannerAd.Instance.client.onAdLoadFailureEvent += OnBannerAdLoadFailure;
        ATBannerAd.Instance.client.onAdImpressEvent += OnStart;


        ATSplashAd.Instance.client.onAdLoadEvent += OnSplashAdLoad;
        ATSplashAd.Instance.client.onAdLoadTimeoutEvent += OnSplashAdComplete;
        ATSplashAd.Instance.client.onAdLoadFailureEvent += OnSplashAdComplete;
        ATSplashAd.Instance.client.onAdCloseEvent += OnSplashAdComplete;

        //string[] jsonList = { mPlacementId_rewardvideo_all };
        //ATRewardedVideo.Instance.addAutoLoadAdPlacementID(jsonList);
    }

    public void ShowRewardAd()
    {
#if UNITY_EDITOR
        if(rewardAction != null)
        {
            rewardAction();
        }
        return;
#endif
        if(ATRewardedVideo.Instance.hasAdReady(mPlacementId_rewardvideo_all))
        {
            Debug.Log("播放广告");
            Dictionary<string, string> jsonmap = new Dictionary<string, string>();
            jsonmap.Add(AnyThinkAds.Api.ATConst.SCENARIO, showingScenario);
            ATRewardedVideo.Instance.showAd(mPlacementId_rewardvideo_all, jsonmap);

        }
        else
        {
            LoadRewardAD();
            //ShowAd();
        }
    }

    public void ShowInterstitialAd()
    {
            Debug.Log("准备播放广告");
        if(ATInterstitialAd.Instance.hasInterstitialAdReady(mPlacementId_interstitial_all))
        {
            Debug.Log("播放广告");
            ATInterstitialAd.Instance.showInterstitialAd(mPlacementId_interstitial_all);
        }
        else
        {
            LoadInterstitialAd();
            //ShowAd();
        }
    }

    public void ShowBannerAd()
    {
        //if(!isLoadBanner)
        //{
            isLoadBanner = true;
            //ATBannerAd.Instance.showBannerAd(mPlacementId_native_all, ATBannerAdLoadingExtra.kATBannerAdShowingPisitionBottom);
            ATBannerAd.Instance.showBannerAd(mPlacementId_native_all, ATBannerAdLoadingExtra.kATBannerAdShowingPisitionBottom);
            ATRect arpuRect = new ATRect((Screen.width - 320) / 2, Screen.height - 200, AdBannerWidth, AdBannerHeight, true);
            //ATBannerAd.Instance.showBannerAd(mPlacementId_native_all, arpuRect);
        //}
        //else
        //{
        //    ATBannerAd.Instance.showBannerAd(mPlacementId_native_all);
        //}

    }

    public void RemoveBannerAd()
    {
        ATBannerAd.Instance.cleanBannerAd(mPlacementId_native_all);
        LoadBannerAd();
    }

    public void HideBannerAd()
    {
        ATBannerAd.Instance.hideBannerAd(mPlacementId_native_all);
    }

    public void ReshowBannerAd()
    {
        ATBannerAd.Instance.showBannerAd(mPlacementId_native_all);
    }

    public void OnAdLoadFailure(object sender, ATAdEventArgs erg)
    {
        LoadRewardAD();
    }

    public void OnInterstitialAdLoadFailure(object sender, ATAdEventArgs erg)
    {
        LoadInterstitialAd();
    }

    public void OnBannerAdLoadFailure(object sender, ATAdEventArgs erg)
    {
        LoadBannerAd();
    }

    public void OnAdLoad(object sender, ATAdEventArgs erg)
    {
        Debug.Log("OnAdLoad " + erg.callbackInfo.adsource_id);
    }

    public void OnSplashAdLoad(object sender, ATAdEventArgs erg)
    {
        //
        if(erg.isTimeout)
        {
        }
        else
        {
            StartCoroutine(OpenStart(erg));
        }

        //splashCompleteAction();
    }

    IEnumerator OpenStart(ATAdEventArgs erg)
    {
        yield return new WaitForEndOfFrame();
        ATSplashAd.Instance.showSplashAd(mPlacementId_splash_all, new Dictionary<string, object>());

        Debug.Log("erg.isTimeout " + erg.isTimeout);
    }

    public void OnSplashAdComplete(object sender, ATAdEventArgs erg)
    {
        Debug.Log("OnSplashAdComplete");
    }

    public void OnClosed(object sender, ATAdEventArgs erg)
    {
        Debug.Log("OnClosed " + erg.callbackInfo.adsource_id);
        LoadRewardAD();
    }

    public void OnInterstitialClosed(object sender, ATAdEventArgs erg)
    {
        Debug.Log("OnClosed " + erg.callbackInfo.adsource_id);
        LoadInterstitialAd();
    }

    public void OnStart(object sender, ATAdEventArgs erg)
    {
        Debug.Log("StartAD " + erg.callbackInfo.adsource_id);
    }

    public void OnReward(object sender, ATAdEventArgs erg)
    {
        Debug.Log("广告激励");
        StartCoroutine(OnReward());
    }

    public IEnumerator OnReward()
    {
        yield return new WaitForEndOfFrame();

        if (rewardAction != null)
        {
            rewardAction();
        }
    }
    #endregion*/

    //全自动加载插屏广告和激励广告
    private readonly string mPlacementId_rewardvideo_all = "b1gfnjso5u3hej";      //激励广告ID
    private readonly string mPlacementId_interstitial_all = "b1gfnjso5u3p46";     //插屏广告ID
    //private readonly string mPlacementId_rewardvideo_all = "b1fn8aua8i1k5i";
    //private readonly string mPlacementId_interstitial_all = "b1fn8aua8i1s42";
    private Action videoRewardAction;
    private Action videoCloseAction;

    private Action intersRewardAction;
    private Action intersCloseAction;

    public override void OnSingletonInit()
    {
        ATSDKAPI.initSDK("a68229c5f42e2b", "a82844bf085483dd3018ef16e347250e5");  //AppID和AppKey
        //ATSDKAPI.initSDK("a667e2369f1df6", "a5e2ae5036721db4f108aef055cbe44c3");
        ATSDKAPI.setLogDebug(false);
        //Debug.Log("初始化SDK");

        addAutoLoadAdPlacementID();
    }

    public void addAutoLoadAdPlacementID()
    {
        ATRewardedAutoVideo.Instance.client.onRewardEvent += onAdVideoReward;
        ATRewardedAutoVideo.Instance.client.onAdVideoCloseEvent += onAdVideoClosedEvent;
        ATRewardedAutoVideo.Instance.client.onAdLoadFailureEvent += onAdVideoLoadFail;

        ATInterstitialAutoAd.Instance.client.onAdShowEvent += onAdIntersShow;
        ATInterstitialAutoAd.Instance.client.onAdCloseEvent += onAdIntersSClose;
        ATInterstitialAutoAd.Instance.client.onAdLoadFailureEvent += onAdIntersLoadFail;

        //全托管加载激励视频、插屏广告
        ATInterstitialAutoAd.Instance.addAutoLoadAdPlacementID(new string[] { mPlacementId_interstitial_all });
        ATRewardedAutoVideo.Instance.addAutoLoadAdPlacementID(new string[] { mPlacementId_rewardvideo_all });
        //Debug.Log("自动加载广告");
    }

    #region 插屏广告

    private void onAdIntersSClose(object sender, ATAdEventArgs e)
    {
        intersCloseAction?.Invoke();
        intersCloseAction = null;
    }

    private void onAdIntersShow(object sender, ATAdEventArgs e)
    {
        intersRewardAction?.Invoke();
        intersRewardAction = null;
    }

    private void onAdIntersLoadFail(object sender, ATAdErrorEventArgs e)
    {
        //Debug.Log("插屏广告111111加载失败");
    }

    #endregion

    #region 激励视频

    private void onAdVideoReward(object sender, ATAdEventArgs e)
    {
        videoRewardAction?.Invoke();
        videoCloseAction = null;
    }

    private void onAdVideoClosedEvent(object sender, ATAdRewardEventArgs e)
    {
        videoCloseAction?.Invoke();
        videoCloseAction = null;
    }

    private void onAdVideoLoadFail(object sender, ATAdErrorEventArgs e)
    {
        //Debug.Log("激励视频2222222加载失败");
    }

    #endregion

    public bool ShowVideoAd(Action rewardAction, Action onAdClose)
    {
        var hasAd = ATRewardedAutoVideo.Instance.autoLoadRewardedVideoReadyForPlacementID(mPlacementId_rewardvideo_all);
        //Debug.Log("是否有激励广告缓存：" + hasAd);
        if (hasAd)
        {
            ATRewardedAutoVideo.Instance.showAutoAd(mPlacementId_rewardvideo_all);
            videoRewardAction = rewardAction;
            videoCloseAction = onAdClose;
            return true;
        }

        return false;
    }

    public bool ShowIntersAd(Action rewardAction, Action onAdClose)
    {
        var hasAd = ATInterstitialAutoAd.Instance.autoLoadInterstitialAdReadyForPlacementID(mPlacementId_interstitial_all);
        //Debug.Log("是否有插屏广告缓存：" + hasAd);
        if (hasAd)
        {
            ATInterstitialAutoAd.Instance.showAutoAd(mPlacementId_interstitial_all);
            intersRewardAction = rewardAction;
            intersCloseAction = onAdClose;
            return true;
        }
        return false;
    }
}
