using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class HTANodeCtrl : MonoBehaviour
{
    [SerializeField] private float mStartPosY;
    [SerializeField] private float mEndPosY;

    [SerializeField] private Image mPlayer;
    [SerializeField] private Image mProgressBar;
    [Header("底部节点靠前，当前设计最多3个节点")]
    [SerializeField] private Text[] mStageTexts;

    private List<Tween> mTweens;
    private HighTowerActivity mHTActivity;

    private float mCache_RemainingToNextReward;
    private float mCache_CurrentRewardStageGap;
    private int mCache_NextRewardStageIndex;

    public void Init(HighTowerActivity activity)
    {
        //数据缓存,避免数据修改后表现错误
        mHTActivity = activity;
        mCache_RemainingToNextReward = mHTActivity.WinRemainingToNextReward;
        mCache_CurrentRewardStageGap = mHTActivity.CurrentRewardStageGap;
        mCache_NextRewardStageIndex = mHTActivity.NextRewardStageIndex;

        mStageTexts[0].text = mHTActivity.RewardStages[mCache_NextRewardStageIndex - 1].ToString();
        mStageTexts[1].text = mHTActivity.RewardStages[mCache_NextRewardStageIndex].ToString();
        //如果不是顶楼节点,则有三条文本(外部实例保证索引不会越界)
        if (mStageTexts.Length == 3)
            mStageTexts[2].text = mHTActivity.RewardStages[mCache_NextRewardStageIndex + 1].ToString();

        //当前阶段进度比例
        var _offset = 
            (float)(mCache_CurrentRewardStageGap - mCache_RemainingToNextReward) / mCache_CurrentRewardStageGap;

        RectTransform _rt = mPlayer.GetComponent<RectTransform>();
        Vector2 _anchored = _rt.anchoredPosition;
        _anchored.y = mStartPosY + (mEndPosY - mStartPosY) * _offset;
        _rt.anchoredPosition = _anchored;

        mProgressBar.fillAmount = _offset;
    }

    public void PlayTween(bool playerWin, RewardPackSO rewardPackSO , Action grantReward)
    {
        //增加连胜数据
        if (playerWin)
            mHTActivity.StreakWin();
        else
            mHTActivity.Fail();

        if (mHTActivity.HTAStreakWinNum == mHTActivity.RewardStages[mCache_NextRewardStageIndex])
        {
            //Debug.Log("到达下一目标-------发放奖励");
            grantReward?.Invoke();
        }

        if (playerWin)
        {
            --mCache_RemainingToNextReward;
            var _offset =
                 (float)(mCache_CurrentRewardStageGap - mCache_RemainingToNextReward) / mCache_CurrentRewardStageGap;
            float _targetY = mStartPosY + (mEndPosY - mStartPosY) * _offset;
            RectTransform _rt = mPlayer.GetComponent<RectTransform>();
            Tween _playerUp = _rt.DOAnchorPosY(_targetY, 1.3f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    if (mHTActivity.HTAStreakWinNum == mHTActivity.RewardStages[mCache_NextRewardStageIndex])
                    {
                        //Debug.Log("到达下一目标-------播放奖励动画");
                        StartCoroutine(RewardUIManager.Instance.PlayRewardAnim(
                            rewardPackSO, rewardPackSO.Coins != 0));
                    } 
                });
            Tween _progressUp = DOTween.To(() => mProgressBar.fillAmount,
                value => mProgressBar.fillAmount = value,
               _offset, 1f).SetEase(Ease.OutQuad);
            mTweens.Add(_playerUp);
            mTweens.Add(_progressUp);
        }
        else
        {
            RectTransform _rt = mPlayer.GetComponent<RectTransform>();
            Tween _playerFall = _rt.DOAnchorPosY(mStartPosY, 1.3f).SetEase(Ease.OutQuad);
            Tween _progressClear = DOTween.To(() => mProgressBar.fillAmount,
                value => mProgressBar.fillAmount = value,
                0f, 1f).SetEase(Ease.OutQuad);
                
            mTweens.Add(_playerFall);
            mTweens.Add(_progressClear);
        }
    }

    private void Awake()
    {
        mTweens = new List<Tween>();
    }

    private void OnDestroy()
    {
        foreach (var tween in mTweens)
        {
            tween.Kill();
        }
        mTweens.Clear();
    }
}
