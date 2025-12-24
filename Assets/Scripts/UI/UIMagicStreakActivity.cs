using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using System.Collections.Generic;
using JsonFileData;
using DG.Tweening;
using GameDefine;
using TMPro;
using Spine;

namespace QFramework.Example
{
    public class UIMagicStreakActivityData : UIPanelData
    {
        public bool? ISWin;
        public bool? IsManagedOpen;
        public bool? HasRankRewardToSettle;
        public SettlementActivityStatus? Status;
    }

    public partial class UIMagicStreakActivity : UIPanel, ICanGetUtility ,ICanGetModel
    {
        [Serializable]
        private class MSARewardCofig
        {
            public int TriggerValue;
            public GiftPackSO RewardPack;
        }

        [Header("奖励配置")]
        [SerializeField] private List<MSARewardCofig> mStageRewardCofigs;
        [SerializeField] private List<MSARewardCofig> mRankRewardCofigs;
        [SerializeField] private RectTransform mRankPar;
        [SerializeField] private Sprite mCoinSprite;

        [Header("双倍效果表现UI")]
        [SerializeField] private GameObject mDoubleBuffPanel;
        [SerializeField] private GameObject mDoubleBuff;
        [SerializeField] private TextMeshProUGUI[] mPointsTier;
        //五档基础积分
        private readonly int[] POINTS_TIER = new int[] { 1, 5, 10, 25, 100 };
        
        //滑动块五档坐标
        private readonly int[] TARGER_POSX = new int[] { -294, -147, 0, 143, 286 };

        private List<GameObject> mRankNodePool;
        private List<Tween> mTweenList;
        private GameGlobalModel mGameGlobalModel;
        private MagicStreakActivity mMagicStreakActivity;
        private RewardGrantUtility mRewardGrantUtility;
        private int cacheStageRewardIndex;
        private int cachePlayerTotalScore;
        private int targetStageScore = -1;
        private int lastStageTotalScore = 0;

        private DG.Tweening.Sequence mSequence;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIMagicStreakActivityData ?? new UIMagicStreakActivityData();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            mRankNodePool = new List<GameObject>();
            mTweenList = new List<Tween>();

            //字体赋值
            TxtProgress_Red.font = LevelManager.Instance.redFont;
            TxtTitle_Blue.font = LevelManager.Instance.blueFont;

            mMagicStreakActivity = GameActivityManager.Instance.GetActivity<MagicStreakActivity>();
            mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            mGameGlobalModel = this.GetModel<GameGlobalModel>();

            //数据缓存
            CacheTempData();

            //计时器
            Tween _countDownTween = DOTween.To(() => 0, x =>
            {
                if (mMagicStreakActivity.ActivityStatus == SettlementActivityStatus.Active)
                    TxtCountDown.text = mMagicStreakActivity.GetActivityReamingTime();
                else
                    TxtCountDown.text = "Finished";
            }, 1, 1f)
           .SetLoops(-1, LoopType.Restart)
           .SetUpdate(true);
            mTweenList.Add(_countDownTween);

            UIInit();
        }

