using System;
using AnyThinkAds.Api;
using QFramework;

namespace Game.Water
{
    [MonoSingletonPath("[Analytics]/TopOnADManager")]
    public class TopOnADManager: MonoSingleton<TopOnADManager>
    {
        //ȫ�Զ����ز������ͼ������
        private readonly string mPlacementId_rewardvideo_all = "b69718fcb63294"; //�������ID

        private readonly string mPlacementId_interstitial_all = "b69718fcc0a9fb"; //�������ID

        //private readonly string mPlacementId_rewardvideo_all = "b1gfnjso5u3hej";      //�������ID
        //private readonly string mPlacementId_interstitial_all = "b1gfnjso5u3p46";     //�������ID
        //private readonly string mPlacementId_rewardvideo_all = "b1fn8aua8i1k5i";
        //private readonly string mPlacementId_interstitial_all = "b1fn8aua8i1s42";
        private Action videoRewardAction;
        private Action videoCloseAction;

        private Action intersRewardAction;
        private Action intersCloseAction;

        public override void OnSingletonInit()
        {
            ATSDKAPI.setLogDebug(false);
            ATSDKAPI.initSDK("a69718fa406b36", "ae758ec796d75f4ff2fada0e795426b39"); //AppID��AppKey

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
            return false;
        }
    }
}
