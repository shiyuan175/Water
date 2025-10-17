using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedLevelCommand : AbstractCommand ,ICanGetModel
{
    private TierRankActivity mTierRankActivity;
    private BannerActivity mBannerActivity;
    private StageModel mStageModel;

    protected override void OnExecute()
    {
        //各模块连胜重置逻辑...
        mTierRankActivity ??= GameActivityManager.Instance.GetActivity<TierRankActivity>();
        if (mTierRankActivity?.ActivityStatus is SettlementActivityStatus.Active)
            mTierRankActivity?.Fail();

        mStageModel ??= this.GetModel<StageModel>();
        mStageModel?.ResetCountinueWinNum();

        mBannerActivity ??= GameActivityManager.Instance.GetActivity<BannerActivity>();
        if (mBannerActivity?.ActivityStatus == GameActivityStatus.Active)
            mBannerActivity.Fail();
    }
}
