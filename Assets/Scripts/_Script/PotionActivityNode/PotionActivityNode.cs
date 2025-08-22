using UnityEngine;
using QFramework;
using DG.Tweening;
using UnityEngine.UI;
using GameDefine;
using UnityEditor;

namespace QFramework.Example
{
    public partial class PotionActivityNode : ViewController
    {
        private int mCacheProgress;
        private int mCacheGoal;

        private CountDownTimerManager countDownTimerManager;
        private PotionActivityModel potionActivityModel;
        private RewardGrantUtility rewardGrantUtility;

        private Sequence mProgressSequence;

        //五档连胜积分
        private readonly int[] TARGER_GOALS = new int[] { 140, 500, 500, 500, 500 };
        //五档积分底框位置
        private readonly int[] TARGER_POSX = new int[] { -280, -140, 0, 140, 280 };

        [SerializeField] private RewardPackSO[] potionActivityPackSO;

        private void Awake()
        {
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            potionActivityModel = this.GetModel<PotionActivityModel>();
            countDownTimerManager = CountDownTimerManager.Instance;

            TextProgress.font = LevelManager.Instance.blueFont;

            mCacheProgress = potionActivityModel.PotionActivityProgress;
            mCacheGoal = potionActivityModel.PotionActivityGoal;
            TextProgress.text = $"{mCacheGoal}/{TARGER_GOALS[mCacheProgress]}";
            ImgProgressBar.fillAmount = (float)mCacheGoal / TARGER_GOALS[mCacheProgress];
            Selected.localPosition = new Vector3(TARGER_POSX[potionActivityModel.WinStreakLevel], Selected.localPosition.y, 0);
            TxtCurLevel.text = potionActivityModel.WinStreakPoints == 0 ? 
                $"X1" : $"X{potionActivityModel.WinStreakPoints}";
        }

        private void OnEnable()
        {
            if (potionActivityModel.PotionActivityGoal >= TARGER_GOALS[mCacheProgress])
            {
                //需要遮罩等动画播放完
                UIKit.OpenPanel<UIMask>(UILevel.PopUI);
            }
           
            //更新位置
            if (!countDownTimerManager.IsTimerFinished(GameConst.POTION_ACTIVITY_SIGN))
            {
                Selected.DOLocalMoveX(TARGER_POSX[potionActivityModel.WinStreakLevel], 1f)
                .OnComplete(() =>
                {
                    TxtCurLevel.text = potionActivityModel.WinStreakPoints == 0 ?
                    $"X1" : $"X{potionActivityModel.WinStreakPoints}";
                    DoUpdateProgress();
                });
            }
            else
            {
                //走动画逻辑触发
                DoUpdateProgress();
                //活动结束且积分满足，跳过动画直接走领奖逻辑
                //CheckOpenBox(potionActivityModel.PotionActivityGoal);
            }
        }

        private void Start()
        {
            //首次启用调用一次
            CheckOpenBox(mCacheGoal);
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;

            if (!countDownTimerManager.IsTimerFinished(GameDefine.GameConst.POTION_ACTIVITY_SIGN))
            {
                TextTimer.text = countDownTimerManager.GetRemainingTimeText(GameDefine.GameConst.POTION_ACTIVITY_SIGN);
            }
            else
            {
                TextTimer.text = "00:00:00";
                //极端情况:积分足够且正好活动结束
                if (potionActivityModel.PotionActivityGoal < TARGER_GOALS[mCacheProgress])
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }

        private void OnDisable()
        {
            Selected.DOKill();
            mProgressSequence?.Kill();
            mProgressSequence = null;

            if (!countDownTimerManager.IsTimerFinished(GameConst.POTION_ACTIVITY_SIGN))
            {
                mCacheProgress = potionActivityModel.PotionActivityProgress;
                mCacheGoal = potionActivityModel.PotionActivityGoal;
                if (mCacheProgress >= 0 && mCacheProgress < TARGER_GOALS.Length)
                {
                    TextProgress.text = $"{mCacheGoal}/{TARGER_GOALS[mCacheProgress]}";
                    ImgProgressBar.fillAmount = (float)mCacheGoal / TARGER_GOALS[mCacheProgress];
                }
                Selected.localPosition = new Vector3(TARGER_POSX[potionActivityModel.WinStreakLevel], Selected.localPosition.y, 0);
                TxtCurLevel.text = potionActivityModel.WinStreakPoints == 0 ?
                    $"X1" : $"X{potionActivityModel.WinStreakPoints}";
                DoUpdateProgress();
            }
        }

        /// <summary>
        /// 更新文本、进度条
        /// </summary>
        private void DoUpdateProgress()
        {
            //获取当前进度和目标
            var _tempGoal = potionActivityModel.PotionActivityGoal;

            if (_tempGoal != mCacheGoal)
            {
                int _startValue = mCacheGoal;

                mProgressSequence = DOTween.Sequence();
                mProgressSequence.Join(
                DOTween.To(() => _startValue, x =>
                {
                    _startValue = x;
                    TextProgress.text = $"{_startValue}/{TARGER_GOALS[mCacheProgress]}";
                }, _tempGoal, 1f));

                mProgressSequence.Join(
                ImgProgressBar.DOFillAmount((float)_tempGoal / TARGER_GOALS[mCacheProgress], 1f));

                mProgressSequence.OnComplete(() =>
                {
                    mCacheGoal = _tempGoal;
                    CheckOpenBox(_tempGoal);
                });
                mProgressSequence.Play();

                //使用容器控制
                /*
                mCacheGoal = _tempGoal;
                //等待动画完成
                ActionKit.Delay(1f, () =>
                {
                    CheckOpenBox(_tempGoal);
                }).Start(this);
                */
            }
        }

        private void CheckOpenBox(int _tempGoal)
        {
            //判断是否开箱
            if (_tempGoal >= TARGER_GOALS[mCacheProgress])
            {
                UIKit.ClosePanel<UIMask>();
                RewardUIManager.Instance.PlayRewardAnim(
                    potionActivityPackSO[mCacheProgress],
                    potionActivityPackSO[mCacheProgress].Coins,
                    () =>
                    {
                        //奖励发放
                        rewardGrantUtility.GrantReward(potionActivityPackSO[mCacheProgress]);

                        //活动进度更新
                        potionActivityModel.ReducePotionActivityGoal(TARGER_GOALS[mCacheProgress]);
                        potionActivityModel.AddPotionActivityProgress();
                        mCacheProgress = potionActivityModel.PotionActivityProgress;

                        //越界(最后一档连胜)销毁节点
                        if (mCacheProgress >= TARGER_GOALS.Length)
                        {
                            //Debug.Log("已越界，销毁节点");
                            Destroy(gameObject);
                            return;
                        }

                        //未越界,重置进度
                        _tempGoal = potionActivityModel.PotionActivityGoal;
                        mCacheGoal = _tempGoal;
                        TextProgress.text = $"{_tempGoal}/{TARGER_GOALS[mCacheProgress]}";
                        ImgProgressBar.fillAmount = (float)_tempGoal / TARGER_GOALS[mCacheProgress];

                        ActionKit.DelayFrame(1, () => CheckOpenBox(potionActivityModel.PotionActivityGoal)).Start(this);
                    });
            }
        }
    }
}
