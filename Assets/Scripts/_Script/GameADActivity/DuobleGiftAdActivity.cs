using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using QFramework;

public class DuobleGiftAdActivity : BaseGameActivity, ICanGetModel
{
    public override string ActivitySign => GameConst.DOUBLEGIFT_AD_ACTIVITY_SIGN;
    public override string ActivityCooldownSign => GameConst.DOUBLEGIFT_COOL_AD_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;

    public override int ActivityBeginLevel => GameConst.DG_AD_BEGIN_LEVEL;



    public bool AllGot => mDGModel.IsBuy && mDGModel.GiftIsGot;

    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
                return GameActivityStatus.Locked;
            else if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
                return GameActivityStatus.Active;

            else return GameActivityStatus.CoolingDown;
        }
    }
    private DoubleGiftADActivityModel mDGModel;
    public DuobleGiftAdActivity()
    {
        mDGModel = this.GetModel<DoubleGiftADActivityModel>();
    }
    public override void RestartActivity()
    {
        mDGModel.ClearData();
        CountDownTimerManager.Instance.ResetCountdownTimer(ActivitySign, 7 * 30);
    }

    public override void StartActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign, 7 * 30);
    }
    public override void Tick()
    {
        switch (ActivityStatus)
        {
            case GameActivityStatus.Active:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign)||AllGot)
                    RestartActivity();
                break;

            case GameActivityStatus.CoolingDown:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign)) 
                    RestartActivity();

                break;
        }

        base.Tick();
    }

    public override void StreakWin()
    {
        if (AllGot)
            Fail();
    }

    public override void Fail()
    {
        CoolDownActivity();
    }
    public override void CoolDownActivity()
    {
        CountDownTimerManager.Instance.ResetCountdownTimer(ActivityCooldownSign, 7 * 24);
    }
    public void BuyGift()
    {
        mDGModel.SetIsBuy();
    }
    public void GotFreeGift()
    {
        mDGModel.GetGift();
    }
}
