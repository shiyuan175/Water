using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTableADActivity : BaseGameADActivity
{
    public override string ActivitySign => GameConst.TurnTable_AD_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;
    public override float ActivityDurationMinutes => 24 * 60;

    public override int ActivityBeginLevel => GameConst.TT_AD_BEGIN_LEVEL;
    /// <summary>
    /// 当前是第几次转转盘
    /// </summary>
    public int CurrentTurnTableCount =>mTTModel.CurrentTurnTableCount;

    /// <summary>
    ///  直接触发
    /// </summary>
    public override GameActivityStatus ActivityStatus
    {
        get
        {

            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
            {
                return GameActivityStatus.Locked;
            }
            else
                return GameActivityStatus.Active;
        }
    }


    private TurnTableADActivityModel mTTModel;

    public TurnTableADActivity()
    {
        mTTModel = this.GetModel<TurnTableADActivityModel>();
        
    }

    public override void StartActivity()
    {

        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign);
    }

    public override void RestartActivity()
    {
        Debug.Log("s3");
        CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign);
        mTTModel.TurnTableTimeEnd();
    }

    public override void Tick()
    {
        Debug.Log(1);
        if (GameUtils.DoesCountDownKeyExist(ActivitySign) &&
           CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
        {
            Debug.Log("DAS");
 RestartActivity();
        }
           

        base.Tick();
    }

    public override void ADPlaybackCompleted(GameObject target)
    {     
        mTTModel.TurnTableAnimationEnd();

        StageModel _stageModel = this.GetModel<StageModel>();


    }
     
}
