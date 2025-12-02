using DG.Tweening;
using GameDefine;
using QFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        private Image rewardImg;

        //五档连胜积分
        private readonly int[] TARGER_GOALS = new int[] { 140, 500, 500, 500, 1000, 2000, 2000, 2000 };
        //五档积分底框位置
        private readonly int[] TARGER_POSX = new int[] { -280, -140, 0, 140, 280 };

        [SerializeField] private GiftPackSO[] potionActivityPackSO;
        [SerializeField] private RectTransform mCupPos;
        [SerializeField] private Sprite mCoinSprite;

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

            UpdateGiftSprite();
        }

        private void OnEnable()
        {
            if (potionActivityModel.PotionActivityGoal >= TARGER_GOALS[mCacheProgress])
            {
                //需要遮罩等动画播放完
                UIKit.OpenPanel<UIMask>(UILevel.PopUI);
            }
           
            //更新位置
            if (!countDownTimerManager.IsTimerFinished(GameConst.POTION_ACTIVITY_SIGN)
                && potionActivityModel.PotionActivityGoal != mCacheGoal)
            {
                UIKit.OpenPanel<BannerActivityPop>(UILevel.PopUI, new BannerActivityPopData
                {
                    Goals = potionActivityModel.WinStreakPoints,
                    TargetPos = mCupPos.position
                });

                ActionKit.Delay(1.5f, () =>
                {
                    Selected.DOLocalMoveX(TARGER_POSX[potionActivityModel.WinStreakLevel], 1f)
                    .OnComplete(() =>
                    {
                        TxtCurLevel.text = potionActivityModel.WinStreakPoints == 0 ?
                        $"X1" : $"X{potionActivityModel.WinStreakPoints}";
                        DoUpdateProgress();
                    });
                }).Start(this);
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
                //DoUpdateProgress();
            }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
            if (rewardImg != null)
                RewardUIManager.Instance.Recycle(rewardImg);
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
                //奖励发放
                rewardGrantUtility.GrantReward(potionActivityPackSO[mCacheProgress]);
                //活动进度更新
                potionActivityModel.ReducePotionActivityGoal(TARGER_GOALS[mCacheProgress]);
                potionActivityModel.AddPotionActivityProgress();

                UIKit.ClosePanel<UIMask>();
                RewardUIManager.Instance.PlayRewardAnim(
                    potionActivityPackSO[mCacheProgress].Coins,true,
                    () =>
                    {
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

                        UpdateGiftSprite();
                    },
                    potionActivityPackSO[mCacheProgress]);
            }
        }
        
        //更新奖励图标
        //礼包只有一个奖励(特殊类型奖励/金币)
        //或是三个进关选择道具无限-时长相同
        private void UpdateGiftSprite()
        {
            if (rewardImg == null)
                rewardImg = RewardUIManager.Instance.Allocate();

            rewardImg.transform.SetParent(ImgRewardIcon.transform);
            rewardImg.transform.localScale = Vector3.one * 0.5f;
            rewardImg.TryGetComponent(out PropRewardPoolNode _node);

            if (potionActivityPackSO[mCacheProgress].SpecialRewards.Count != 0)
            {
                var duration = potionActivityPackSO[mCacheProgress].SpecialRewards[0].Duration;

                if (potionActivityPackSO[mCacheProgress].SpecialRewards.Count > 1)
                {
                    var rewardSprite = RewardUIManager.Instance.GetRewardSprite(SpecialRewardsType.Unlimited_S_ALL);
                    _node.Init(rewardSprite,Vector2.zero, duration, true);
                }
                else
                {
                    var rewardSprite = RewardUIManager.Instance.GetRewardSprite(potionActivityPackSO[mCacheProgress].SpecialRewards[0].SpecialRewardType);
                    _node.Init(rewardSprite, Vector2.zero, duration, true);
                }
            }
            else
            {
                var duration = potionActivityPackSO[mCacheProgress].Coins;
                var rewardSprite = RewardUIManager.Instance.GetRewardSprite(NormalRewardsType.AddCoins);
                _node.Init(rewardSprite, Vector2.zero, duration, false);
            }
        }
    }
}
