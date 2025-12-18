using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class HTANodeCtrl : MonoBehaviour
{
    [SerializeField] private RectTransform mPlayer;
    [SerializeField] private Image mProgressBar;

    [Header("底部节点靠前，当前设计最多3个节点")]
    [SerializeField] private Text[] mStageTexts;
    [Header("目标阶段宝箱(开启奖励后隐藏)")]
    [SerializeField] private Image mBoxImgs;
    [Header("进度条段数(用于计算进度权重)")]
    [SerializeField] private ProgressSegmentType mProgressSegmentType;

    private enum ProgressSegmentType
    {
        Single = 1,
        Double = 2
    }
    private List<Tween> mTweens;
    private HighTowerActivity mHTActivity;

    //区间高度
    private float mStagePosYGap;
    //下一阶段奖励索引
    private int mCache_NextRewardStageIndex;

    public void Init(HighTowerActivity activity)
    {
        //数据缓存,避免数据修改后表现错误
        mHTActivity = activity;
        mCache_NextRewardStageIndex = mHTActivity.NextRewardStageIndex;

        mStageTexts[0].text = mHTActivity.RewardStages[mCache_NextRewardStageIndex - 1].ToString();
        mStageTexts[1].text = mHTActivity.RewardStages[mCache_NextRewardStageIndex].ToString();
        //Mid 有三个节点(外部根据当前连胜所处阶段进行实例)
        if (mStageTexts.Length == 3)
            mStageTexts[2].text = mHTActivity.RewardStages[mCache_NextRewardStageIndex + 1].ToString();

        //区间高度
        mStagePosYGap = mStageTexts.Last().transform.position.y - mStageTexts.First().transform.position.y;

        //人物位置、进度条初始
        var _tuple = GetProgressRatioAndYGap();
        mPlayer.position = new Vector3(mPlayer.position.x, _tuple.posYGap, mPlayer.position.z);
        mProgressBar.fillAmount = _tuple.ratio;
    }

    public void PlayTween(bool playerWin, GiftPackSO rewardPackSO , Action grantReward)
    {
        //增加连胜数据
        if (playerWin)
            mHTActivity.StreakWin();
        else
            mHTActivity.Fail();

        bool _sendReward = false;
        if (mHTActivity.HTAStreakWinNum >= mHTActivity.RewardStages[mCache_NextRewardStageIndex])
        {
            //Debug.Log("到达下一目标-------发放奖励");
            _sendReward = true;
            grantReward?.Invoke();
        }

        if (playerWin)
        {
            var _tuple = GetProgressRatioAndYGap(true);

            //动画效果
            Tween _playerUp = mPlayer.DOMoveY(_tuple.posYGap, 1.3f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    if (_sendReward)
                    {
                        //Debug.Log("到达下一目标-------播放奖励动画");
                        RewardUIManager.Instance.PlayRewardAnim(rewardPackSO.Coins, true, () => mBoxImgs.Hide(), rewardPackSO);
                    } 
                });
            Tween _progressUp = DOTween.To(() => mProgressBar.fillAmount,
                value => mProgressBar.fillAmount = value,
               _tuple.ratio, 1f).SetEase(Ease.OutQuad);
            mTweens.Add(_playerUp);
            mTweens.Add(_progressUp);
        }
        else
        {
            Tween _playerFall = mPlayer.DOAnchorPosY(mStageTexts.First().transform.position.y, 1.3f).SetEase(Ease.OutQuad);
            Tween _progressClear = DOTween.To(() => mProgressBar.fillAmount,
                value => mProgressBar.fillAmount = value,
                0f, 1f).SetEase(Ease.OutQuad);
              
            mTweens.Add(_playerFall);
            mTweens.Add(_progressClear);
        }
    }

    /// <summary>
    /// 获取当前进度比例、人物高度
    /// </summary>
    /// <returns></returns>
    private (float ratio ,float posYGap) GetProgressRatioAndYGap(bool sss = false)
    {
        #region 公式
        //(当前连胜 - 当前阶段连胜基数) / 当前区间所需连胜(下阶段连胜基数 - 当前阶段连胜基数) * 权重
        //如跨阶段,则需将索引后移
        //权重 + (当前连胜 - 下阶段连胜基数) / 下阶段区间所需连胜(下下阶段连胜基数 - 下阶段连胜基数) * 权重
        #endregion

        //顶部特判(避免越界)
        if (mProgressSegmentType is ProgressSegmentType.Single)
        {
            var __ratio = (mHTActivity.HTAStreakWinNum - mHTActivity.RewardStages[mCache_NextRewardStageIndex - 1])
             / (float)(mHTActivity.RewardStages[mCache_NextRewardStageIndex] - mHTActivity.RewardStages[mCache_NextRewardStageIndex - 1]);
            //当前高度(线性插值计算)
            var __posYGap = mStageTexts.First().transform.position.y + mStagePosYGap * __ratio;

            return (__ratio, __posYGap);
        }

        //平均权重
        float _stageWeight = 1f / (int)mProgressSegmentType;
        float _ratio;

        if (mHTActivity.HTAStreakWinNum >= mHTActivity.RewardStages[mCache_NextRewardStageIndex])
        {
            _ratio = _stageWeight + 
                (mHTActivity.HTAStreakWinNum - mHTActivity.RewardStages[mCache_NextRewardStageIndex]) 
                / (float)(mHTActivity.RewardStages[mCache_NextRewardStageIndex + 1] - mHTActivity.RewardStages[mCache_NextRewardStageIndex])
                * _stageWeight;
        }
        else
        {
            //避免被除数为 0 
            if (mHTActivity.HTAStreakWinNum == 0)
                _ratio = 0;
            else
            {
                _ratio = (mHTActivity.HTAStreakWinNum - mHTActivity.RewardStages[mCache_NextRewardStageIndex - 1])
               / (float)(mHTActivity.RewardStages[mCache_NextRewardStageIndex] - mHTActivity.RewardStages[mCache_NextRewardStageIndex - 1])
               * _stageWeight;
            }
        }

        //当前高度(线性插值计算)
        var _posYGap = mStageTexts.First().transform.position.y + mStagePosYGap * _ratio;

        return (_ratio, _posYGap);
    }
                   
    private void Awake()
    {
        mTweens = new List<Tween>();
    }

    private void OnDestroy()
    {
        //Debug.Log("对象被销毁了");
        foreach (var tween in mTweens)
        {
            tween.Kill();
        }
        mTweens.Clear();
    }
}
