using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIContinueData : UIPanelData
	{
	}
	public partial class UIContinue : UIPanel, ICanGetUtility, ICanRegisterEvent,ICanSendEvent, IController
    {
        private SaveDataUtility saveData;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIContinueData ?? new UIContinueData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            TxtRetry.font = LevelManager.Instance.greenFont;
            TxtCoinCost.font = LevelManager.Instance.greenFont;
        }
		
		protected override void OnShow()
		{
			SetCoin();

            RegisterBtnEvent();

            this.RegisterEvent<CoinChangeEvent>(e =>
            {
                SetCoin();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            BtnContinue.onClick.RemoveAllListeners();
            BtnClose.onClick.RemoveAllListeners();
            BtnAddCoin.onClick.RemoveAllListeners();
        }

        private void RegisterBtnEvent()
		{
            BtnContinue.onClick.AddListener(() =>
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
            BtnClose.onClick.AddListener(() =>
            {
                saveData = this.GetUtility<SaveDataUtility>();
                /* CloseSelf();
                 UIKit.OpenPanel<UIDeleteLife>();*/
                string _del = $"用户退出关卡:{saveData.GetCurrentLevel()}," +
                 $"当前关卡进度:{saveData.GetCurrentLevel()}";
                AnalyticsManager.Instance.SendLevelEvent(_del);

                HealthManager.Instance.UseHp();
                //避免引导关退出的UI残留
                UIKit.ClosePanel<UIGuideAnimPop>();
                if (saveData.GetCurrentLevel() == 1 || saveData.GetCurrentLevel() == 2)
                {
                    UIKit.ClosePanel<UIGuideLevel1>();
                    UIKit.ClosePanel<UIGuideLevel2>();
                }
                UIKit.ClosePanel<UIGameNode>();
                this.GetModel<StageModel>().ResetCountinueWinNum();
                this.SendEvent<ReturnMainEvent>(new ReturnMainEvent());
                CloseSelf();


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
