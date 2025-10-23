using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDefine;
using QFramework;
using QFramework.Example;
using UnityEngine;

public class HighTowerActivity : BaseGameActivity
{
    public override string ActivitySign => GameConst.HIGH_TOWER_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;
    public override float ActivityDurationMinutes => throw new NotImplementedException("Unconfigured HTA Duration");
    public override int ActivityBeginLevel => GameConst.HTA_BEGIN_LEVEL;

    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
                return GameActivityStatus.Locked;

            else if (!GameUtils.DoesCountDownKeyExist(ActivitySign))
                return GameActivityStatus.Inactive;

            else if (!EndWin)
                return GameActivityStatus.Active;

            else return GameActivityStatus.CoolingDown;
        }
    }

    public IReadOnlyList<int> RewardStages => mHTAModel.RewardStages;
    public int NextRewardStageIndex => mHTAModel.NextRewardStageIndex;
    public int HTAStreakWinNum => mHTAModel.HTAStreakWinNum;
    public bool EndWin => HTAStreakWinNum >= RewardStages.Last();
    public bool IsAtBaseNode => HTAStreakWinNum < RewardStages[1];
    public bool IsAtTopNode => HTAStreakWinNum >= RewardStages[RewardStages.Count - 2];

    private HighTowerActivityModel mHTAModel;

    public HighTowerActivity()
    {
        mHTAModel = this.GetModel<HighTowerActivityModel>();
    }

    public override void StartActivity()
    {
        CountDownTimerManager.Instance.StartEasternMidnightTimer(ActivitySign, 2);
    }

    public override void RestartActivity()
    {
        mHTAModel.ReloadHighTowerActivity();
        //手动开启
        CountDownTimerManager.Instance.DeleteTimer(ActivitySign);
        //自动重置
        //CountDownTimerManager.Instance.ResetEasternMidnightTimer(ActivitySign, 2);
    }

    public override void StreakWin()
    {
        mHTAModel.HTAStreakWin();
    }

    public override void Fail()
    {
        mHTAModel.HTAStreakLose();
    }

    public override void Tick()
    {
        if (GameUtils.DoesCountDownKeyExist(ActivitySign) &&
           CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
            RestartActivity();

        base.Tick();
    }
}
