using System;
using System.Collections;
using System.Collections.Generic;
using GameDefine;
using QFramework;
using UnityEngine;

public class VolcanicActivity : BaseGameActivity
{
    public override string ActivityID => GetType().Name;
    public override string ActivitySign => "VolcanicActivity";
    public override string ActivityCooldownSign => "VolcanicActivityCoolDown";
    public override int ActivityBeginLevel => GameConst.VA_BEGIN_LEVEL;

    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
                return GameActivityStatus.Locked;

            if (!VAActivateState)
                return GameActivityStatus.Inactive;
            //可以加上ActivitySign是否过期的条件
            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign)
                && CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign)
                && !IsExceededDailyRefreshLimit)
                return GameActivityStatus.Active;

            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign)
                && !CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign)
                && !IsExceededDailyRefreshLimit)
                return GameActivityStatus.CoolingDown;

            return GameActivityStatus.WaitStart;
        }
    }

    public int RewardCoins => mVolcanicActivityModel.GetVARewardCoins;
    public int VAStreakWinNum => mVolcanicActivityModel.VAStreakWinNum;
    public int VACurrentPlayerNum => mVolcanicActivityModel.VACurrentPlayerNum;
    public int VADailyUsedRefreshCount => mVolcanicActivityModel.VADailyUsedRefreshCount;
   //闯关胜利结束 
    public bool EndWin => VAStreakWinNum >= VA_MAX_STREAK_WIN_NUM;
    public bool VAActivateState => mVolcanicActivityModel.VAActivateState;

    private bool IsExceededDailyRefreshLimit => VADailyUsedRefreshCount > VA_MAX_REFRESH_PER_DAY;

    private const int VA_MAX_REFRESH_PER_DAY = 3;
    private const int VA_MAX_STREAK_WIN_NUM = 7;
    private VolcanicActivityModel mVolcanicActivityModel;
    private GameActivityStatus mCurrentStatus;

    public VolcanicActivity()
    {
        mVolcanicActivityModel = this.GetModel<VolcanicActivityModel>();
        mCurrentStatus = ActivityStatus;
    }

    public override void StartActivity()
    {
        mVolcanicActivityModel.MarkActivateState();
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign);
    }

    public override void StreakWin()
    {
        mVolcanicActivityModel.AddVAStreakWin();
        if (EndWin)
            CoolDownActivity();
    }

    public override void Fail()
    {
        mVolcanicActivityModel.VA_Fail();
        CoolDownActivity();
    }

    /// <summary>
    /// 每日重置
    /// </summary>
    public void RefreshActivity()
    {
        mVolcanicActivityModel.RefreshVolcanicActivity();
        CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign);
    }

    /// <summary>
    /// 活动重置(每日三次)
    /// </summary>
    public override void RestartActivity()
    {
        if (IsExceededDailyRefreshLimit)
            return;

        mVolcanicActivityModel.ReloadVolcanicActivity();
    }

    public override void CoolDownActivity()
    {
        mVolcanicActivityModel.AddDailyUsedRefreshCount();
        if (IsExceededDailyRefreshLimit)
            return;

        CountDownTimerManager.Instance.ResetCountdownTimer(ActivityCooldownSign, 0.5f);
    }

    public override void Tick()
    {
        switch (mCurrentStatus)
        {
            case GameActivityStatus.Inactive:
                if (VAActivateState)
                    mCurrentStatus = GameActivityStatus.Active;
                break;

            case GameActivityStatus.Active:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign) || IsExceededDailyRefreshLimit)
                    mCurrentStatus = GameActivityStatus.WaitStart;
                else if (!CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
                    mCurrentStatus = GameActivityStatus.CoolingDown;
                break;

            case GameActivityStatus.CoolingDown:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign) || IsExceededDailyRefreshLimit)
                    mCurrentStatus = GameActivityStatus.WaitStart;
                else if (CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
                {
                    RestartActivity();
                    mCurrentStatus = GameActivityStatus.Active;
                }
                break;

            case GameActivityStatus.WaitStart:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
                {
                    RefreshActivity();
                    mCurrentStatus = GameActivityStatus.Inactive;
                }
                break;
        }

        base.Tick();
    }

    /// <summary>
    /// 获取冷却倒计时
    /// </summary>
    /// <returns></returns>
    public string GetCooldownReamingTime()
    {
        return CountDownTimerManager.Instance.GetRemainingTimeText(ActivityCooldownSign);
    }
}