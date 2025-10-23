using GameDefine;
using JsonFileData;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrograssGiftADActivity : BaseGameADActivity,ICanGetModel
{
    public override string ActivitySign => GameDefine.GameConst.PROGRASSGIFT_AD_ACTIVITY_SIGN;

    public override string ActivityID => GetType().Name;

    public override int ActivityBeginLevel => GameDefine.GameConst.PG_AD_BEGIN_LEVEL;

    private PrograssGiftADActivityModel mPGModel;
    public override float ActivityDurationMinutes => 30 * 24 * 60;
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

    public PrograssGiftADActivity()
    {
        mPGModel = this.GetModel<PrograssGiftADActivityModel>();
        mPGModel.LoadPGActivity();
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

        // 未领取的奖励发放?
        mPGModel.ReloadPGActivity();
        mPGModel.LoadPGActivity();
    }
    /// <summary>
    /// 发放奖励
    /// </summary>
    /// <param name="pack">礼包</param>
    /// <param name=""></param>
    public void DistributeReward(System.Action PlayAnim ,RewardItem[] pack)
    {
        RewardUIManager.Instance.PlayRewardAnim(PlayAnim, false, pack);
        mRewardGrantUtility.GrantReward(pack);
    }

}
