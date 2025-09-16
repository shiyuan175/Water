using System.Collections;
using System.Collections.Generic;
using GameDefine;
using QFramework;
using UnityEngine;

//奖励结算式活动基类
public abstract class BaseRewardSettlementActivity : IGameActivity, ICanGetModel, ICanGetUtility, ICanSendEvent
{
    //活动是否结算(活动结束)
    public abstract bool IsRewardSettled { get; }
    public abstract string ActivitySign { get; }
    public virtual string ActivityID { get; }
    public virtual string ActivityCooldownSign { get; }
    public virtual float ActivityDurationMinutes { get; }
    public virtual float ActivityCooldownMinutes { get; }
    public virtual int ActivityBeginLevel { get; }
    //是否有奖励可结算(达到某个目标触发)
    public virtual bool HasRankReward { get; }

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
    public abstract void MarkRewardAsSettled();

    public virtual void StartActivity()
    {
        Debug.Log("sda");
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
