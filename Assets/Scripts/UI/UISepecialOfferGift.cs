using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using DG.Tweening;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using System;

namespace QFramework.Example
{
	public class UISepecialOfferGiftData : UIPanelData
	{
        public bool? IsManagedOpen;
    }
	public partial class UISepecialOfferGift : UIPanel,ICanGetUtility
	{
		[SerializeField] private TextMeshProUGUI[] textRed;
		private Tween mCountDownTween;
		private SepecialOfferADActivity mSepecialOfferADActivity;
        private GooglePayManager googlePay;
        private Dictionary<string, Action> giftPackBuySuccessActions;
        private RewardGrantUtility rewardGrantUtility;
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UISepecialOfferGiftData ?? new UISepecialOfferGiftData();
			foreach (var i in textRed)
				i.font = LevelManager.Instance.redFont;

		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			mSepecialOfferADActivity = GameActivityManager.Instance.GetActivity<SepecialOfferADActivity>();
            googlePay = GooglePayManager.Instance;
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            // 初始化购买成功回调
            giftPackBuySuccessActions = new Dictionary<string, Action>();
            var _packSo = BtnBuy.GetComponent<GiftPack>().giftPack;
            giftPackBuySuccessActions[_packSo.ID] = () => OnPaySuccess(_packSo);
         
            SetBtnClick();
        }
		
		protected override void OnShow()
		{
            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mSepecialOfferADActivity.ActivityStatus == GameActivityStatus.Active)
                    Time_Red.text = mSepecialOfferADActivity.GetActivityReamingTime();
                else
                    Time_Red.text = "Finished";
            }, 1, 1f)
          .SetLoops(-1, LoopType.Restart)
          .SetUpdate(isIndependentUpdate: true);

        }

        protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }


		private void SetBtnClick()
		{
			BtnClose.onClick.RemoveAllListeners();
            BtnClose.onClick.AddListener(() =>
			{
				CloseSelf();
			});
            BtnBuy.onClick.RemoveAllListeners();
            BtnBuy.onClick.AddListener(() =>
			{
				Debug.Log(BtnBuy.GetComponent<GiftPack>());
				var _packSo = BtnBuy.GetComponent<GiftPack>().giftPack;
                googlePay.BuyProduct(_packSo.ID);
            });
		}
        /// <summary>
        /// 礼包购买成功回调
        /// </summary>
        private void OnPaySuccess(GiftPackSO _packSo)
        {
            rewardGrantUtility.GrantReward(_packSo);
            RewardUIManager.Instance.PlayRewardAnim(_packSo.Coins, true, null, _packSo);

            UIKit.OpenPanel<UIBuyPackSuccess>();
            ActionKit.Delay(1, () =>
            {
                UIKit.ClosePanel<UIShop>();//延迟1s等待协程结束关闭
            }).Start(this);
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
