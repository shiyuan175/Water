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
        [SerializeField] private GiftPackSO mStartActivitySO;
        [SerializeField] private GiftPackSO mRankSettlementSO;
        [SerializeField] private Sprite mSpriteGreyBtn;

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
            mCountDownTween.Kill();
            mCountDownTween = null;
        }

        private void InitUI()
        {
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
                    CloseSelf();
                };

                var _isRewardSettled = mTierRankActivity.RestartOneHourRankActivity();
                mRewardGrantUtility.GrantReward(mStartActivitySO);
                if (!_isRewardSettled)
                {
                    mRewardGrantUtility.GrantReward(mRankSettlementSO);
                    RewardUIManager.Instance.PlayRewardAnim(mStartActivitySO.Coins, true, _action, mStartActivitySO, mRankSettlementSO);
                }
                else
                    RewardUIManager.Instance.PlayRewardAnim(mStartActivitySO.Coins, true, _action, mStartActivitySO);

            });
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
