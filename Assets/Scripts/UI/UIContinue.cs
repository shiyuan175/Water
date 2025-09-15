using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIContinueData : UIPanelData
	{
	}
	public partial class UIContinue : UIPanel,ICanSendEvent, IController
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
            SetWarringContent();
        }
		
		protected override void OnShow()
		{
            saveData = this.GetUtility<SaveDataUtility>();

            SetCoin();

            RegisterBtnEvent();

            StringEventSystem.Global.Register(GameDefine.GameConst.COIN_CHANGE, () =>
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
            BtnQuit.onClick.RemoveAllListeners();
            BtnAddCoin.onClick.RemoveAllListeners();
            saveData = null;
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
            BtnQuit.onClick.AddListener(() =>
            {
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
                this.GetModel<TierRankActivityModel>().ResetStreakWinNum();
                this.SendEvent<ReturnMainEvent>(new ReturnMainEvent());

                EnqueueAllPanels();
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

        private void EnqueueAllPanels()
        {
            //火山活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (GameActivityManager.Instance.GetActivity<VolcanicActivity>() is VolcanicActivity volcanicActivity &&
                volcanicActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIVolcanicActivity>(new UIVolcanicActivityData()
                    {
                        isSuceed = false,
                        IsManagedOpen = true
                    });
                    return true;
                }
                return false;
            });

            //火箭活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (GameActivityManager.Instance.GetActivity<RocketActivity>() is RocketActivity rocketActivity &&
                rocketActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIRocketActivity>(new UIRocketActivityData()
                    {
                        isSuceed = false,
                        IsManagedOpen = true
                    });
                    return true;
                }

                return false;
            });

            //高塔活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (GameActivityManager.Instance.GetActivity<HighTowerActivity>() is HighTowerActivity highTowerActivity &&
                highTowerActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIHighTowerActivity>(new UIHighTowerActivityData()
                    {
                        isSuceed = false,
                        IsManagedOpen = true
                    });
                    return true;
                }
                return false;
            });

            //魔法连胜
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (GameActivityManager.Instance.GetActivity<MagicStreakActivity>() is MagicStreakActivity MSA &&
                MSA.ActivityStatus == SettlementActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIMagicStreakActivity>(new UIMagicStreakActivityData()
                    {
                        ISWin = false,
                        IsManagedOpen = true,
                        Status = SettlementActivityStatus.Active
                    });
                    return true;
                }
                return false;
            });

            //...其他活动等
            // 转盘活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if(GameActivityManager.Instance.GetActivity<TurnTableADActivity>() is TurnTableADActivity TT &&
                TT.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIMallTurntable>(new UIMallTurntableData() { });
                    return true;
                }
                return false;
            });
            //开启
            PanelQueueManager.Instance.PopFirstPanel();

            
        }
        /// <summary>
        ///　设置提示文本
        /// </summary>
        private void SetWarringContent()
        {
            // 获取活动状态，对应设置文本，
            /*GameActivityManager.Instance.GetActivity;*/
            
        }
    }

    
}
