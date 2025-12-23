using GameDefine;
using QFramework;

public class SepecialOfferADActivity : BaseGameADActivity
{
    public override string ActivitySign => GameConst.SPECIAL_OFFER_AD_ACTIVITY_SIGN;

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
        mSOModel.ReStartActivity();
    }

    public override void StartActivity() 
    {
    }

    public override void CoolDownActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign, 7);
    }

    public override void Tick()
    {
        if (CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
        {
            RestartActivity();
        }
    }
}
