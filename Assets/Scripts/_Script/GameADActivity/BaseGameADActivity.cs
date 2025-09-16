using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameADActivity : IGameActivity, ICanGetModel, ICanGetUtility, ICanSendEvent
{
    public abstract string ActivitySign { get; }
    public abstract string ActivityID { get; }
    // 活动触发的时机（关卡）
    public abstract int ActivityBeginLevel { get; }

    public abstract GameActivityStatus ActivityStatus { get; }

    public virtual string ActivityCooldownSign { get; }
    public virtual float ActivityCooldownMinutes { get; }
    public virtual float ActivityDurationMinutes { get; }

    public SaveDataUtility mSaveUtility;
    public StageModel mStageModel;
    public RewardGrantUtility mRewardGrantUtility;

    // 基类用于维护活动状态
    private GameActivityStatus mLastActivityStatus;
    /// <summary>
    /// 获取活动剩余时间
    /// </summary>
    /// <returns></returns>

    public BaseGameADActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        mStageModel = this.GetModel<StageModel>();
        mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();
    }
    public abstract void StartActivity();
    public abstract void RestartActivity();
    public virtual void CoolDownActivity()
    {

    }
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
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
    public string GetActivityReamingTime()
    {
        return CountDownTimerManager.Instance.GetRemainingTimeText(ActivitySign);
    }
    public virtual void Fail()
    {

    }
   
    public virtual void StreakWin()
    {
        
    }

    public virtual void ADPlaybackCompleted(GameObject target)
    {

    }
}