        protected override void OnShow()
        {
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            //活动结束结算
            if (mData.HasRankRewardToSettle ?? false)
            {
                UIKit.OpenPanel<UIMask>();
                TxtTitle_Blue.text = "Activity ended, ranking settlement.";
                RankNodeInit();
                mMagicStreakActivity.MarkRewardAsSettled();
                var _packSO = GetRankPackSO(mMagicStreakActivity.PlayerRank);
                mRewardGrantUtility.GrantReward(_packSO);

                ActionKit.Delay(0.5f, () =>
                {
                    //延迟动画
                    UIKit.ClosePanel<UIMask>();
                    RewardUIManager.Instance.PlayRewardAnim(_packSO.Coins, true, null, _packSO);
                }).Start(this);
                return;
            }

            //由主页入口进入
            if (mData.ISWin == null)
            {
                RankNodeInit();
                return;
            }

            bool _stageRewardSign = false;
            if ((bool)mData.ISWin)
            {
                mMagicStreakActivity.StreakWin();

                //判定是否触发阶段奖励
                _stageRewardSign = cacheStageRewardIndex < mStageRewardCofigs.Count
                     && mMagicStreakActivity.MSAData.Player.Score >= mStageRewardCofigs[cacheStageRewardIndex].TriggerValue;
                if (_stageRewardSign)
                {
                    UIKit.OpenPanel<UIMask>();
                    mMagicStreakActivity.MarkNextStageRewardIdnex();

                    //奖励发放
                    var _packSO = mStageRewardCofigs[cacheStageRewardIndex].RewardPack;
                    mRewardGrantUtility.GrantReward(_packSO);
                }
            }
            else
                mMagicStreakActivity.Fail();

            RankNodeInit();

            var _tempPosIndex = GetStreakTierIndex(mMagicStreakActivity.StreakWinNum);
            Tween _moveX = ImgSelected.DOLocalMoveX(TARGER_POSX[_tempPosIndex], 1f);
            mTweenList.Add(_moveX);

            if (!(bool)mData.ISWin)
                return;

            //插入双倍动画
            if (!mGameGlobalModel.IsTimerFinished(
                mGameGlobalModel.GameGlobalJsonData.TimedBuffData,
                nameof(mGameGlobalModel.GameGlobalJsonData.TimedBuffData.DoubleBuff)))
            {
                mSequence = DOTween.Sequence();
                mDoubleBuffPanel.Show();

                mSequence.Append(mDoubleBuff.transform.DOScale(1.2f, 0.65f).From(0));

                var _shrinkTween = mDoubleBuff.transform.DOScale(0.1f, 0.5f);
                _shrinkTween.OnComplete(() => mDoubleBuffPanel.Hide());

                mSequence.Append(_shrinkTween);

                for (int i = 0; i < mPointsTier.Length; i++)
                {
                    int _index = i; 
                    int _startValue = POINTS_TIER[_index];
                    int _targetValue = _startValue * 2;

                    int _delta = _targetValue - _startValue;
                    float _duration = _delta * 0.01f;

                    var tween = DOTween.To(() => _startValue, x => mPointsTier[_index].text = $"X{x}", _targetValue, _duration);
                    if (_index == 0)
                        mSequence.Append(tween);
                    else
                        mSequence.Join(tween);
                }

                mSequence.AppendCallback(() => ExecuteAfterDoubleBuff(_stageRewardSign));
            }
            else
                ExecuteAfterDoubleBuff(_stageRewardSign);
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnClose.onClick.RemoveAllListeners();

            foreach (var rankNode in mRankNodePool)
            {
                MSARankNodePool.Instance.Recycle(rankNode);
            }

            foreach (var tween in mTweenList)
            {
                tween?.Kill();
            }

            mSequence.Kill();
            mSequence = null;
            mRankNodePool.Clear();
            mRankNodePool = null;
            mTweenList.Clear();
            mTweenList = null;

            mRewardGrantUtility = null;
            mMagicStreakActivity = null;

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }

        private void CacheTempData()
        {
            targetStageScore = -1;
            cacheStageRewardIndex = mMagicStreakActivity.CurStageReward;
            cachePlayerTotalScore = mMagicStreakActivity.MSAData.Player.Score;
            if (cacheStageRewardIndex == 0)
                targetStageScore = mStageRewardCofigs[cacheStageRewardIndex].TriggerValue;
            else if (cacheStageRewardIndex < mStageRewardCofigs.Count)
            {
                lastStageTotalScore = mStageRewardCofigs[cacheStageRewardIndex - 1].TriggerValue;
                targetStageScore = mStageRewardCofigs[cacheStageRewardIndex].TriggerValue - lastStageTotalScore;
            }
        }

