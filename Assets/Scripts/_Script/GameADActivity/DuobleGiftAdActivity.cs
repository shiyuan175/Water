using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using QFramework;

public class DuobleGiftAdActivity : BaseGameActivity, ICanGetModel
{
    public override string ActivitySign => GameConst.DOUBLE_GIFT_AD_ACTIVITY_SIGN;
    public override string ActivityCooldownSign => GameConst.DOUBLE_GIFT_COOL_AD_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;

    public override int ActivityBeginLevel => GameConst.DG_AD_BEGIN_LEVEL;


    private GameActivityStatus mCurrentStatus;
    public bool AllGot => mDGModel.IsBuy && mDGModel.GiftIsGot;

    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
                return GameActivityStatus.Locked;
            else if (AllGot)
                return GameActivityStatus.CoolingDown;

            else return GameActivityStatus.Active;
        }
    }
    private DoubleGiftADActivityModel mDGModel;
    public DuobleGiftAdActivity()
    {
        mDGModel = this.GetModel<DoubleGiftADActivityModel>();
        mCurrentStatus = ActivityStatus;
    }
    public override void RestartActivity()
    {

        CountDownTimerManager.Instance.ResetCountdownTimer(ActivityCooldownSign, 7 * 30);

        mDGModel.ClearData();
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
                if (AllGot)
                {
                    CoolDownActivity();
                    mCurrentStatus = GameActivityStatus.CoolingDown;
                }

                else if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
                {

                    RestartActivity();
                    mCurrentStatus = GameActivityStatus.Active;
                }

                break;

            case GameActivityStatus.CoolingDown:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
                {

                    RestartActivity();
                    mCurrentStatus = GameActivityStatus.Active;
                }

                break;
        }

        base.Tick();
    }

    public override void StreakWin()
    {/*
        if (AllGot)
            Fail();*/
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
