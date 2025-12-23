using GameDefine;
using QFramework;

public class DuobleGiftAdActivity : BaseGameActivity, ICanGetModel
{
    public override string ActivitySign => GameConst.DOUBLE_GIFT_AD_ACTIVITY_SIGN;
    public override string ActivityCooldownSign => GameConst.DOUBLE_GIFT_COOL_AD_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;
    public override int ActivityBeginLevel => GameConst.DG_AD_BEGIN_LEVEL;
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
    public bool AllGot => mDGModel.IsBuy && mDGModel.GiftIsGot;

    private readonly DoubleGiftADActivityModel mDGModel;

    private GameActivityStatus mCurrentStatus;

    public DuobleGiftAdActivity()
    {
        mDGModel = this.GetModel<DoubleGiftADActivityModel>();
        mCurrentStatus = GameActivityStatus.None;
    }

    public override void StartActivity()
    {

    }

    public override void RestartActivity()
    {
        CountDownTimerManager.Instance.DeleteTimer(ActivityCooldownSign);

        mDGModel.ClearData();
    }

    public override void CoolDownActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivityCooldownSign, 7);
    }

    public override void Tick()
    {
        switch (mCurrentStatus)
        {
            case GameActivityStatus.None:
                if (CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
                {
                    RestartActivity();
                    mCurrentStatus = GameActivityStatus.Active;
                }
                else mCurrentStatus = GameActivityStatus.CoolingDown;
                break;

            case GameActivityStatus.Active:
                if (AllGot)
                {
                    CoolDownActivity();
                    mCurrentStatus = GameActivityStatus.CoolingDown;
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
    }

    public override void StreakWin()
    {
    }

    public override void Fail()
    {
        CoolDownActivity();
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