        private void UIInit()
        {
            var _curStatus = mData.Status ?? mMagicStreakActivity.ActivityStatus;
            if (_curStatus == SettlementActivityStatus.Active)
                TxtTitle_Blue.text = "Beat levels without fail to get more rewards!";
            else if (_curStatus == SettlementActivityStatus.Finished)
                TxtTitle_Blue.text = "Pass one level to settle ranking rewards.";
            else if (_curStatus == SettlementActivityStatus.WaitStart)
                TxtTitle_Blue.text = "Pass one level to restart the activity.";

            var _posIndex = GetStreakTierIndex(mMagicStreakActivity.StreakWinNum);
            ImgSelected.DOLocalMoveX(TARGER_POSX[_posIndex], 0f);

            if (targetStageScore == -1)
            {
                TxtProgress_Red.text = $"Completed!";
                ImgProgressBar.fillAmount = 1;
            }
            else
            {
                var _playerStageScroe = cachePlayerTotalScore - lastStageTotalScore;
                TxtProgress_Red.text = $"{_playerStageScroe} / {targetStageScore}";
                ImgProgressBar.fillAmount = (float)_playerStageScroe / targetStageScore;
            }

            UpdateRewardUI(targetStageScore == -1);
        }

        //面板弹出逻辑()
        private void ExecuteAfterDoubleBuff(bool stageRewardSign)
        {
            //当前玩家总分
            var _curPlayerTotalScore = mMagicStreakActivity.MSAData.Player.Score;

            //阶段奖励未领取完
            if (targetStageScore != -1)
            {
                var _lastPlayerStageScore = cachePlayerTotalScore - lastStageTotalScore;
                var _curPlayerStageScore = _curPlayerTotalScore - lastStageTotalScore;

                Tween _txtUp2 = DOTween.To(() => _lastPlayerStageScore,
                  x => TxtProgress_Red.text = $"{x} / {targetStageScore}",
                  _curPlayerStageScore, 1.5f);
                mTweenList.Add(_txtUp2);

                Tween _progressUp = ImgProgressBar.DOFillAmount((float)_curPlayerStageScore / targetStageScore, 1.5f)
                    .OnComplete(() =>
                    {
                        if (stageRewardSign)
                        {
                            UIKit.ClosePanel<UIMask>();
                            var _packSO = mStageRewardCofigs[cacheStageRewardIndex].RewardPack;
                            RewardUIManager.Instance.PlayRewardAnim(_packSO.Coins, true, () =>
                            {
                                //重新标记缓存数据
                                CacheTempData();
                                if (targetStageScore == -1)
                                {
                                    TxtProgress_Red.text = $"Completed!";
                                    ImgProgressBar.fillAmount = 1;
                                    return;
                                }
                                UpdateRewardUI(targetStageScore == -1);

                                _curPlayerStageScore = _curPlayerTotalScore - lastStageTotalScore;
                                TxtProgress_Red.text = $"{_curPlayerStageScore} / {targetStageScore}";
                                ImgProgressBar.fillAmount = (float)_curPlayerStageScore / targetStageScore;
                            }, _packSO);
                        }
                    });
                mTweenList.Add(_progressUp);
            }
        }

        private void UpdateRewardUI(bool rewardOver)
        {
            if (rewardOver)
            {
                ImgRewardUI.Hide();
                return;
            }

            var _packSO = mStageRewardCofigs[cacheStageRewardIndex].RewardPack;

            //阶段奖励礼包内容只有一个(一个特殊道具/普通道具/金币)
            if (_packSO.ItemReward.Count != 0)
            {
                ImgRewardUI.sprite = RewardUIManager.Instance.GetRewardSprite(_packSO.ItemReward[0].NormalRewardsType);
                TxtRewardNum.text = $"X{_packSO.ItemReward[0].Quantity}";
            }
            else if (_packSO.SpecialRewards.Count != 0)
            {
                ImgRewardUI.sprite = RewardUIManager.Instance.GetRewardSprite(_packSO.SpecialRewards[0].SpecialRewardType);
                TxtRewardNum.text = $"X{_packSO.SpecialRewards[0].Duration}min";
            }
            else
            {
                ImgRewardUI.sprite = mCoinSprite;
                TxtRewardNum.text = $"X{_packSO.Coins}";
            }
        }

