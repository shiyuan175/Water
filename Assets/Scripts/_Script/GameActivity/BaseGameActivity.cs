using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public abstract class BaseGameActivity : IGameActivity, ICanGetModel, ICanGetUtility, ICanSendEvent
{
    public abstract string ActivitySign { get; }
    public abstract string ActivityCooldownSign { get; }

    /// <summary>
    /// 活动时长(分钟)
    /// </summary>
    public abstract float ActivityDurationMinutes { get; }

    /// <summary>
    /// 活动冷却时长(分钟)
    /// </summary>
    public abstract float ActivityCooldownMinutes { get; }

    public abstract string ActivityID { get; }

    public abstract GameActivityStatus ActivityStatus { get; }

    // 基类用于维护活动状态
    private GameActivityStatus mLastActivityStatus;
    private GameActivityStatus mCurrentStatus = GameActivityStatus.WaitStart;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    /// <summary>
    /// 连胜业务逻辑
    /// </summary>
    public abstract void StreakWin();
    /// <summary>
    /// 失败业务逻辑
    /// </summary>
    public abstract void Fail();

    /// <summary>
    /// 重置计时器初始化(数据重置)
    /// </summary>
    public abstract void RestartActivityInit();
    /// <summary>
    /// 冷却活动初始化(数据重置)
    /// </summary>
    public abstract void CoolDownActivityInit();

    public virtual void StartActivity()
    {
        CountDownTimerManager.Instance.StartTimer(ActivitySign, ActivityDurationMinutes);
    }

    public virtual void RestartActivity()
    {
        //Debug.Log("重启活动");
        RestartActivityInit();
        CountDownTimerManager.Instance.ResetTimer(ActivitySign, ActivityDurationMinutes);
        CountDownTimerManager.Instance.DeleteTimer(ActivityCooldownSign);
    }

    public virtual void CoolDownActivity()
    {
        //Debug.Log("开启冷却");
        CoolDownActivityInit();
        CountDownTimerManager.Instance.DeleteTimer(ActivitySign);
        CountDownTimerManager.Instance.StartTimer(ActivityCooldownSign, ActivityCooldownMinutes);
        mCurrentStatus = GameActivityStatus.CoolingDown;
    }

    /// <summary>
    /// 获取活动剩余时间
    /// </summary>
    /// <returns></returns>
    public string GetActivityReamingTime()
    {
        return CountDownTimerManager.Instance.GetRemainingTimeText(ActivitySign);
    }

    //活动注册后会自动调用
    public virtual void Tick()
    {
        switch (mCurrentStatus)
        {
            case GameActivityStatus.Active:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
                {
                    CoolDownActivity();
                    mCurrentStatus = GameActivityStatus.CoolingDown;
                }
                break;

            case GameActivityStatus.CoolingDown:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
                {
                    RestartActivity();
                    mCurrentStatus = GameActivityStatus.Active;
                }
                break;

            case GameActivityStatus.WaitStart:
                if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
                {
                    mCurrentStatus = GameActivityStatus.Active;
                }
                else
                {
                    mCurrentStatus = GameActivityStatus.CoolingDown;
                }
                break;
        }

        //启用时默认发送一次状态事件(未开启除外)
        //由CoolDown 进入 WaitStart 状态时的事件可以不发送(无实质作用)
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
