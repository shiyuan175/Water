using System;
using System.Collections;
using System.Collections.Generic;
using GameDefine;
using QFramework;
using UnityEngine;

public class RocketActivity : BaseGameActivity
{
    public override string ActivitySign => GameConst.ROCKET_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;
    public override int ActivityBeginLevel => GameConst.RA_BEGIN_LEVEL;
    public override GameActivityStatus ActivityStatus
    { 
        get
        { 
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
            {
                return GameActivityStatus.Locked;
            }
            else if (!GameUtils.DoesCountDownKeyExist(ActivitySign))
            {
                return GameActivityStatus.Inactive;
            }
            else if (!IsExceededDailyRefreshLimit)
            {
                return GameActivityStatus.Active;
            }
            else return GameActivityStatus.CoolingDown;
        } 
    }

    public int RAMaxStreakWinNum => mRocketActivityModel.RAMAxStreakWinNum;
    public int PlayerStreakWin => mRocketActivityModel.PlayerStreakWin;
    public int Robot1StreakWin => mRocketActivityModel.Robot1StreakWin;
    public int Robot2StreakWin => mRocketActivityModel.Robot2StreakWin;
    public int DailyUsedRefreshCount => mRocketActivityModel.DailyUsedRefreshCount;

    public bool PlayWin => mRocketActivityModel.PlayerWin;
    public bool RobotWin => mRocketActivityModel.RobotWin;
    public bool IsExceededDailyRefreshLimit => DailyUsedRefreshCount > RA_MAX_REFRESH_PER_DAY;

    private const int RA_MAX_REFRESH_PER_DAY = 3;
    private RocketActivityModel mRocketActivityModel;

    public RocketActivity()
    {
        mRocketActivityModel = this.GetModel<RocketActivityModel>();
    }
    
    public override void StartActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign);
    }

    public override void StreakWin()
    {
        mRocketActivityModel.RAStreakWin();
    }

    //活动面板关闭时调用
    public void TryRestarActivity()
    {
        if (PlayWin || RobotWin)
            RefreshActivity();
    }

    public override void Fail()
    {
        RefreshActivity();
    }

    public void RefreshActivity()
    {
        //当已刷新次数刚好等于最大次数时,还是会触发一次数据刷新,然后超出每日刷新次数
        //再由Tick将状态切到CoolingDown,然后触发活动隐藏
        if (IsExceededDailyRefreshLimit)
            return;

        else mRocketActivityModel.RefreshRocketActivityData();
    }
    
    public override void RestartActivity()
    {
        mRocketActivityModel.ResetDailyRefreshCount();
        CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign);
    }
    
    public override void Tick()
    {
        if (GameUtils.DoesCountDownKeyExist(ActivitySign) &&
            CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
            RestartActivity();

        base.Tick();
    }
}
