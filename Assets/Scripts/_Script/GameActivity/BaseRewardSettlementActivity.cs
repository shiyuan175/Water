using System.Collections;
using System.Collections.Generic;
using GameDefine;
using QFramework;
using UnityEngine;

//奖励结算式活动基类
public abstract class BaseRewardSettlementActivity : IGameActivity, ICanGetModel, ICanGetUtility, ICanSendEvent
{
    public abstract bool IsRewardSettled { get; }
    public abstract bool HasRankReward { get; }
    public abstract string ActivitySign { get; }
    public virtual string ActivityID { get; }
    public virtual string ActivityCooldownSign { get; }
    public virtual float ActivityDurationMinutes { get; }
    public virtual float ActivityCooldownMinutes { get; }
    public virtual int ActivityBeginLevel { get; }
    public virtual SettlementActivityStatus ActivityStatus
    {
        get
        {
            if (!GameUtils.DoesCountDownKeyExist(GameConst.MAGIC_STREAK_ACTIVITY_SIGN))
            {
                return SettlementActivityStatus.Inactive;
            }

            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
            {
                return SettlementActivityStatus.Active;
            }

            if (!IsRewardSettled && HasRankReward)
            {
                return SettlementActivityStatus.Finished;
            }

            return SettlementActivityStatus.WaitStart;
        }
    }

    public SaveDataUtility mSaveUtility;

    private SettlementActivityStatus mLastActivityStatus;

    public BaseRewardSettlementActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        mLastActivityStatus = SettlementActivityStatus.None;
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public abstract void StreakWin();
    public abstract void Fail();
    public abstract void RestartActivityInit();
    public abstract void CoolDownActivityInit();
    public abstract void MarkRewardAsSettled();

    public virtual void StartActivity()
    {
        RestartActivityInit();
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign);
    }

    public virtual void RestartActivity()
    {
        RestartActivityInit();
        CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign);
    }

    public virtual void CoolDownActivity()
    {
        CoolDownActivityInit();
    }
    
    public virtual void Tick()
    {
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
}
