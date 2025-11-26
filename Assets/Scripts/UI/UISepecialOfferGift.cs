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

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
    public partial class UISepecialOfferGift : UIPanel, ICanGetModel, ICanGetUtility
    {
        [SerializeField] private TextMeshProUGUI[] textRed;
        private Tween mCountDownTween;
        private SepecialOfferADActivity mSepecialOfferADActivity;
        private GooglePayManager googlePay;
        private Dictionary<string, Action> giftPackBuySuccessActions;
        private RewardGrantUtility rewardGrantUtility;
        private SepecialOfferADActivityModel mSOmodel;
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UISepecialOfferGiftData ?? new UISepecialOfferGiftData();
            mSOmodel = this.GetModel<SepecialOfferADActivityModel>();

            foreach (TextMeshProUGUI i in textRed)
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
            // 注册购买成功事件
            foreach (var kvp in giftPackBuySuccessActions)
            {
                StringEventSystem.Global.Register(kvp.Key, kvp.Value).UnRegisterWhenGameObjectDestroyed(gameObject);
            }

            if (mSOmodel.IsBuy)
                BtnBuy.interactable = false;
            else
                BtnBuy.interactable = true;
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
        }
        /// <summary>
        /// 礼包购买成功回调
        /// </summary>
        private void OnPaySuccess(GiftPackSO _packSo)
        {
            rewardGrantUtility.GrantReward(_packSo);
            RewardUIManager.Instance.PlayRewardAnim(_packSo.Coins, true, null, _packSo);
            CloseSelf();
            mSOmodel.BuyGift();
            mSepecialOfferADActivity.CoolDownActivity();
            BtnBuy.interactable = false;
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
