using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using System;
using DG.Tweening;
using TMPro;

namespace QFramework.Example
{
	public class UIDoubleGiftADActivityData : UIPanelData
	{
        public bool? IsManagedOpen;
    }
	public partial class UIDoubleGiftADActivity : UIPanel,ICanGetUtility
	{
        [SerializeField] TextMeshProUGUI[] redText;
        private GooglePayManager googlePay;
        private Tween mCountDownTween;
        private Dictionary<string, Action> giftPackBuySuccessActions;
        private RewardGrantUtility rewardGrantUtility;
        private DuobleGiftAdActivity mDoubleGiftADActivity;
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIDoubleGiftADActivityData ?? new UIDoubleGiftADActivityData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            foreach(var txt in redText)
            {
                txt.font = LevelManager.Instance.redFont;
            }
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            googlePay = GooglePayManager.Instance;
            // 初始化购买成功回调
            giftPackBuySuccessActions = new Dictionary<string, Action>();
            var _packSo = BtnBuy.GetComponent<GiftPack>().giftPack;
            giftPackBuySuccessActions[_packSo.ID] = () => OnPaySuccess(_packSo);

            mDoubleGiftADActivity = GameActivityManager.Instance.GetActivity<DuobleGiftAdActivity>();

            SetBtnClick();

         /*   mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mDoubleGiftADActivity.ActivityStatus == GameActivityStatus.Active)
                    Time_Red.text = mDoubleGiftADActivity.GetActivityReamingTime();
                else
                    Time_Red.text = "Finished";
            }, 1, 1f)
          .SetLoops(-1, LoopType.Restart)
          .SetUpdate(isIndependentUpdate: true);*/

        }
		
		protected override void OnShow()
		{		
            // 注册购买成功事件
            foreach (var kvp in giftPackBuySuccessActions)
            {
                StringEventSystem.Global.Register(kvp.Key, kvp.Value).UnRegisterWhenGameObjectDestroyed(gameObject);
            }
        }
		
		protected override void OnHide()
		{
            // 卸载购买成功事件(避免从UIKit打开商店购买导致重复发放奖励)
            foreach (var kvp in giftPackBuySuccessActions)
            {
                StringEventSystem.Global.UnRegister(kvp.Key, kvp.Value);
            }
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
                var _packSo = BtnBuy.GetComponent<GiftPack>().giftPack;
                googlePay.BuyProduct(_packSo.ID);
            });
            BtnFree.onClick.RemoveAllListeners();
            BtnFree.onClick.AddListener(() =>
            {
                mDoubleGiftADActivity.GotFreeGift();
                var _packSo = BtnBuy.GetComponent<GiftPack>().giftPack;
                rewardGrantUtility.GrantReward(_packSo);
                RewardUIManager.Instance.PlayRewardAnim(_packSo.Coins, true, null, _packSo);
                BtnFree.interactable = false;
            });
        }
        private void OnPaySuccess(GiftPackSO _packSo)
        {
            rewardGrantUtility.GrantReward(_packSo);
            BtnFree.interactable = true;
            BtnBuy.interactable = false;
            RewardUIManager.Instance.PlayRewardAnim(_packSo.Coins, true, null, _packSo);
            mDoubleGiftADActivity.BuyGift();
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
