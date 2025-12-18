using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardADActivity : BaseGameADActivity
{
    
    public override string ActivitySign => GameDefine.GameConst.DAILY_REWARD_AD_ACTIVITY_SIGN;

    public override string ActivityID => GetType().Name;

    public override int ActivityBeginLevel => GameDefine.GameConst.DR_AD_BEGIN_LEVEL;
    public int CurrentWatchADCount => mRWModel.CurrentWatchADCount;
    public override GameActivityStatus ActivityStatus
    {
        get
        { if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
            {
                return GameActivityStatus.Locked;
            }
            else
                return GameActivityStatus.Active;


        }
    }

    private DailyRewardADActivityModel mRWModel;

    public DailyRewardADActivity()
    {
        mRWModel = this.GetModel<DailyRewardADActivityModel>();
       
    }


    public override void StartActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign);
    }

    public override void RestartActivity()
    {
        CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign);
        mRWModel.ClearStageNum();
    }

    public override void Tick()
    {
        if (GameUtils.DoesCountDownKeyExist(ActivitySign) &&
           CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
        {
            RestartActivity();
        }
        base.Tick();
    }

    public override void ADPlaybackCompleted<T>(T pack)
    {
        mRWModel.AddStageNum();
        mRewardGrantUtility.GrantReward((pack as GiftPackSO));
    }
}
