using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class TierRankActivity : BaseRewardSettlementActivity
{
    public override bool IsRewardSettled => false;
    public override bool HasRankReward => false;
    public override string ActivitySign => GameDefine.GameConst.TIER_RANK_ACTIVITY_SIGN;

    //活动状态定义重写，用于重置活动(现在不确定有几个状态要用)


    public int StreakWinNum => mTRAModel.StreakWinNum;
    


    private TierRankActivityModel mTRAModel;

    public TierRankActivity()
    {
        mTRAModel = this.GetModel<TierRankActivityModel>();

    }

    #region 由过关/退出关卡直接调用数据层更新

    public override void StreakWin()
    {
    }

    public override void Fail()
    {
    }
    #endregion

    //标记奖励结算
    public override void MarkRewardAsSettled()
    {

    }

    public override void RestartActivityInit()
    {
        mTRAModel.ResetStreakWinNum();
    }

    public override void Tick()
    {
        //活动重启逻辑


        base.Tick();
    }
}
