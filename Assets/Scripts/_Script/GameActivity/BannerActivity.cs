using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerActivity : BaseGameActivity
{
    public override string ActivitySign => GameDefine.GameConst.BANNER_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;
    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign) 
                && !ProgressEnd)
            {
                return GameActivityStatus.Active;
            }
            
            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign)
                && ProgressEnd)
            {
                return GameActivityStatus.CoolingDown;
            }

            if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
            {
                return GameActivityStatus.WaitStart;
            }
            
            return GameActivityStatus.None;
        }
    }

    public int BAStreakWin => mBannerActivityModel.BAStreakWin;
    public int BACurrentGoal => mBannerActivityModel.BACurrentGoal;
    public int BARewardProgress => mBannerActivityModel.BARewardProgress;
    //五档连胜所加积分
    public int WinStreakPoints => mBannerActivityModel.WinStreakPoints;
    //档位奖励是否领取完
    public bool ProgressEnd => mBannerActivityModel.ProgressEnd;
    public int[] Reware_Target_Goals => mBannerActivityModel.Reware_Target_Goals;
    //活动连胜档位映射(用于表现层索引)
    public int WinStreakLevel => BAStreakWin switch
    {
        >= 5 => 4,
        4 => 3,
        3 => 2,
        2 => 1,
        1 => 0,
        _ => 0
    };

    private BannerActivityModel mBannerActivityModel;

    public BannerActivity()
    {
        mBannerActivityModel = this.GetModel<BannerActivityModel>();

        if (!GameUtils.DoesCountDownKeyExist(ActivitySign))
            StartActivity();
    }
  
    public override void StartActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign, 3);
    }

    public override void RestartActivity()
    {
        mBannerActivityModel.ResetBA();
        CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign, 3);
    }

    public override void StreakWin()
    {
        mBannerActivityModel.BAStrekWin();
    }

    public override void Fail()
    {
        mBannerActivityModel.BAFail();
    }

    //表现层领取奖励时调用
    public void NextRewardProgress()
    {
        mBannerActivityModel.NextRewardProgress();
    }

    public override void Tick()
    {
        switch (ActivityStatus)
        {
            case GameActivityStatus.WaitStart:
                RestartActivity();
                break;
        }


        base.Tick();
    }
}