        private void RankNodeInit()
        {
            //初始化排名节点信息，定格在玩家排名视野
            var _count = mMagicStreakActivity.MSAData.MSARobots.Count;
            var _playerRank = mMagicStreakActivity.PlayerRank;

            //玩家前的节点
            for (int i = 1; i < _playerRank; i++)
            {
                var robot = mMagicStreakActivity.MSAData.MSARobots[i - 1];
                var node = MSARankNodePool.Instance.Allocate();
                node.transform.SetParent(mRankPar, false);
                node.GetComponent<MSANodeCtrl>().InitRobot(i, robot, GetRankPackSO(i));
                mRankNodePool.Add(node);
            }

            //玩家节点
            var playerNode = MSARankNodePool.Instance.Allocate();
            playerNode.transform.SetParent(mRankPar, false);
            playerNode.GetComponent<MSANodeCtrl>().InitPlayer(_playerRank, mMagicStreakActivity.MSAData.Player , GetRankPackSO(_playerRank));
            mRankNodePool.Add(playerNode);

            //玩家后的节点
            for (int i = _playerRank; i <= _count; i++)
            {
                var robot = mMagicStreakActivity.MSAData.MSARobots[i - 1];
                var node = MSARankNodePool.Instance.Allocate();
                node.transform.SetParent(mRankPar, false);
                node.GetComponent<MSANodeCtrl>().InitRobot(i + 1, robot, GetRankPackSO(i + 1));
                mRankNodePool.Add(node);
            }

            FocusPlayerNode();
        }

        /// <summary>
        /// 聚焦玩家排名
        /// </summary>
        private void FocusPlayerNode()
        {
            if (mRankNodePool == null || mRankNodePool.Count == 0) return;
            int playerIndex = mMagicStreakActivity.PlayerRank - 1;
            if (playerIndex < 0 || playerIndex >= mRankNodePool.Count) return;

            LayoutRebuilder.ForceRebuildLayoutImmediate(mRankPar);
            RectTransform viewport = RankScrollRect.viewport != null ? RankScrollRect.viewport : (RankScrollRect.transform as RectTransform);
            RectTransform playerRect = mRankNodePool[playerIndex].GetComponent<RectTransform>();

            // 把玩家节点的世界坐标转换为 Content 的局部坐标
            Vector3 playerLocalPosInContent = mRankPar.InverseTransformPoint(playerRect.position);

            float contentHeight = mRankPar.rect.height;
            float viewportHeight = viewport.rect.height;
            if (contentHeight <= viewportHeight)
                return;

            //计算content顶点Y坐标 、玩家节点与content顶部距离
            float contentTopLocalY = mRankPar.rect.height * (1f - mRankPar.pivot.y);
            float playerYFromTop = contentTopLocalY - playerLocalPosInContent.y;

            //计算玩家在 viewport 中间的滚动距离(减去viewport高度一半让玩家处于中心)。
            float desiredTopOffset = playerYFromTop - (viewportHeight * 0.5f);

            float maxTopOffset = contentHeight - viewportHeight;
            float clampedTopOffset = Mathf.Clamp(desiredTopOffset, 0f, maxTopOffset);
            //1表示顶端,减去比例得到映射值
            float normalized = 1f - (clampedTopOffset / maxTopOffset);

            RankScrollRect.verticalNormalizedPosition = normalized;
        }

        //获取滑动块坐标索引(按连胜划分)
        private int GetStreakTierIndex(int winCount)
        {
            if (winCount <= 1) return 0;
            if (winCount == 2) return 1;
            if (winCount == 3) return 2;
            if (winCount == 4) return 3;
            return 4;
        }

        private GiftPackSO GetRankPackSO(int ranking)
        {
            foreach (var item in mRankRewardCofigs)
            {
                if (ranking <= item.TriggerValue)
                    return item.RewardPack;
            }

            return null;
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
