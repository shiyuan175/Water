using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SepecialOfferADActivity : BaseGameADActivity
{
    public override string ActivitySign => GameConst.SEPECIALOFFER_AD_ACTIVITY_SIGN;

    public override string ActivityID => GetType().Name;

    public override int ActivityBeginLevel => GameConst.SO_AD_BEGIN_LEVEL;

    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
            {
                return GameActivityStatus.Locked;
            }
            else if(mSOModel.IsBuy)
            {
                return GameActivityStatus.CoolingDown;
            }
            else
            {
                return GameActivityStatus.Active;
            }
                
        }
    }
    private SepecialOfferADActivityModel mSOModel;
    public SepecialOfferADActivity()
    {
        mSOModel = this.GetModel<SepecialOfferADActivityModel>();
    }
    public override void RestartActivity()
    {
        CountDownTimerManager.Instance.ResetCountdownTimer(ActivitySign, 7 * 30);
        mSOModel.ReStartActivity();
    }

    public override void StartActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign, 7 * 30);
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
}
