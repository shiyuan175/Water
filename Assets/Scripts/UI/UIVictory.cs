using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using GameDefine;
using TMPro;

namespace QFramework.Example
{
    public class UIVictoryData : UIPanelData
    {
    }
    public partial class UIVictory : UIPanel, ICanSendEvent, ICanGetUtility, ICanGetModel
    {
        private int mLastRankingScore;
        private bool mRankingEnd;
        private bool mIsEnQueue;

        private SaveDataUtility mSaveDataUtility;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIVictoryData ?? new UIVictoryData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            mSaveDataUtility = this.GetUtility<SaveDataUtility>();
            string _del = $"用户通过关卡:{mSaveDataUtility.GetCurrentLevel() - 1}," +
                $"当前关卡进度:{mSaveDataUtility.GetCurrentLevel()}";
            AnalyticsManager.Instance.SendLevelEvent(_del);

            mIsEnQueue = false;
        }

        protected override void OnShow()
        {
            //避免界面停留时过期
            mRankingEnd = CountDownTimerManager.Instance.IsTimerFinished(GameConst.RANKA_ACTIVITY_SIGN);

            //连胜活动开启/排行榜开启状态
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameConst.POTION_ACTIVITY_SIGN)
                || !mRankingEnd)
            {
                var potionActivityModel = this.GetModel<PotionActivityModel>();
                mLastRankingScore = potionActivityModel.PotionActivityTotalGoal;
                potionActivityModel.AddPotionActivityGoal();
            }

            BtnSkip.onClick.AddListener(() =>
            {
                if (!mIsEnQueue)
                    EnqueueAllPanels();
                CloseSelf();
            });

            ShowAnim();

            WaitClose();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnSkip.onClick.RemoveAllListeners();
        }

        private void ShowAnim()
        {
            //目前不播放
            //AnimGo.Play("victoryAnim");
            HornGo1.Play("hornRotation");
            HornGo2.Play("hornRotation");
            HornGo3.Play("hornRotation");
            HornGo4.Play("hornRotation");

            HornSpine1.AnimationState.SetAnimation(0, "animation", false);
            HornSpine2.AnimationState.SetAnimation(0, "animation", false);
            HornSpine3.AnimationState.SetAnimation(0, "animation", false);
            HornSpine4.AnimationState.SetAnimation(0, "animation", false);
        }

        private void WaitClose()
        {
            ActionKit.Delay(3f, () =>
            {
                if (!mIsEnQueue)
                    EnqueueAllPanels();
                CloseSelf();
            }).Start(this);
        }

        private void EnqueueAllPanels()
        {
            mIsEnQueue = true;

            //排行榜
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (!mRankingEnd)
                {
                    UIKit.OpenPanel<UIRankA>(new UIRankAData
                    {
                        LastRankScore = mLastRankingScore,
                        IsManagedOpen = true,
                    });
                    return true;
                }
                return false;
            });
            //火山活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                VolcanicActivity volcanicActivity = GameActivityManager.Instance.GetActivity<VolcanicActivity>();

                if (volcanicActivity is null)
                    return false;

                if (volcanicActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIVolcanicActivity>(new UIVolcanicActivityData()
                    {
                        isSuceed = true,
                        IsManagedOpen = true,
                    });
                    return true;
                }

                else if (volcanicActivity.ActivityStatus == GameActivityStatus.Inactive)
                {
                    UIKit.OpenPanel<UIVolcanicActivityEntrance>(new UIVolcanicActivityEntranceData
                    {
                        IsManagedOpen = true
                    });
                    return true;
                }

                return false;
            });
            //火箭活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                RocketActivity rocketActivity = GameActivityManager.Instance.GetActivity<RocketActivity>();
                if (rocketActivity is null)
                    return false;

                if (rocketActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIRocketActivity>(new UIRocketActivityData()
                    {
                        isSuceed = true,
                        IsManagedOpen = true
                    });
                    return true;
                }
                else if (rocketActivity.ActivityStatus == GameActivityStatus.Inactive)
                {
                    UIKit.OpenPanel<UIRocketActivityEntrance>(new UIRocketActivityEntranceData
                    {
                        IsManagedOpen = true
                    });
                    return true;
                }

                return false;
            });
            //高塔活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                HighTowerActivity highTowerActivity = GameActivityManager.Instance.GetActivity<HighTowerActivity>();
                if (highTowerActivity is null)
                    return false;

                if (highTowerActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIHighTowerActivity>(new UIHighTowerActivityData()
                    {
                        isSuceed = true,
                        IsManagedOpen = true,
                    });
                    return true;
                }
                else if (highTowerActivity.ActivityStatus == GameActivityStatus.Inactive)
                {
                    UIKit.OpenPanel<UIHighTowerActivityEntrance>(new UIHighTowerActivityEntranceData
                    {
                        IsManagedOpen = true
                    });
                    return true;
                }

                return false;
            });
            //魔法连胜活动
            HandleMSA();

            // 特惠礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                var curLevel = mSaveDataUtility.GetCurrentLevel();
                int delta = curLevel - GameConst.SO_AD_BEGIN_LEVEL;

                //特权未全部购买 && 起始关 && 每七关弹出一次
                if (this.GetModel<GameGlobalModel>().IsAllPurchased() &&
                    curLevel != GameDefine.GameConst.SO_AD_BEGIN_LEVEL &&
                    (delta < 0 || delta % 7 != 0))
                    return false;
                UIKit.OpenPanel<UISpecialOffer>(new UISpecialOfferData
                {
                    IsManagedOpen = true,
                });
                return true;
            });
            // 阶梯礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                var curLevel = mSaveDataUtility.GetCurrentLevel();
                if (curLevel != GameDefine.GameConst.PG_AD_BEGIN_LEVEL) return false;
                GameActivityManager.Instance.GetActivity<PrograssGiftADActivity>().StartActivity();
                UIKit.OpenPanel<UIPrograssGiftADActivity>(new UIPrograssGiftADActivityData()
                {
                    IsManagedOpen = true,
                });
                return true;
            });
            // 1+1礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                var curLevel = mSaveDataUtility.GetCurrentLevel();
                if (curLevel != GameDefine.GameConst.DG_AD_BEGIN_LEVEL) return false;
                UIKit.OpenPanel<UIDoubleGiftADActivity>(new UIDoubleGiftADActivityData()
                {
                    IsManagedOpen = true,
                });
                return true;
            });
            // 免广告礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                var curLevel = mSaveDataUtility.GetCurrentLevel();
                if (curLevel != GameDefine.GameConst.REMOVE_AD_BEGIN_LEVEL) return false;
                UIKit.OpenPanel<UIRemoveAdADActivity>(new UIRemoveAdADActivityData()
                {
                    IsManagedOpen = true,
                });
                return true;
            });

            //最后结算界面
            PanelQueueManager.Instance.Enqueue(() =>
            {
                UIKit.OpenPanel<UIGetCoin>();
                return true;
            });
            PanelQueueManager.Instance.PopFirstPanel();
        }

        private void HandleMSA()
        {
            var _activit = GameActivityManager.Instance.GetActivity<MagicStreakActivity>();
            if (_activit is null)
                return;
            //阶段奖励全部领取完不弹出活动
            //(该活动的阶段奖励为20个)
            else if (_activit.CurStageReward >= 20 && _activit.ActivityStatus == SettlementActivityStatus.Active)
            {
                _activit.StreakWin();
                return;
            }

            bool _openPanel = false;
            UIMagicStreakActivityData _uiData = null;

            if (_activit.ActivityStatus == SettlementActivityStatus.Active)
            {
                _openPanel = true;
                _uiData = new UIMagicStreakActivityData
                {
                    ISWin = true,
                    IsManagedOpen = true,
                    Status = SettlementActivityStatus.Active
                };
            }

            //活动结束 有排名奖
            else if (_activit.ActivityStatus == SettlementActivityStatus.Finished)
            {
                _openPanel = true;
                _uiData = new UIMagicStreakActivityData
                {
                    HasRankRewardToSettle = true,
                    IsManagedOpen = true,
                    Status = SettlementActivityStatus.None
                };
            }
            /*//现交由Tick管理重启活动
            //活动结束 无排名奖/已结算
            //else if (_activit.ActivityStatus == SettlementActivityStatus.WaitStart)
            //{
            //    //重启活动
            //    _activit.RestartActivity();
            //}*/
            if (_openPanel)
            {
                PanelQueueManager.Instance.Enqueue(() =>
                {
                    UIKit.OpenPanel<UIMagicStreakActivity>(_uiData);
                    return true;
                });
            }

            if (_activit.ActivityStatus == SettlementActivityStatus.Inactive ||
               _activit.ActivityStatus == SettlementActivityStatus.WaitStart)
            {
                PanelQueueManager.Instance.Enqueue(() =>
                {
                    UIKit.OpenPanel<UIMagicStreakActivityEntrance>(new UIMagicStreakActivityEntranceData
                    {
                        IsManagedOpen = true
                    });
                    return true;
                });
                
            }
        }
    }
}
