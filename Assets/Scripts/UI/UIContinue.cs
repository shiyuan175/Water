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
        private GameGlobalModel mGameGlobalModel;
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
            mGameGlobalModel = this.GetModel<GameGlobalModel>();

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
            mGameGlobalModel = null;

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

            var _rankStreakWin = mGameGlobalModel.InGameRankStreakWinNum;
            var _curRankIndex = Mathf.Min(8, Mathf.Max(0, (_rankStreakWin - 1) / 5));
            ImgRankIcon.sprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(_curRankIndex));
        }

        private void RegisterBtnEvent()
		{
            BtnContinue.onClick.AddListener(() =>
            {
                if (CoinManager.Instance.Coin >= GameDefine.GameConst.ADD_BOTTLE_COST)
                {
                    //���ӹ���
                    LevelManager.Instance.AddBottle(false, () =>
                    {
                        CoinManager.Instance.CostCoin(GameDefine.GameConst.ADD_BOTTLE_COST);
                    });
                    CloseSelf();
                }
                else
                {
                    //�����̵�
                    UIKit.OpenPanel<UIShop>();
                }

            });
            BtnQuit.onClick.AddListener(() =>
            {
                HealthManager.Instance.UseHp();
                //�����������˳���UI����
                UIKit.ClosePanel<UIGuideAnimPop>();
                UIKit.ClosePanel<UIGameNode>();
                
                this.SendCommand<FailedLevelCommand>();
                this.SendEvent(new ReturnToMainEvent { PassLevel = false });
                TypeEventSystem.Global.Send(new ReportLevelEvent
                {
                    level = mSaveData.GetCurrentLevel(),
                    type = 2,
                    iswin = 2
                });
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
            //��ɽ�
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

            //����
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

            //�����
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

            //ħ����ʤ
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

            //...�������
            /* ��ʱ�ر� // ת�̻
            PanelQueueManager.Instance.Enqueue(() =>
            {
                // û�м�ʱ��ʱ����ʾ ������ʱ��ĳ�ʼ��
                if (GameActivityManager.Instance.GetActivity<TurnTableADActivity>() is TurnTableADActivity turnTableADActivity
                    && turnTableADActivity.ActivityStatus == GameActivityStatus.Active && !GameUtils.DoesCountDownKeyExist(GameDefine.GameConst.TURNTABLE_AD_ACTIVITY_SIGN))
                {
                    GameActivityManager.Instance.GetActivity<TurnTableADActivity>().StartActivity();
                    UIKit.OpenPanel<UIMallTurntable>(new UIMallTurntableData { IsManagedOpen = true });
                    return true;
                }
                return false;
            });*/

            //����
            PanelQueueManager.Instance.PopFirstPanel();
        }
    }
}
