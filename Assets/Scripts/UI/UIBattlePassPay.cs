using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Purchasing;

namespace QFramework.Example
{
	public class UIBattlePassPayData : UIPanelData
	{
	}
	public partial class UIBattlePassPay : UIPanel
	{
        private const string BPViP_GIFT_ID = "battlepass_vip";
		private GooglePayManager mGooglePay;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIBattlePassPayData ?? new UIBattlePassPayData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            mGooglePay = GooglePayManager.Instance;

            BtnBuy.onClick.AddListener(() =>
            {
                mGooglePay.BuyProduct(BPViP_GIFT_ID);
            });

			BtnClose.onClick.AddListener(() =>
			{
				CloseSelf();
			});

            StringEventSystem.Global.Register(BPViP_GIFT_ID, OnPaySuccess)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

        private void OnPaySuccess()
        {
			CloseSelf();
        }
    }
}
