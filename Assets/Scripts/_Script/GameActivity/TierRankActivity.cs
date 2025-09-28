using System.Collections;
using System.Collections.Generic;
using JsonFileData;
using QFramework;
using QFramework.Example;
using UnityEngine;

public class TierRankActivity : BaseRewardSettlementActivity
{
    public override bool IsRewardSettled => TRAData.Player.IsRewardSettled;
    public override string ActivitySign => GameDefine.GameConst.TIER_RANK_ACTIVITY_SIGN;
    public override SettlementActivityStatus ActivityStatus
    {
        get
        {
            if (!mCountDownTimerManager.IsTimerFinished(ActivitySign)
                && !GameDefine.GameUtils.DoesCountDownKeyExist(GameDefine.GameConst.TRA_ONE_HOUR_RANK))
            {
                return SettlementActivityStatus.Inactive;
            }

            if (!mCountDownTimerManager.IsTimerFinished(ActivitySign)
                && !mCountDownTimerManager.IsTimerFinished(GameDefine.GameConst.TRA_ONE_HOUR_RANK))
            {
                return SettlementActivityStatus.Active;
            }

            if (!mCountDownTimerManager.IsTimerFinished(ActivitySign)
                && mCountDownTimerManager.IsTimerFinished(GameDefine.GameConst.TRA_ONE_HOUR_RANK))
            {
                return SettlementActivityStatus.Finished;
            }

            if (mCountDownTimerManager.IsTimerFinished(ActivitySign))
            {
                return SettlementActivityStatus.WaitStart;
            }

            return SettlementActivityStatus.None;
        }
    }

    public int StreakWinNum => mTRAModel.StreakWinNum;
    public int PlayerTierRankIndex => GetTierRankIndex(StreakWinNum);
    public int RankSettlementCoins => (PlayerTierRankIndex + 1) * COINS_PER_RANK;
    public TRActivityData TRAData => mTRAModel.TRAData;

    private const int COINS_PER_RANK = 100;

    private CountDownTimerManager mCountDownTimerManager;
    private TierRankActivityModel mTRAModel;

    public TierRankActivity()
    {
        mCountDownTimerManager = CountDownTimerManager.Instance;
        mTRAModel = this.GetModel<TierRankActivityModel>();

        if (!GameDefine.GameUtils.DoesCountDownKeyExist(ActivitySign))
            StartActivity();
        else
            mTRAModel.LoadTRAData();
    }

    public override void StreakWin()
    {
        mTRAModel.StreakWin();
    }

    public override void Fail()
    {
        mTRAModel.Fail();
    }

    public override void MarkRewardAsSettled()
    {
        mTRAModel.MarkRewardAsSettled();
    }

    public override void RestartActivityInit()
    {
        mTRAModel.ReloadTRAData();
        mTRAModel.LoadTRAData();
    }

    public override void RestartActivity()
    {
        mCountDownTimerManager.ResetEasternMidnightTimer(ActivitySign);
    }

    public override void Tick()
    {
        //»î¶¯Ë¢ÐÂÂß¼­
        switch (ActivityStatus)
        {
            case SettlementActivityStatus.WaitStart:
                RestartActivity();
                mCountDownTimerManager.DeleteTimer(GameDefine.GameConst.TRA_ONE_HOUR_RANK);
                break;
        }

        base.Tick();
    }
    
    public int GetTierRankIndex(int streakWin)
    {
        return mTRAModel.GetTierRankIndex(streakWin);
    }

    public bool CompareWithHistoryBestRank()
    {
        return mTRAModel.CompareWithHistoryBestRank();
    }

    public (bool isRewardSettled, bool isFirstRank) RestartOneHourRankActivity()
    {
        var _isRewardSettled = TRAData.Player.IsRewardSettled;
        var _isFirstRank = mTRAModel.FirstHourTierRank;

        if (_isFirstRank)
            mTRAModel.MarkFirstHourTierRank();

        CountDownTimerManager.Instance.ResetCountdownTimer(GameDefine.GameConst.TRA_ONE_HOUR_RANK, 1);
        RestartActivityInit();

        return (_isRewardSettled, _isFirstRank);
    }

    public string GetOneHourTierRankTime()
    {
        return CountDownTimerManager.Instance.GetRemainingTimeText(GameDefine.GameConst.TRA_ONE_HOUR_RANK);
    }
}
