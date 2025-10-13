using GameDefine;
using QFramework;

public class PassLevelCommand : AbstractCommand ,ICanGetModel
{
    private TierRankActivity mTierRankActivity;
    private BannerActivity mBannerActivity;
    private StageModel mStageModel;

    protected override void OnExecute()
    {
        int currentLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
        if (currentLevel == GameConst.VA_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<VolcanicActivity>();
        }
        if (currentLevel == GameConst.RA_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<RocketActivity>();
        }
        if (currentLevel == GameConst.HTA_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<HighTowerActivity>();
        }
        if (currentLevel == GameConst.WIN_STREAK_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<TierRankActivity>();
        }
        if (currentLevel == GameConst.SO_AD_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<SepecialOfferADActivity>();
        }
        if (currentLevel == GameConst.TT_AD_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<TurnTableADActivity>();
        }

        //通过第七关开启连胜活动
        if (currentLevel == GameConst.WIN_STREAK_BEGIN_LEVEL)
        {
            StringEventSystem.Global.Send(GameConst.START_POTION_ACTIVITY);
            //开启排行榜活动
            CountDownTimerManager.Instance.StartTimer(GameConst.RANKA_ACTIVITY_SIGN, 1440f);
        }

        //各模块增加连胜逻辑...
        mTierRankActivity ??= GameActivityManager.Instance.GetActivity<TierRankActivity>();
        mTierRankActivity?.StreakWin();

        mStageModel ??= this.GetModel<StageModel>();
        mStageModel?.PassLevel();

        mBannerActivity ??= GameActivityManager.Instance.GetActivity<BannerActivity>();
        if (mBannerActivity?.ActivityStatus == GameActivityStatus.Active)
            mBannerActivity.StreakWin();
    }
}
