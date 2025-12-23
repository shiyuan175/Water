using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using System;
using DG.Tweening;
using TMPro;
using GameDefine;

namespace QFramework.Example
{
	public class UIDoubleGiftADActivityData : UIPanelData
	{
        public bool? IsManagedOpen;
    }
	public partial class UIDoubleGiftADActivity : UIPanel,ICanGetUtility,ICanGetModel
	{
        [SerializeField] TextMeshProUGUI[] redText;
        [SerializeField] GiftPackSO mDGPackSO;
        [SerializeField] GiftPackSO mDGPackSO_Free;

        private GooglePayManager googlePay;
        private RewardGrantUtility rewardGrantUtility;
        private DuobleGiftAdActivity mDoubleGiftADActivity;
        private DoubleGiftADActivityModel mDPModel;

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

            mDPModel = this.GetModel<DoubleGiftADActivityModel>();
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            googlePay = GooglePayManager.Instance;
            mDoubleGiftADActivity = GameActivityManager.Instance.GetActivity<DuobleGiftAdActivity>();

            // 初始化购买成功回调
            StringEventSystem.Global.Register(mDGPackSO.ID, OnPaySuccess).UnRegisterWhenGameObjectDestroyed(this);
           
            SetBtnClick();
        }
		
		protected override void OnShow()
		{		
            if (mDPModel.IsBuy)
                BtnBuy.interactable = false;
            else
                BtnBuy.interactable = true;

            if(!mDPModel.GiftIsGot&&mDPModel.IsBuy)
                BtnFree.interactable = true;
            else
                BtnFree.interactable = false;
        }
		
		protected override void OnHide()
		{
        }

        protected override void OnClose()
		{
            BtnClose.onClick.RemoveAllListeners();
            BtnBuy.onClick.RemoveAllListeners();
            BtnFree.onClick.RemoveAllListeners();

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        
            StringEventSystem.Global.Send(GameConst.GIFT_PACK_ENTRY_STATE_CHANGED);
        }

        private void SetBtnClick()
		{
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            BtnBuy.onClick.AddListener(() =>
            {
                googlePay.BuyProduct(mDGPackSO.ID);
            });

            BtnFree.onClick.AddListener(() =>
            {
                mDoubleGiftADActivity.GotFreeGift();
                rewardGrantUtility.GrantReward(mDGPackSO_Free);
                RewardUIManager.Instance.PlayRewardAnim(mDGPackSO_Free.Coins, true, null, mDGPackSO_Free);
                BtnFree.interactable = false;
            });
        }

        private void OnPaySuccess()
        {
            rewardGrantUtility.GrantReward(mDGPackSO);
            BtnFree.interactable = true;
            BtnBuy.interactable = false;
            RewardUIManager.Instance.PlayRewardAnim(mDGPackSO.Coins, true, null, mDGPackSO);
            mDoubleGiftADActivity.BuyGift();
            UIKit.OpenPanel<UIBuyPackSuccess>();
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
