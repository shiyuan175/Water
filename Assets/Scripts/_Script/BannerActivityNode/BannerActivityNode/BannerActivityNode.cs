using DG.Tweening;
using GameDefine;
using QFramework;
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class BannerActivityNode : ViewController
	{
        private int mCacheProgress;
        private int mCacheGoal;

        private BannerActivity mBannerActivity;
        private RewardGrantUtility mRewardGrantUtility;

        private Sequence mProgressSequence;

        //五档积分底框位置
        private readonly int[] TARGER_POSX = new int[] { -280, -140, 0, 140, 280 };

        [SerializeField] private GiftPackSO[] mBannerActivityPackSO;

        private void Awake()
        {
            mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            mBannerActivity = GameActivityManager.Instance.GetActivity<BannerActivity>();

            TextProgress.font = LevelManager.Instance.blueFont;

            mCacheProgress = mBannerActivity.BARewardProgress;
            mCacheGoal = mBannerActivity.BACurrentGoal;
            TextProgress.text = $"{mCacheGoal}/{mBannerActivity.Reware_Target_Goals[mCacheProgress]}";
            ImgProgressBar.fillAmount = (float)mCacheGoal / mBannerActivity.Reware_Target_Goals[mCacheProgress];
            Selected.localPosition = new Vector3(TARGER_POSX[mBannerActivity.WinStreakLevel], Selected.localPosition.y, 0);
            TxtCurLevel.text = mBannerActivity.WinStreakPoints == 0 ?
                $"X1" : $"X{mBannerActivity.WinStreakPoints}";
        }

        private void OnEnable()
        {
            if (mBannerActivity.BACurrentGoal >= mBannerActivity.Reware_Target_Goals[mCacheProgress])
            {
                //需要遮罩等动画播放完
                UIKit.OpenPanel<UIMask>(UILevel.PopUI);
            }

            //更新位置
            if (mBannerActivity.ActivityStatus == GameActivityStatus.Active)
            {
                Selected.DOLocalMoveX(TARGER_POSX[mBannerActivity.WinStreakLevel], 1f)
                .OnComplete(() =>
                {
                    TxtCurLevel.text = mBannerActivity.WinStreakPoints == 0 ?
                    $"X1" : $"X{mBannerActivity.WinStreakPoints}";
                    DoUpdateProgress();
                });
            }
            else
                DoUpdateProgress();
        }

        private void Start()
        {
            //首次启用调用一次
            CheckOpenBox(mCacheGoal);
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;

            TextTimer.text = mBannerActivity.GetActivityReamingTime();
        }

        private void OnDisable()
        {
            Selected.DOKill();
            mProgressSequence?.Kill();
            mProgressSequence = null;

            if (mBannerActivity.ActivityStatus == GameActivityStatus.Active)
            {
                mCacheProgress = mBannerActivity.BARewardProgress;
                mCacheGoal = mBannerActivity.BACurrentGoal;
                if (mCacheProgress >= 0 && mCacheProgress < mBannerActivity.Reware_Target_Goals.Length)
                {
                    TextProgress.text = $"{mCacheGoal}/{mBannerActivity.Reware_Target_Goals[mCacheProgress]}";
                    ImgProgressBar.fillAmount = (float)mCacheGoal / mBannerActivity.Reware_Target_Goals[mCacheProgress];
                }
                Selected.localPosition = new Vector3(TARGER_POSX[mBannerActivity.WinStreakLevel], Selected.localPosition.y, 0);
                TxtCurLevel.text = mBannerActivity.WinStreakPoints == 0 ?
                    $"X1" : $"X{mBannerActivity.WinStreakPoints}";
                DoUpdateProgress();
            }
        }

        /// <summary>
        /// 更新文本、进度条
        /// </summary>
        private void DoUpdateProgress()
        {
            //获取当前进度和目标
            var _tempGoal = mBannerActivity.BACurrentGoal;

            if (_tempGoal != mCacheGoal)
            {
                int _startValue = mCacheGoal;

                mProgressSequence = DOTween.Sequence();
                mProgressSequence.Join(
                DOTween.To(() => _startValue, x =>
                {
                    _startValue = x;
                    TextProgress.text = $"{_startValue}/{mBannerActivity.Reware_Target_Goals[mCacheProgress]}";
                }, _tempGoal, 1f));

                mProgressSequence.Join(
                ImgProgressBar.DOFillAmount((float)_tempGoal / mBannerActivity.Reware_Target_Goals[mCacheProgress], 1f));

                mProgressSequence.OnComplete(() =>
                {
                    mCacheGoal = _tempGoal;
                    CheckOpenBox(_tempGoal);
                });
                mProgressSequence.Play();
            }
        }

        private void CheckOpenBox(int _tempGoal)
        {
            //判断是否开箱
            if (_tempGoal >= mBannerActivity.Reware_Target_Goals[mCacheProgress])
            {
                //奖励发放
                mRewardGrantUtility.GrantReward(mBannerActivityPackSO[mCacheProgress]);
                UIKit.ClosePanel<UIMask>();
                RewardUIManager.Instance.PlayRewardAnim(
                    mBannerActivityPackSO[mCacheProgress].Coins, true,
                    () =>
                    {
                        //活动进度更新
                        mBannerActivity.NextRewardProgress();

                        //越界(最后一档连胜)销毁节点
                        if (mBannerActivity.ProgressEnd)
                        {
                            //Debug.Log("已越界，销毁节点");
                            Destroy(gameObject);
                            return;
                        }

                        //未越界,重置进度
                        mCacheProgress = mBannerActivity.BARewardProgress;

                        _tempGoal = mBannerActivity.BACurrentGoal;
                        mCacheGoal = _tempGoal;
                        TextProgress.text = $"{_tempGoal}/{mBannerActivity.Reware_Target_Goals[mCacheProgress]}";
                        ImgProgressBar.fillAmount = (float)_tempGoal / mBannerActivity.Reware_Target_Goals[mCacheProgress];

                        ActionKit.DelayFrame(1, () => CheckOpenBox(mBannerActivity.BACurrentGoal)).Start(this);
                    },
                    mBannerActivityPackSO[mCacheProgress]);
            }
        }
    }
}
