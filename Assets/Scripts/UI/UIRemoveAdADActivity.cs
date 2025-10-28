using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;
using System;
using UnityEngine.Purchasing;
using TMPro;

namespace QFramework.Example
{
    public class UIRemoveAdADActivityData : UIPanelData
    {
        public bool? IsManagedOpen;
    }

    public partial class UIRemoveAdADActivity : UIPanel,ICanGetUtility,ICanGetModel
    {
        private GooglePayManager googlePay;
        private Dictionary<string, Action> giftPackBuySuccessActions;
        private RewardGrantUtility rewardGrantUtility;
        private RemoveADACtivityModel removeADModel;
        [SerializeField]
        TextMeshProUGUI[] redText;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIRemoveAdADActivityData ?? new UIRemoveAdADActivityData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            googlePay = GooglePayManager.Instance;
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            removeADModel = this.GetModel<RemoveADACtivityModel>();

            foreach (var i in redText)
                i.font = LevelManager.Instance.redFont;

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

            // 多余处理
            if (removeADModel.IsBuy == true)
                BtnBuy.interactable = false;
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
            removeADModel.BuyGift();
            rewardGrantUtility.GrantReward(_packSo);
            RewardUIManager.Instance.PlayRewardAnim(_packSo.Coins, true, null, _packSo);

            UIKit.OpenPanel<UIBuyPackSuccess>();
            ActionKit.Delay(1, () =>
            {
                CloseSelf();//延迟1s等待协程结束关闭
            }).Start(this);
        }
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
