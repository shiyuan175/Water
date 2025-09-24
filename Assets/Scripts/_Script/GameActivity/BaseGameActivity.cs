using System.Collections;
using System.Collections.Generic;
using GameDefine;
using QFramework;
using UnityEngine;

//奖励触发式活动基类
public abstract class BaseGameActivity : IGameActivity, ICanGetModel, ICanGetUtility, ICanSendEvent
{
    public abstract string ActivitySign { get; }
    public abstract string ActivityID { get; }
    public abstract GameActivityStatus ActivityStatus { get; }
    public virtual int ActivityBeginLevel { get; }
    public virtual string ActivityCooldownSign { get; }
    public virtual float ActivityCooldownMinutes { get; }
    public virtual float ActivityDurationMinutes { get; }

    public SaveDataUtility mSaveUtility;

    // 基类用于维护活动状态
    private GameActivityStatus mLastActivityStatus;

    public BaseGameActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
    }

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
    public abstract void StartActivity();
    public abstract void RestartActivity();
    public virtual void CoolDownActivity()
    {
        
    }

    //活动注册后会自动调用
    public virtual void Tick()
    {
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

    /// <summary>
    /// 获取活动剩余时间
    /// </summary>
    /// <returns></returns>
    public string GetActivityReamingTime()
    {
        return CountDownTimerManager.Instance.GetRemainingTimeText(ActivitySign);
    }
}
