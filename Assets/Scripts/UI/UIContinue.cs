using GameDefine;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class UIContinueData : UIPanelData
	{
	}
	public partial class UIContinue : UIPanel,ICanSendEvent, IController
    {
        private SaveDataUtility mSaveData;
        private StageModel mStageModel;
        private ResLoader mResLoader;
        private SpriteAtlas mRankLevelSpriteAtlas;

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
            TxtCoinCost.font = LevelManager.Instance.greenFont;
            mSaveData = this.GetUtility<SaveDataUtility>();
            mStageModel = this.GetModel<StageModel>();

            LoadRes();
            RegisterBtnEvent();
            StringEventSystem.Global.Register(GameDefine.GameConst.COIN_CHANGE, () =>
            {
                SetCoin();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        protected override void OnShow()
		{
            SetCoin();
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            BtnContinue.onClick.RemoveAllListeners();
            BtnQuit.onClick.RemoveAllListeners();
            BtnAddCoin.onClick.RemoveAllListeners();
            mSaveData = null;
            mStageModel = null;

            if (mResLoader != null)
            {
                mResLoader.Recycle2Cache();
                mResLoader = null;
                mRankLevelSpriteAtlas = null;
            }
        }

        private void LoadRes()
        {
            mResLoader = ResLoader.Allocate();
            mRankLevelSpriteAtlas = mResLoader.LoadSync<SpriteAtlas>
                (ABResourceDefine.RANK_LEVEL_ATLAS_BUNDLENAME, ABResourceDefine.RANK_LEVEL_ATLAS_ASSETNAME);

            var _rankStreakWin = mStageModel.InGameRankStreakWinNum;
            var _curRankIndex = Mathf.Min(8, Mathf.Max(0, (_rankStreakWin - 1) / 5));
            ImgRankIcon.sprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(_curRankIndex));
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
                
                string _del = $"用户退出关卡:{mSaveData.GetCurrentLevel()}," +
                 $"当前关卡进度:{mSaveData.GetCurrentLevel()}";
                AnalyticsManager.Instance.SendLevelEvent(_del);

                HealthManager.Instance.UseHp();
                //避免引导关退出的UI残留
                UIKit.ClosePanel<UIGuideAnimPop>();
                UIKit.ClosePanel<UIGameNode>();
                
                this.SendCommand<FailedLevelCommand>();
                this.SendEvent(new ReturnToMainEvent { PassLevel = false });

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
                // 没有计时的时候显示 并进行时间的初始化
                if (GameActivityManager.Instance.GetActivity<TurnTableADActivity>() is TurnTableADActivity turnTableADActivity
                    && turnTableADActivity.ActivityStatus == GameActivityStatus.Active && !GameUtils.DoesCountDownKeyExist(GameDefine.GameConst.TURNTABLE_AD_ACTIVITY_SIGN))
                {
                    GameActivityManager.Instance.GetActivity<TurnTableADActivity>().StartActivity();
                    UIKit.OpenPanel<UIMallTurntable>(new UIMallTurntableData { IsManagedOpen = true });
                    return true;
                }
                return false;
            });

            //开启
            PanelQueueManager.Instance.PopFirstPanel();
        }
    }
}
