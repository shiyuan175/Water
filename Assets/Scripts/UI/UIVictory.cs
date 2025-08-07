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
            TxtLevel.font = LevelManager.Instance.blueFont;

            string _del = $"用户通过关卡:{this.GetUtility<SaveDataUtility>().GetCurrentLevel() - 1}," +
				$"当前关卡进度:{this.GetUtility<SaveDataUtility>().GetCurrentLevel()}";
			AnalyticsManager.Instance.SendLevelEvent(_del);
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
			TxtLevel.text = (this.GetUtility<SaveDataUtility>().GetCurrentLevel() - 1).ToString();
			WaitClose();
        }

		protected override void OnHide()
		{
		}

		protected override void OnClose()
		{
            BtnSkip.onClick.RemoveAllListeners();
        }

		private	void ShowAnim()
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

            //...其他活动等

            //最后结算界面
            PanelQueueManager.Instance.Enqueue(() =>
            {
                UIKit.OpenPanel<UIGetCoin>();
                return true;
            });
            //开启
            PanelQueueManager.Instance.PopFirstPanel();
        }
    }
}
