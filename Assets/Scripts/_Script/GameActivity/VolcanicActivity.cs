using System;
using System.Collections;
using System.Collections.Generic;
using GameDefine;
using QFramework;
using UnityEngine;

public class VolcanicActivity : BaseGameActivity
{
    public int RewardCoins => mVolcanicActivityModel.GetVARewardCoins;
    public int VAStreakWinNum => mVolcanicActivityModel.VAStreakWinNum;
    public int VACurrentPlayerNum => mVolcanicActivityModel.VACurrentPlayerNum;
    //闯关胜利结束
    public bool EndWin => VAStreakWinNum >= VA_MAX_STREAK_WIN_NUM;
    public bool VAActivateState => mVolcanicActivityModel.VAActivateState;

    public override string ActivityID => GetType().Name;
    public override string ActivitySign => "VolcanicActivity";
    public override string ActivityCooldownSign => "VolcanicActivityCoolDown";
    public override float ActivityDurationMinutes => 1440;//0.2f;
    public override float ActivityCooldownMinutes => 60;//0.2f;
    public override int ActivityBeginLevel => GameConst.VA_BEGIN_LEVEL;
   
    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
                return GameActivityStatus.Locked;
            if (!VAActivateState)
                return GameActivityStatus.Inactive;
            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
                return GameActivityStatus.Active;
            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
                return GameActivityStatus.CoolingDown;

            return GameActivityStatus.WaitStart;
        }
    }

    private const int VA_MAX_STREAK_WIN_NUM = 7;
    private VolcanicActivityModel mVolcanicActivityModel;
    private GameActivityStatus mCurrentStatus;

    public VolcanicActivity()
    {
        mVolcanicActivityModel = this.GetModel<VolcanicActivityModel>();
        mCurrentStatus = GameActivityStatus.None;
    }

    public override void StartActivity()
    {
        mVolcanicActivityModel.MarkActivateState();
        CountDownTimerManager.Instance.StartTimer(ActivitySign, ActivityDurationMinutes);
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

    public override void RestartActivity()
    {
        //数据重置
        mVolcanicActivityModel.ReloadVolcanicActivity();

        CountDownTimerManager.Instance.ResetTimer(ActivitySign, ActivityDurationMinutes);
        CountDownTimerManager.Instance.DeleteTimer(ActivityCooldownSign);
    }

    public override void CoolDownActivity()
    {
        //依赖于计时器是否存在,重置时间是以触发重置为起始时间,而不是以结束时间为起始时间(针对于下线后上线活动已过期)
        //如主动触发了冷却，以活动结束时计算冷却
        //如下线了后活之动过期，则以上线的时间点计算冷却(采用这种方法可以在上线的时候领取结算奖励)
        //解决方法：
        //在开启冷却的时候去获取一下活动结束的时间点，然后以结束时间为准开启冷却(可能存在马上结束又冷却结束)

        CountDownTimerManager.Instance.DeleteTimer(ActivitySign);
        CountDownTimerManager.Instance.StartTimer(ActivityCooldownSign, ActivityCooldownMinutes);
    }

    public override void Tick()
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
                if (!GameUtils.DoesCountDownKeyExist(ActivityCooldownSign))
                {
                    CoolDownActivity();
                    mCurrentStatus = GameActivityStatus.CoolingDown;
                    break;
                }

                if (CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
                {
                    RestartActivity();
                    mCurrentStatus = GameActivityStatus.Active;
                }
                break;

            case GameActivityStatus.None:
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

        base.Tick();
    }
}
