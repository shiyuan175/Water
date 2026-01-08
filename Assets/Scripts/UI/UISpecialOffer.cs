using GameGlobalJson;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
    public class UISpecialOfferData : UIPanelData
    {
        public bool? IsManagedOpen;
    }

    public partial class UISpecialOffer : UIPanel, ICanGetModel, ICanGetUtility
    {
        //[SerializeField] private TextMeshProUGUI[] textRed;
        [SerializeField] private TextMeshProUGUI mPriceTmp;
        [SerializeField] private GiftPackSO[] mPrivilegePacks;
        [SerializeField] private GameObject[] mPrivilegePackPanel;

        private int mLowestUnboughtPack;
        private GooglePayManager mGooglePay;
        private GameGlobalModel mGameGlobalModel;
        private RewardGrantUtility mRewardGrantUtility;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UISpecialOfferData ?? new UISpecialOfferData();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            //foreach (TextMeshProUGUI i in textRed)
            //    i.font = LevelManager.Instance.redFont;

            mPriceTmp.font = LevelManager.Instance.redFont;
            mGooglePay = GooglePayManager.Instance;
            mGameGlobalModel = this.GetModel<GameGlobalModel>();
            mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();

            UpdatePurchasedPackIndex();
            // 注册购买成功事件
            StringEventSystem.Global.Register(mPrivilegePacks[mLowestUnboughtPack].ID, OnPaySuccess).UnRegisterWhenGameObjectDestroyed(gameObject);
            SetBtnClick();
        }

        protected override void OnShow()
        {
            mPrivilegePackPanel[mLowestUnboughtPack].Show();
            mPriceTmp.text = GetPackPrice();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnClose.onClick.RemoveAllListeners();
            BtnBuy.onClick.RemoveAllListeners();

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }

        private void SetBtnClick()
        {
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            BtnBuy.onClick.AddListener(() =>
            {
                mGooglePay.BuyProduct(mPrivilegePacks[mLowestUnboughtPack].ID);
            });
        }

        /// <summary>
        /// 礼包购买成功回调
        /// </summary>
        private void OnPaySuccess()
        {
            mRewardGrantUtility.GrantReward(mPrivilegePacks[mLowestUnboughtPack]);
            mGameGlobalModel.SetFieldAndSave(JsonType.GameGlobalJson,
                mGameGlobalModel.GameGlobalJsonData.GiftPackPurchases,
                mPrivilegePacks[mLowestUnboughtPack].ID, true);
            RewardUIManager.Instance.PlayRewardAnim(mPrivilegePacks[mLowestUnboughtPack].Coins, true, null, mPrivilegePacks[mLowestUnboughtPack]);
            UIKit.OpenPanel<UIBuyPackSuccess>();
            CloseSelf();
        }

        /// <summary>
        /// 获取未购买的最低档位礼包索引
        /// </summary>
        private void UpdatePurchasedPackIndex()
        {
            var giftPacks = mGameGlobalModel.GameGlobalJsonData.GiftPackPurchases;
            mLowestUnboughtPack = giftPacks switch
            {
                { gift_1: false } => 0,
                { gift_2: false } => 1,
                { gift_3: false } => 2,
                { gift_4: false } => 3,
                { gift_5: false } => 4,
                { gift_6: false } => 5,
                _ => -1 // 全部购买
            };
        }

        private string GetPackPrice()
        {
            return mLowestUnboughtPack switch
            {
                0 => "$0.99",
                1 => "$4.99",
                2 => "$9.99",
                3 => "$19.99",
                4 => "$49.99",
                5 => "$99.99",
                _ => null,
            };
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
