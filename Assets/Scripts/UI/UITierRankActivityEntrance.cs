using DG.Tweening;
using GameDefine;
using QFramework;
using UnityEngine;
using UnityEngine.U2D;

namespace QFramework.Example
{
    public class UITierRankActivityEntranceData : UIPanelData
    {
    }

    public partial class UITierRankActivityEntrance : UIPanel, ICanGetUtility
    {
        [SerializeField] private RewardPackSO mStartActivitySO;
        [SerializeField] private RewardPackSO mRankSettlementSO;
        [SerializeField] private Sprite mSpriteGreyBtn;

        private ResLoader mResLoader;
        private SpriteAtlas mRankLevelSpriteAtlas;
        private RewardGrantUtility mRewardGrantUtility;
        private TierRankActivity mTierRankActivity;
        private Tween mCountDownTween;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UITierRankActivityEntranceData ?? new UITierRankActivityEntranceData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            mResLoader = ResLoader.Allocate();
            mRankLevelSpriteAtlas = mResLoader.LoadSync<SpriteAtlas>
                    (ABResourceDefine.RANK_LEVEL_ATLAS_BUNDLENAME, ABResourceDefine.RANK_LEVEL_ATLAS_ASSETNAME);

            mTierRankActivity = GameActivityManager.Instance.GetActivity<TierRankActivity>();
            mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();

            mCountDownTween = DOTween.To(() => 0, x =>
            {
                TxtCountDown.text = mTierRankActivity.GetActivityReamingTime();
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
            mResLoader.Recycle2Cache();
            mResLoader = null;

            mCountDownTween.Kill();
            mCountDownTween = null;
        }

        private void InitUI()
        {
            ImgRankSprite.sprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(mTierRankActivity.PlayerTierRankIndex));

            if (mTierRankActivity.ActivityStatus != SettlementActivityStatus.Inactive)
            {
                BtnStart.image.sprite = mSpriteGreyBtn;
                BtnStart.interactable = false;
            }
        }

        private void BindBtn()
        {
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            BtnStart.onClick.AddListener(() =>
            {
                System.Action _action = () =>
                {
                    UIKit.OpenPanel<UITierRankActivity>();
                    StringEventSystem.Global.Send(GameDefine.GameConst.START_TIER_RANK_ACTIVITY);
                    CloseSelf();
                };

                var _tuple = mTierRankActivity.RestartOneHourRankActivity();
                mRewardGrantUtility.GrantReward(mStartActivitySO);
                if (!_tuple.isRewardSettled)
                    mRewardGrantUtility.GrantReward(mRankSettlementSO);
                if (!_tuple.isFirstRank)
                {
                    CoinManager.Instance.AddCoin(mTierRankActivity.RankSettlementCoins);

                    RewardUIManager.Instance.PlayRewardAnim(mTierRankActivity.RankSettlementCoins, true, _action, mStartActivitySO, mRankSettlementSO);
                }
                else
                    RewardUIManager.Instance.PlayRewardAnim(null, true, _action, mStartActivitySO);

            });
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
