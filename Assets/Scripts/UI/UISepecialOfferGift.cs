using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

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
        [SerializeField] private GiftPackSO mSpPackSO;

        private SepecialOfferADActivity mSepecialOfferADActivity;
        private GooglePayManager googlePay;
        private RewardGrantUtility rewardGrantUtility;
        private SepecialOfferADActivityModel mSOmodel;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UISepecialOfferGiftData ?? new UISepecialOfferGiftData();
            mSOmodel = this.GetModel<SepecialOfferADActivityModel>();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            foreach (TextMeshProUGUI i in textRed)
                i.font = LevelManager.Instance.redFont;

            mSepecialOfferADActivity = GameActivityManager.Instance.GetActivity<SepecialOfferADActivity>();
            googlePay = GooglePayManager.Instance;
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();

            // 注册购买成功事件
            StringEventSystem.Global.Register(mSpPackSO.ID, OnPaySuccess).UnRegisterWhenGameObjectDestroyed(gameObject);
            SetBtnClick();
        }

        protected override void OnShow()
        {
            if (mSOmodel.IsBuy)
                BtnBuy.interactable = false;
            else
                BtnBuy.interactable = true;
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
                googlePay.BuyProduct(mSpPackSO.ID);
            });
        }

        /// <summary>
        /// 礼包购买成功回调
        /// </summary>
        private void OnPaySuccess()
        {
            mSOmodel.BuyGift();
            rewardGrantUtility.GrantReward(mSpPackSO);
            RewardUIManager.Instance.PlayRewardAnim(mSpPackSO.Coins, true, null, mSpPackSO);
            mSepecialOfferADActivity.CoolDownActivity();
            CloseSelf();
            UIKit.OpenPanel<UIBuyPackSuccess>();
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
