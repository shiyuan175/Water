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
        private BannerActivity mBannerActivity;

        private int mLastRankingScore;
        private bool mRankingEnd;

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
            string _del = $"用户通过关卡:{this.GetUtility<SaveDataUtility>().GetCurrentLevel() - 1}," +
                $"当前关卡进度:{this.GetUtility<SaveDataUtility>().GetCurrentLevel()}";
            AnalyticsManager.Instance.SendLevelEvent(_del);

            mBannerActivity = GameActivityManager.Instance.GetActivity<BannerActivity>();
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
                EnqueueAllPanels();
                CloseSelf();
            }).Start(this);
        }

        private void EnqueueAllPanels()
        {
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
                if (GameActivityManager.Instance.GetActivity<VolcanicActivity>() is VolcanicActivity volcanicActivity
                    && volcanicActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIVolcanicActivity>(new UIVolcanicActivityData()
                    {
                        isSuceed = true,
                        IsManagedOpen = true,
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
                        isSuceed = true,
                        IsManagedOpen = true
                    });
                    return true;
                }

                return false;
            });
            //轮盘活动
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
            //高塔活动
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (GameActivityManager.Instance.GetActivity<HighTowerActivity>() is HighTowerActivity highTowerActivity
                    && highTowerActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    UIKit.OpenPanel<UIHighTowerActivity>(new UIHighTowerActivityData()
                    {
                        isSuceed = true,
                        IsManagedOpen = true,
                    });
                    return true;
                }
                return false;
            });

            // 特惠礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (GameActivityManager.Instance.GetActivity<SepecialOfferADActivity>() is SepecialOfferADActivity sepecialOfferADActivity1
                   && sepecialOfferADActivity1.ActivityStatus == GameActivityStatus.Active && !GameUtils.DoesCountDownKeyExist(GameDefine.GameConst.SEPECIALOFFER_AD_ACTIVITY_SIGN))
                {
                    GameActivityManager.Instance.GetActivity<SepecialOfferADActivity>().StartActivity();
                    UIKit.OpenPanel<UISepecialOfferGift>(new UISepecialOfferGiftData()
                    {
                        IsManagedOpen = true,
                    });
                    return true;
                }
                return false;
            });
            // 阶梯礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (this.GetUtility<SaveDataUtility>().GetCurrentLevel() == GameDefine.GameConst.PG_AD_BEGIN_LEVEL)
                {
                    GameActivityManager.Instance.GetActivity<PrograssGiftADActivity>().StartActivity();
                    UIKit.OpenPanel<UIPrograssGiftADActivity>(new UIPrograssGiftADActivityData()
                    {
                        IsManagedOpen = true,
                    });
                    return true;
                }
                return false;
            });
            // 1+1礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (this.GetUtility<SaveDataUtility>().GetCurrentLevel() == GameDefine.GameConst.DG_AD_BEGIN_LEVEL)
                {
                    GameActivityManager.Instance.GetActivity<DuobleGiftAdActivity>().StartActivity();
                    UIKit.OpenPanel<UIDoubleGiftADActivity>(new UIDoubleGiftADActivityData()
                    {
                        IsManagedOpen = true,
                    });
                    return true;
                }
                return false;
            });

            // 免广告礼包
            PanelQueueManager.Instance.Enqueue(() =>
            {
                if (this.GetUtility<SaveDataUtility>().GetCurrentLevel() == GameDefine.GameConst.REMOVE_AD_BEGIN_LEVEL)
                {
                    UIKit.OpenPanel<UIRemoveAdADActivity>(new UIRemoveAdADActivityData()
                    {
                        IsManagedOpen = true,
                    });
                    return true;
                }
                return false;
            });

            //魔法连胜活动
            HandleMSA();

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
            else if (_activit.CurStageReward >= 20)
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
        }
    }
}
