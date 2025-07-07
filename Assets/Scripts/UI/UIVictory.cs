using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using GameDefine;

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
			string _del = $"用户通过关卡:{this.GetUtility<SaveDataUtility>().GetLevelClear() - 1}," +
				$"当前关卡进度:{this.GetUtility<SaveDataUtility>().GetLevelClear()}";
			AnalyticsManager.Instance.SendLevelEvent(_del);
        }

		protected override void OnShow()
		{
            //通过第七关开启连胜活动
            if (this.GetUtility<SaveDataUtility>().GetLevelClear() == GameConst.WIN_STREAK_BEGIN_LEVEL)
            {
                StringEventSystem.Global.Send("StartPotionActivity");
                //开启排行榜活动
                CountDownTimerManager.Instance.StartTimer(GameConst.RANKA_ACTIVITY_SIGN, 1440f);
            }
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
                if (!mRankingEnd)
                    UIKit.OpenPanel<UIRankA>(new UIRankAData { LastRankScore = mLastRankingScore });
                else
                    UIKit.OpenPanel<UIGetCoin>();
                CloseSelf();
            });

            ShowAnim();
			TxtLevel.text = (this.GetUtility<SaveDataUtility>().GetLevelClear() - 1).ToString();
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
                if (!mRankingEnd)
                {
                    UIKit.OpenPanel<UIRankA>(new UIRankAData { LastRankScore = mLastRankingScore });
                }
                else
                    UIKit.OpenPanel<UIGetCoin>();
                CloseSelf();
            }).Start(this);
        }
    }
}
