using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIRetryData : UIPanelData
	{
	}
	public partial class UIRetry : UIPanel, ICanGetUtility, ICanRegisterEvent
    {
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIRetryData ?? new UIRetryData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            TxtGiveUp.font = LevelManager.Instance.redFont;
            TxtRetry.font = LevelManager.Instance.greenFont;
            TxtCoin.font = LevelManager.Instance.greenFont;
        }
		
		protected override void OnShow()
        {
            SetCoin();

            RegisterBtnEvent();

            StringEventSystem.Global.Register(GameDefine.GameConst.COIN_CHANGE,() =>
            {
                SetCoin();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            BtnGiveUp.onClick.RemoveAllListeners();
            BtnAddBottle.onClick.RemoveAllListeners();
            BtnAddCoin.onClick.RemoveAllListeners();
        }

        private void RegisterBtnEvent()
        {
            BtnGiveUp.onClick.AddListener(() =>
            {
                CloseSelf();
                UIKit.OpenPanel<UIContinue>();
            });

            BtnAddBottle.onClick.AddListener(() =>
            {
                if (CoinManager.Instance.Coin >= GameDefine.GameConst.ADD_BOTTLE_COST)
                {
                    //增加管子
                    LevelManager.Instance.AddBottle(false, () =>
                    {
                        CoinManager.Instance.CostCoin(GameDefine.GameConst.ADD_BOTTLE_COST);
                    });
                    CloseSelf();
                }
                else
                {
                    //唤起商店
                    UIKit.OpenPanel<UIShop>();
                }
            });

            BtnAddCoin.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIShop>();
            });
        }

        private void SetCoin()
        {
            var coin = CoinManager.Instance.Coin;
            TxtCoin.text = coin.ToString();
            TxtCoinCost.color = coin < GameDefine.GameConst.ADD_BOTTLE_COST ? Color.red : Color.white;
        }
    }
}
