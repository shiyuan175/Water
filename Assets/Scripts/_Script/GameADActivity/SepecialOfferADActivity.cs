using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SepecialOfferADActivity : BaseGameActivity
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
        mSOModel.ChangeSepecialOfferIsbuy(false);
        CountDownTimerManager.Instance.ResetCountdownTimer(ActivitySign, 30);
    }

    public override void StartActivity()
    {
        mSOModel.ChangeSepecialOfferIsbuy(false);
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign,30);
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
    public void BuyGift()
    {
        mSOModel.ChangeSepecialOfferIsbuy(true);
    }

    public override void StreakWin()
    {
        
    }

    public override void Fail()
    {
        
    }
}
