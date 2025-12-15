using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedLevelCommand : AbstractCommand ,ICanGetModel
{
    private TierRankActivity mTierRankActivity;
    private BannerActivity mBannerActivity;
    private GameGlobalModel mGameGlobalModel;

    protected override void OnExecute()
    {
        //��ģ����ʤ�����߼�...
        mTierRankActivity ??= GameActivityManager.Instance.GetActivity<TierRankActivity>();
        if (mTierRankActivity?.ActivityStatus is SettlementActivityStatus.Active)
            mTierRankActivity?.Fail();

        mGameGlobalModel ??= this.GetModel<GameGlobalModel>();
        mGameGlobalModel?.ResetCountinueWinNum();

        mBannerActivity ??= GameActivityManager.Instance.GetActivity<BannerActivity>();
        if (mBannerActivity?.ActivityStatus == GameActivityStatus.Active)
            mBannerActivity.Fail();
    }
}
