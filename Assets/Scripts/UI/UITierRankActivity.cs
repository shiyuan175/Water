using DG.Tweening;
using GameDefine;
using JsonFileData;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework.Example
{
	public class UITierRankActivityData : UIPanelData
	{
	}

	public partial class UITierRankActivity : UIPanel ,ICanGetUtility
	{
		[SerializeField] private GameObject[] mTRANodeCtrls;
        [SerializeField] private RewardPackSO mRankFirstPackSO;
        
        private RewardGrantUtility mRewardGrantUtility;
        private TierRankActivity mTierRankActivity;
        private Tween mCountDownTween;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITierRankActivityData ?? new UITierRankActivityData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            GameUtils.SotrArray(mTRANodeCtrls);
            TxtClaimReward_Red.font = LevelManager.Instance.redFont;
            TxtRewardTip_Blue.font = LevelManager.Instance.blueFont;

			mTierRankActivity = GameActivityManager.Instance.GetActivity<TierRankActivity>();
            mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();

            //倒计时
            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mTierRankActivity.ActivityStatus == SettlementActivityStatus.Active)
                    TxtCountDown.text = mTierRankActivity.GetHalfOneHourTierRankTime();
                else
                    TxtCountDown.text = "Finished";
            }, 1, 1f)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);

			InitUI();
			BindBtn();
        }

        protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            mCountDownTween.Kill();
            mCountDownTween = null;
			mTierRankActivity = null;
        }

		private void InitUI()
		{
            //排名
            var _allEntities = new List<object>();
            _allEntities.AddRange(mTierRankActivity.TRAData.TRARobots);
            _allEntities.Add(mTierRankActivity.TRAData.Player);

            var _sorted = _allEntities.OrderByDescending(x =>
            {
                if (x is TRAPlayer p) return p.StreamWinNum;
                if (x is TRARobotsData r) return r.StreamWinNum;
                return 0;
            }).ToList();

            for (int i = 0; i < _sorted.Count; i++)
            {
                var _traNode = mTRANodeCtrls[i].GetComponent<TRANodeCtrl>();

				if (_sorted[i] is TRAPlayer _player)
					_traNode.InitPlayer(_player);
				else
				{
                    TRARobotsData _robot = _sorted[i] as TRARobotsData;
                    _traNode.InitRobot(_robot);
                }
            }
          
            //按钮初始
            if (mTierRankActivity.ActivityStatus == SettlementActivityStatus.Finished
                && !mTierRankActivity.TRAData.Player.IsRewardSettled)
                BtnClaimReward.Show();
            else
                BtnClaimReward.Hide();
        }

        private void BindBtn()
        {
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            BtnClaimReward.onClick.AddListener(() =>
            {
                mRewardGrantUtility.GrantReward(mRankFirstPackSO);
                mTierRankActivity.MarkRewardAsSettled();
                UIKit.OpenPanel<UIMask>();

                RewardUIManager.Instance.PlayRewardAnim(null,true ,() =>
                {
                    UIKit.ClosePanel<UIMask>();
                    CloseSelf();
                }, mRankFirstPackSO);
            });

            BtnRewardInfo.onClick.AddListener(() =>
            {
                RewardInfo.Show();
            });

            BtnCloseRewardInfo.onClick.AddListener(() =>
            {
                RewardInfo.Hide();
            });
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
