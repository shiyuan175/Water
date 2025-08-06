using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class RocketActivity : BaseGameActivity
{
    public override string ActivitySign => "RocketActivity";
    //该活动用于每日刷新次数的冷却
    public override string ActivityCooldownSign => "RocketActivityCoolDown";

    public override float ActivityDurationMinutes => 1440f;
    public override float ActivityCooldownMinutes => 1440f;

    public override string ActivityID => GetType().Name;

    public override GameActivityStatus ActivityStatus
    { 
        get
        {
            if (mSaveUtility.GetCurrentLevel() < GameDefine.GameConst.VA_BEGIN_LEVEL)
            {
                return GameActivityStatus.Locked;
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

    public bool PlayWin => mRocketActivityModel.PlayerWin;
    public bool RobotWin => mRocketActivityModel.RobotWin;
    public bool IsExceededDailyRefreshLimit => mRocketActivityModel.IsExceededDailyRefreshLimit;

    private SaveDataUtility mSaveUtility;
    private RocketActivityModel mRocketActivityModel;

    public RocketActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        mRocketActivityModel = this.GetModel<RocketActivityModel>();
        
        //首次解锁活动触发
        if (!PlayerPrefs.HasKey(CountDownTimerManager.COUNTDOWN_TIMER_SIGN + ActivityCooldownSign))
        {
            StartActivity();
        }
    }

    public override void StartActivity()
    {
        base.StartActivity();
        CountDownTimerManager.Instance.StartTimer(ActivityCooldownSign, ActivityCooldownMinutes);
    }

    public override void StreakWin()
    {
        mRocketActivityModel.RAStreakWin();
        //此活动依赖刷新次数驱动，自动重置活动会导致使用数据被重置(需要分离)
        //if (PlayWin || RobotWin)
        //    RestartActivity();
    }

    //活动面板关闭时调用
    public void TryRestarActivity()
    {
        if (PlayWin || RobotWin)
            RestartActivity();
    }

    public override void Fail()
    {
        RestartActivity();
    }

    public override void RestartActivity()
    {
        //中断重置(可以考虑吧该活动注册表删除)
        if (IsExceededDailyRefreshLimit)
            return;

        else mRocketActivityModel.ReloadRocketActivity();
        CountDownTimerManager.Instance.ResetTimer(ActivitySign, ActivityDurationMinutes);
    }

    public override void RestartActivityInit()
    {
      
    }

    public override void CoolDownActivityInit()
    {

    }

    private GameActivityStatus mLastActivityStatus;
    public override void Tick()
    {
        //刷新次数重置判定
        if (CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
        {
            mRocketActivityModel.ResetDailyRefreshCount();
            CountDownTimerManager.Instance.ResetTimer(ActivityCooldownSign, ActivityCooldownMinutes);
        }

        //活动过期重启判定
        if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign) && 
           ActivityStatus == GameActivityStatus.Active)
        {
            RestartActivity();
        }

        //状态变更事件
        if (ActivityStatus != mLastActivityStatus)
        {
            this.SendEvent(new OnActivityStatusChanged()
            {
                Sender = this,
                Status = ActivityStatus
            });
            mLastActivityStatus = ActivityStatus;
        }
    }
}
