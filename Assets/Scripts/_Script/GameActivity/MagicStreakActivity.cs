using System.Collections;
using System.Collections.Generic;
using GameDefine;
using JsonFileData;
using QFramework;
using UnityEngine;

public class MagicStreakActivity : BaseRewardSettlementActivity
{
    public override string ActivitySign => GameDefine.GameConst.MAGIC_STREAK_ACTIVITY_SIGN;
    public override bool HasRankReward => PlayerRank <= REWARD_RANK_THRESHOLD;
    public override bool IsRewardSettled => mMSAModel.IsRewardSettled;
    public override int ActivityBeginLevel => GameDefine.GameConst.MS_BEGIN_LEVEL;
    public override SettlementActivityStatus ActivityStatus
    {
        get
        {
            if (!GameUtils.DoesCountDownKeyExist(ActivitySign))
            {
                return SettlementActivityStatus.Inactive;
            }

            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
            {
                return SettlementActivityStatus.Active;
            }

            if (!IsRewardSettled && HasRankReward)
            {
                return SettlementActivityStatus.Finished;
            }

            return SettlementActivityStatus.WaitStart;
        }
    }

    //同分玩家排名靠后(排名从1开始,不会取到0)
    public int PlayerRank
    {
        get
        {
            int left = 0;
            int right = MSAData.MSARobots.Count - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (MSAData.MSARobots[mid].Score >= MSAData.Player.Score)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return left + 1;
        }
    }
    public int CurStageReward => mMSAModel.CurStageReward;
    public int StreakWinNum => mMSAModel.StreakWinNum;
    public MSActivityData MSAData => mMSAModel.MSAData;

    private const int REWARD_RANK_THRESHOLD = 20;
    private MagicStreakActivityModel mMSAModel;

    public MagicStreakActivity()
    {
        mMSAModel = this.GetModel<MagicStreakActivityModel>();
        if (GameUtils.DoesCountDownKeyExist(GameConst.MAGIC_STREAK_ACTIVITY_SIGN))
            mMSAModel.LoadMagicStreakActivity();
    }

    public override void StreakWin()
    {
        mMSAModel.StreakWin();
    }

    public override void Fail()
    {
        mMSAModel.Fail();
    }

    public override void StartActivity()
    {
        RestartActivityInit();
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign, 3);
    }

    public override void RestartActivity()
    {
        RestartActivityInit();

        //手动重置活动
        CountDownTimerManager.Instance.DeleteTimer(ActivitySign);
        //自动重置活动
        //CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign, 3);
    }

    public override void RestartActivityInit()
    {
        mMSAModel.ReloadMagicStreakActivity();
        mMSAModel.LoadMagicStreakActivity();
    }

    /// <summary>
    /// 标记活动已结算
    /// </summary>
    public override void MarkRewardAsSettled()
    {
        mMSAModel.MarkRewardAsSettled();
    }

    public override void Tick()
    {
        if (ActivityStatus is SettlementActivityStatus.WaitStart)
            RestartActivity();

        base.Tick();
    }

    public void MarkNextStageRewardIdnex()
    {
        mMSAModel.MarkNextStageRewardIdnex();
    }
}
