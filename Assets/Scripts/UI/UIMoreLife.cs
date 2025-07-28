using UnityEngine;
using UnityEngine.UI;
using QFramework;
using GameDefine;

namespace QFramework.Example
{
	public class UIMoreLifeData : UIPanelData
	{
	}
	public partial class UIMoreLife : UIPanel, ICanGetUtility, ICanRegisterEvent
    {
        private const int MAX_HP_COST = 900;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMoreLifeData ?? new UIMoreLifeData();
			// please add init code here
		}

        protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			BindBtn();

            this.RegisterEvent<VitalityChangeEvent>(e =>
            {
                SetHpTxt();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            SetHpTxt();
        }

        protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            BtnClose.onClick.RemoveAllListeners();
            BtnCoinBuy.onClick.RemoveAllListeners();
            BtnAD.onClick.RemoveAllListeners();
        }

        private void BindBtn()
        {
            var coin = CoinManager.Instance.Coin;
            TxtCoinCost.color = coin < MAX_HP_COST ? Color.red : Color.white;

            BtnClose.onClick.AddListener(() =>
            {
                this.CloseSelf();
            });

            BtnCoinBuy.onClick.AddListener(() =>
            {
                if (HealthManager.Instance.IsMaxHp || HealthManager.Instance.UnLimitHp)
                    return;

                if (CoinManager.Instance.Coin < MAX_HP_COST)
                {
                    UIKit.ClosePanel<UIBeginSelect>();
                    CloseSelf();
                    StringEventSystem.Global.Send(GameConst.OPEN_SHOP_PANEL_EVENT);
                    return;
                }
                CoinManager.Instance.CostCoin(MAX_HP_COST, () =>
                {
                    HealthManager.Instance.SetNowHpToMax();
                });
                CloseSelf();
            });

            BtnAD.onClick.AddListener(() =>
            {
                if (HealthManager.Instance.IsMaxHp || HealthManager.Instance.UnLimitHp)
                    return;

                TopOnADManager.Instance.ShowVideoAd(() => HealthManager.Instance.AddHp(), null);
            });
        }

        private void Update()
        {
            TxtTime.text = HealthManager.Instance.UnLimitHp ? 
              HealthManager.Instance.UnLimitHpTimeStr : 
              HealthManager.Instance.RecoverTimerStr;
        }

        private void SetHpTxt()
        {
            TxtHpNum.text = HealthManager.Instance.UnLimitHp ? "¡Þ" : HealthManager.Instance.CurRecoverySlot.ToString();
            TxtDetail.text = HealthManager.Instance.UnLimitHp ? "Remaining Unlimited HP Time" : "Time To Next Life";
        }
    }
}
