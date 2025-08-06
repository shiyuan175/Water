using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using QFramework.Example;
using UnityEngine;

public class HighTowerActivity : BaseGameActivity
{
    public override string ActivitySign => "HighTowerActivity";
    public override string ActivityCooldownSign => "HighTowerActivityCoolDown";
    public override float ActivityDurationMinutes => 1440f;
    public override float ActivityCooldownMinutes => 1440f;
    public override string ActivityID => GetType().Name;
    
    public IReadOnlyList<int> RewardStages => mHTAModel.RewardStages;
    public int NextRewardStageIndex => mHTAModel.NextRewardStageIndex;
    public int WinRemainingToNextReward => mHTAModel.WinRemainingToNextReward;
    public int CurrentRewardStageGap => mHTAModel.CurrentRewardStageGap;
    public int HTAStreakWinNum => mHTAModel.HTAStreakWinNum;
    public bool EndWin => HTAStreakWinNum >= RewardStages.Last();
    public bool IsAtBaseNode => HTAStreakWinNum < RewardStages[1];
    public bool IsAtTopNode => HTAStreakWinNum >= RewardStages[RewardStages.Count - 2];

    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < GameDefine.GameConst.HTA_BEGIN_LEVEL)
            {
                return GameActivityStatus.Locked;
            }

            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivitySign))
            {
                return GameActivityStatus.Active;
            }

            if (!CountDownTimerManager.Instance.IsTimerFinished(ActivityCooldownSign))
            {
                return GameActivityStatus.CoolingDown;
            }

            else
            {
                return GameActivityStatus.WaitStart;
            }
        }
    }

    private HighTowerActivityModel mHTAModel;
    private SaveDataUtility mSaveUtility;

    public HighTowerActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        mHTAModel = this.GetModel<HighTowerActivityModel>();

        if (ActivityStatus == GameActivityStatus.WaitStart
            && !PlayerPrefs.HasKey(CountDownTimerManager.COUNTDOWN_TIMER_SIGN + ActivityCooldownSign))
        {
            StartActivity();
        }
    }

    public override void StreakWin()
    {
        mHTAModel.HTAStreakWin();
        if (EndWin)
        {
            CoolDownActivity();
        }
    }

    public override void Fail()
    {
        mHTAModel.HTAStreakLose();
    }

    public override void RestartActivityInit()
    {
        mHTAModel.ReloadHighTowerActivity();
    }

    public override void CoolDownActivityInit()
    {
        //如要在冷却时就重置数据，不能通过StreakWin自驱动重置
        //需手动在表现层逻辑触发完后，手动的触发判定是否活动冷却并重置数据
        //mHTAModel.ReloadHighTowerActivity();
    }
}
