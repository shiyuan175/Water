using GameDefine;
using JsonFileData;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassADActivity : BaseGameADActivity, ICanRegisterEvent
{
    public override string ActivitySign => GameConst.BATTLEPASS_AD_ACTIVITY_SIGN;

    public override string ActivityID => GetType().Name;

    public override int ActivityBeginLevel => GameConst.BP_AD_BEGIN_LEVEL;

    private BattlePassModel mBPModel;
    public override float ActivityDurationMinutes => 30*24*60;
    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
            {
                return GameActivityStatus.Locked;
            }
            else
                return GameActivityStatus.Active;
        }
    }
    private GameActivityStatus mCurrentStatus;
    public BattlePassADActivity()
    {
        mBPModel = this.GetModel<BattlePassModel>();
        /*        if (GameUtils.DoesCountDownKeyExist(GameConst.BATTLEPASS_AD_ACTIVITY_SIGN))*/
        mBPModel.LoadBattlePassActivity();
        this.RegisterEvent<ReturnToMainEvent>((_event) =>
        {
            // 活动启动才计数
            // 活动启动才计数
            if (ActivityStatus == GameActivityStatus.Active && _event.PassLevel)
                mBPModel.AddGameWinCount();
        });
    }
    public override void Tick()
    {
        if (!GameUtils.DoesCountDownKeyExist(ActivitySign) &&
           CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
        {
            RestartActivity();
        }

        base.Tick();
    }

    public override void StartActivity()
    {
        CountDownTimerManager.Instance.StartTimer(ActivitySign, ActivityDurationMinutes);
    }

    public override void RestartActivity()
    {
        CountDownTimerManager.Instance.ResetTimer(ActivitySign, ActivityDurationMinutes);
        mBPModel.LoadBattlePassActivity();
        // 未领取的奖励发放?
        mBPModel.ReloadBattlePassActivity();
        mBPModel.LoadBattlePassActivity();
    }
    /// <summary>
    /// 发放奖励
    /// </summary>
    /// <param name="pack">礼包</param>
    /// <param name="isVIPPack"></param>
    public void DistributeReward(RewardItem[] pack, bool isVipPack = false,int level = 0)
    {  
        RewardUIManager.Instance.PlayRewardAnim( null, false, pack);
        mRewardGrantUtility.GrantReward(pack);
        mBPModel.AddRewardGotLevel(isVipPack, level);

    }
}
