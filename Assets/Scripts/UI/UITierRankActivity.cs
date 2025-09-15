using DG.Tweening;
using GameDefine;
using JsonFileData;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

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
        private ResLoader mResLoader;
        private SpriteAtlas mRankLevelSpriteAtlas;
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

            mResLoader = ResLoader.Allocate();
			mTierRankActivity = GameActivityManager.Instance.GetActivity<TierRankActivity>();
            mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();

            //倒计时
            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mTierRankActivity.ActivityStatus == SettlementActivityStatus.Active)
                    TxtCountDown.text = mTierRankActivity.GetOneHourTierRankTime();
                else
                    TxtCountDown.text = "Finished";
            }, 1, 1f)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);

            LoadRes();
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

            mResLoader.Recycle2Cache();
			mResLoader = null;
            mRankLevelSpriteAtlas = null;
			mTierRankActivity = null;
        }

		private void LoadRes()
		{
            mRankLevelSpriteAtlas = mResLoader.LoadSync<SpriteAtlas>
                  (ABResourceDefine.RANK_LEVEL_ATLAS_BUNDLENAME, ABResourceDefine.RANK_LEVEL_ATLAS_ASSETNAME);
        }

		private void InitUI()
		{
			var _playerTierSprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(mTierRankActivity.PlayerTierRankIndex));
			ImgTierRankIcon_Top.sprite = _playerTierSprite;

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
					_traNode.InitPlayer(_player, _playerTierSprite);
				else
				{
                    TRARobotsData _robot = _sorted[i] as TRARobotsData;
                    Sprite _tierSprite = mRankLevelSpriteAtlas.GetSprite(
                    GameUtils.GetAtlasSpriteName(mTierRankActivity.GetTierRankIndex(_robot.StreamWinNum)));

                    _traNode.InitRobot(_robot, _tierSprite);
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

                RewardUIManager.Instance.PlayRewardAnim(null, () =>
                {
                    UIKit.ClosePanel<UIMask>();
                    CloseSelf();
                }, mRankFirstPackSO);
            });
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
