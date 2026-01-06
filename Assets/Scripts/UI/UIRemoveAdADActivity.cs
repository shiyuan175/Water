using UnityEngine;
using TMPro;
using GameGlobalJson;

namespace QFramework.Example
{
    public class UIRemoveAdADActivityData : UIPanelData
    {
        public bool? IsManagedOpen;
    }

    public partial class UIRemoveAdADActivity : UIPanel,ICanGetUtility,ICanGetModel
    {
        private GooglePayManager googlePay;
        private RewardGrantUtility rewardGrantUtility;
        private GameGlobalModel mGameGlobalModel;
        [SerializeField] private AbilityGiftPackSO mRemoveAdPack;
        [SerializeField] private TextMeshProUGUI[] redText;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIRemoveAdADActivityData ?? new UIRemoveAdADActivityData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            googlePay = GooglePayManager.Instance;
            mGameGlobalModel = this.GetModel<GameGlobalModel>();
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();

            foreach (var i in redText)
                i.font = LevelManager.Instance.redFont;

            StringEventSystem.Global.Register(mRemoveAdPack.ID, OnPaySuccess).UnRegisterWhenGameObjectDestroyed(this);
           
            SetBtnClick();
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            StringEventSystem.Global.UnRegister(mRemoveAdPack.ID, OnPaySuccess);

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
                googlePay.BuyProduct(mRemoveAdPack.ID);
            });
        }

        private void OnPaySuccess()
        {
            rewardGrantUtility.GrantReward(mRemoveAdPack);
            //免广礼包视为礼包2的特权
            mGameGlobalModel.SetFieldAndSave(JsonType.GameGlobalJson,
                mGameGlobalModel.GameGlobalJsonData.GiftPackPurchases, "gift_2", true);
            RewardUIManager.Instance.PlayRewardAnim(mRemoveAdPack.Coins, true, null, mRemoveAdPack);
            UIKit.OpenPanel<UIBuyPackSuccess>();
            UIKit.ClosePanel(this);
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
