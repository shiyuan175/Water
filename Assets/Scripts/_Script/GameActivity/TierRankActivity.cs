using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (!mCountDownTimerManager.IsTimerFinished(TRA_NEXT_DAY_SIGN))
                return SettlementActivityStatus.Locked;

            if (!mCountDownTimerManager.IsTimerFinished(ActivitySign)
                && !GameDefine.GameUtils.DoesCountDownKeyExist(GameDefine.GameConst.TRA_HALF_ONE_HOUR_RANK))
                return SettlementActivityStatus.Inactive;

            if (!mCountDownTimerManager.IsTimerFinished(ActivitySign)
                && !mCountDownTimerManager.IsTimerFinished(GameDefine.GameConst.TRA_HALF_ONE_HOUR_RANK))
                return SettlementActivityStatus.Active;

            if (!mCountDownTimerManager.IsTimerFinished(ActivitySign)
                && mCountDownTimerManager.IsTimerFinished(GameDefine.GameConst.TRA_HALF_ONE_HOUR_RANK))
                return SettlementActivityStatus.Finished;

            if (mCountDownTimerManager.IsTimerFinished(ActivitySign))
                return SettlementActivityStatus.WaitStart;

            return SettlementActivityStatus.None;
        }
    }
    public TRActivityData TRAData => mTRAModel.TRAData;

    private const string TRA_NEXT_DAY_SIGN = "TRA_NextDaySign";
    private CountDownTimerManager mCountDownTimerManager;
    private TierRankActivityModel mTRAModel;

    private float mRobotWinTimer = 0f;
    private const float ROBOT_WIN_INTERVAL = 3 * 60f;

    public TierRankActivity()
    {
        mCountDownTimerManager = CountDownTimerManager.Instance;
        mTRAModel = this.GetModel<TierRankActivityModel>();

        mCountDownTimerManager.StartEasternMidnightTimer(TRA_NEXT_DAY_SIGN);

        if (mCountDownTimerManager.IsTimerFinished(TRA_NEXT_DAY_SIGN))
        {
            if (!GameDefine.GameUtils.DoesCountDownKeyExist(ActivitySign))
                StartActivity();
            else
                mTRAModel.LoadTRAData();
        }
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
        if (ActivityStatus is SettlementActivityStatus.Active)
        {
            mRobotWinTimer += 1f;
            if (mRobotWinTimer >= ROBOT_WIN_INTERVAL)
            {
                mRobotWinTimer = 0f; 
                AddRobotStreakWin();
            }
        }

        //活动刷新逻辑
        switch (ActivityStatus)
        {
            case SettlementActivityStatus.WaitStart:
                RestartActivity();
                mCountDownTimerManager.DeleteTimer(GameDefine.GameConst.TRA_HALF_ONE_HOUR_RANK);
                break;
        }

        base.Tick();
    }
    
    public bool RestartOneHourRankActivity()
    {
        CountDownTimerManager.Instance.ResetCountdownTimer(GameDefine.GameConst.TRA_HALF_ONE_HOUR_RANK, 0.5f);

        //在游戏内Next Day倒计时结束，可能存在数据还未加载的情况
        //如临近美东0时到达31关，然后到达美东0时。此时数据还未加载,开启活动会空引用
        if (TRAData is null)
        {
            RestartActivityInit();
            return true;
        }

        var _isRewardSettled = TRAData.Player.IsRewardSettled;
        RestartActivityInit();

        return _isRewardSettled;
    }

    public string GetHalfOneHourTierRankTime()
    {
        return CountDownTimerManager.Instance.GetRemainingTimeText(GameDefine.GameConst.TRA_HALF_ONE_HOUR_RANK);
    }

    /// <summary>
    /// 每三分钟触发一次，为同一机器人增加连胜次数
    /// </summary>
    private void AddRobotStreakWin()
    {
        if (TRAData?.TRARobots == null)
            return;

        var _robot = TRAData.TRARobots.FirstOrDefault(r => r.ID == 3);
        if (_robot != null)
        {
            _robot.StreamWinNum += 1;
            mTRAModel.SaveJson();
        }
    }
}
