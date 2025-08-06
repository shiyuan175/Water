using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class VolcanicActivity : BaseGameActivity
{
    private const int VA_MAX_STREAK_WIN_NUM = 7;

    public int RewardCoins => mVolcanicActivityModel.GetVARewardCoins;
    public int VAStreakWinNum => mVolcanicActivityModel.VAStreakWinNum;
    public int VACurrentPlayerNum => mVolcanicActivityModel.VACurrentPlayerNum;
    //闯关胜利结束
    public bool EndWin => VAStreakWinNum >= VA_MAX_STREAK_WIN_NUM;

    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < GameDefine.GameConst.VA_BEGIN_LEVEL)
            {
                return GameActivityStatus.Locked;
            }

            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
            {
                return GameActivityStatus.Active;
            }

            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
            {
                return GameActivityStatus.CoolingDown;
            }

            else
            {
                return GameActivityStatus.WaitStart;
            }
        }
    }
    public override string ActivityID => GetType().Name;
    public override string ActivitySign => "VolcanicActivity";
    public override string ActivityCooldownSign => "VolcanicActivityCoolDown";
    public override float ActivityDurationMinutes => 1440;//0.2f;
    public override float ActivityCooldownMinutes => 60;//0.2f;

    private SaveDataUtility mSaveUtility;
    private VolcanicActivityModel mVolcanicActivityModel;

    public VolcanicActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        mVolcanicActivityModel = this.GetModel<VolcanicActivityModel>();

        //只在活动首次解锁时手动开启
        if (ActivityStatus == GameActivityStatus.WaitStart
            && !PlayerPrefs.HasKey(CountDownTimerManager.COUNTDOWN_TIMER_SIGN + ActivityCooldownSign))
        {
            //如果已有计时器则会失效(会由Tick驱动重启)
            StartActivity();
        }
    }

    public override void StreakWin()
    {
        mVolcanicActivityModel.AddVAStreakWin();
        if (EndWin)
        {
            CoolDownActivity();
        }
    }

    public override void Fail()
    {
        mVolcanicActivityModel.VA_Fail();
        CoolDownActivity();
    }
   
    public override void RestartActivityInit()
    {
        mVolcanicActivityModel.ReloadVolcanicActivity();
    }

    public override void CoolDownActivityInit()
    {
        //在这重置数据，会导致触发失败之后数据就重置，引起后面流程错误
        //mVolcanicActivityModel.ReloadVolcanicActivity();
    }
}
