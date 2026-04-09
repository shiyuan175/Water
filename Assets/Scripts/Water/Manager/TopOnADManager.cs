using System;
using AnyThinkAds.Api;
using QFramework;
using QFramework.Example;
using UnityEngine;

namespace Game.Water
{
    [MonoSingletonPath("[Analytics]/TopOnADManager")]
    public class TopOnADManager: MonoSingleton<TopOnADManager>
    {
        private readonly string mPlacementId_rewardvideo_all = "n69b91770a54c6"; 
        private readonly string mPlacementId_interstitial_all = "n69b91770615d3"; 

        private Action videoRewardAction;
        private Action videoCloseAction;

        private Action intersRewardAction;
        private Action intersCloseAction;

        public override void OnSingletonInit()
        {
            ATSDKAPI.setLogDebug(false);
            ATSDKAPI.initSDK("h69b91727e8551", "ab2f30359f33af3578f607633fae31fca"); //AppID��AppKey

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

            //ȫ�йܼ��ؼ�����Ƶ���������
            ATInterstitialAutoAd.Instance.addAutoLoadAdPlacementID(new string[] { mPlacementId_interstitial_all });
            ATRewardedAutoVideo.Instance.addAutoLoadAdPlacementID(new string[] { mPlacementId_rewardvideo_all });
            //Debug.Log("�Զ����ع��");
        }

        #region �������

        private void onAdIntersSClose(object sender, ATAdEventArgs e)
        {
            TenjinManager.Instance.TopOnImpressionFromJSON(e.callbackInfo.getOriginJSONString());

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
            //Debug.Log("�������111111����ʧ��");
        }

        #endregion

        #region ������Ƶ

        private void onAdVideoReward(object sender, ATAdEventArgs e)
        {
            //var json = e.callbackInfo.getOriginJSONString();
            //Debug.Log(json);
            TenjinManager.Instance.TopOnImpressionFromJSON(e.callbackInfo.getOriginJSONString());
            videoRewardAction?.Invoke();
            videoRewardAction = null;
        }

        private void onAdVideoClosedEvent(object sender, ATAdRewardEventArgs e)
        {
            videoCloseAction?.Invoke();
            videoCloseAction = null;
        }

        private void onAdVideoLoadFail(object sender, ATAdErrorEventArgs e)
        {
            //Debug.Log("������Ƶ2222222����ʧ��");
        }

        #endregion

        public bool ShowVideoAd(Action rewardAction, Action onAdClose)
        {
            var hasAd = ATRewardedAutoVideo.Instance.autoLoadRewardedVideoReadyForPlacementID(mPlacementId_rewardvideo_all);
            // Debug.Log("�Ƿ��м�����滺�棺" + hasAd);
#if UNITY_EDITOR
            return true;
#endif
            if (hasAd)
            {
                ATRewardedAutoVideo.Instance.showAutoAd(mPlacementId_rewardvideo_all);
                videoRewardAction = rewardAction;
                videoCloseAction = onAdClose;
                return true;
            }

            UIKit.OpenPanel<UIPopUpWindow>();
            return false;
        }

        public bool ShowIntersAd(Action rewardAction, Action onAdClose)
        {
            var hasAd = ATInterstitialAutoAd.Instance.autoLoadInterstitialAdReadyForPlacementID(mPlacementId_interstitial_all);
            //Debug.Log("�Ƿ��в�����滺�棺" + hasAd);
            if (hasAd)
            {
                ATInterstitialAutoAd.Instance.showAutoAd(mPlacementId_interstitial_all);
                intersRewardAction = rewardAction;
                intersCloseAction = onAdClose;
                return true;
            }

            UIKit.OpenPanel<UIPopUpWindow>();
            return false;
        }
    }
}
