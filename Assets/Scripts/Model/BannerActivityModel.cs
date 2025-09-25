using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerActivityModel : AbstractModel, ICanGetModel
{
    public int BAStreakWin => mBAStreakWin.Value; 
    public int BACurrentGoal => mBACurrentGoal.Value;
    public int BATotalGoal => mBATotalGoal.Value;
    public int BARewardProgress => mBARewardProgress.Value;
    //五档连胜所加积分
    public int WinStreakPoints => mBAStreakWin.Value switch
    {
        >= 5 => 100,
        4 => 25,
        3 => 10,
        2 => 5,
        1 => 1,
        _ => 0
    };
    //档位奖励是否领取完
    public bool ProgressEnd => mBARewardProgress.Value >= REWARD_TARGET_GOALS.Length - 1;
    public int[] Reware_Target_Goals => REWARD_TARGET_GOALS;

    private const string BA_STREAK_WIN = "I_BAStreakWin";
    private const string BA_CURRENT_GOAL = "I_BACurrentGoal";
    private const string BA_TOTAL_GOAL = "I_BATotalGoal";
    private const string BA_REWARD_PROGRESS = "I_BARewardProgress";
    //各档位所需积分(25档)
    private readonly int[] REWARD_TARGET_GOALS = new int[]
    {
        1,20,100,300,200,300,400,400,400,500,400,600,500,400,400,800,600,800,700,900,800,800,1000,2000,5000
    };

    private BindableProperty<int> mBAStreakWin;
    private BindableProperty<int> mBACurrentGoal;
    private BindableProperty<int> mBATotalGoal;
    private BindableProperty<int> mBARewardProgress;

    private SaveDataUtility mSaveDataUtility;
    private StageModel mStageModel;

    protected override void OnInit()
    {
        mSaveDataUtility = this.GetUtility<SaveDataUtility>();
        mStageModel = this.GetModel<StageModel>();

        mBAStreakWin = new BindableProperty<int>();
        mBACurrentGoal = new BindableProperty<int>();
        mBATotalGoal = new BindableProperty<int>();
        mBARewardProgress = new BindableProperty<int>();

        mBAStreakWin.SetValueWithoutEvent(mSaveDataUtility.LoadIntValue(BA_STREAK_WIN));
        mBAStreakWin.Register(value =>
        {
            mSaveDataUtility.SaveInt(BA_STREAK_WIN, value);
        });

        mBACurrentGoal.SetValueWithoutEvent(mSaveDataUtility.LoadIntValue(BA_CURRENT_GOAL));
        mBACurrentGoal.Register(value =>
        {
            mSaveDataUtility.SaveInt(BA_CURRENT_GOAL, value);
        });

        mBATotalGoal.SetValueWithoutEvent(mSaveDataUtility.LoadIntValue(BA_TOTAL_GOAL));
        mBATotalGoal.Register(value =>
        {
            mSaveDataUtility.SaveInt(BA_TOTAL_GOAL, value);
        });

        mBARewardProgress.SetValueWithoutEvent(mSaveDataUtility.LoadIntValue(BA_REWARD_PROGRESS));
        mBARewardProgress.Register(value =>
        {
            mSaveDataUtility.SaveInt(BA_REWARD_PROGRESS, value);
        });
    }

    //活动重置
    public void ResetBA()
    {
        mBAStreakWin.Value = 0;
        mBACurrentGoal.Value = 0;
        mBATotalGoal.Value = 0;
        mBARewardProgress.Value = 0;
    }

    //连胜
    public void BAStrekWin()
    {
        ++mBAStreakWin.Value;
        mBACurrentGoal.Value += WinStreakPoints * mStageModel.SettlementMultiple;
        mBATotalGoal.Value += WinStreakPoints * mStageModel.SettlementMultiple;
    }

    //失败
    public void BAFail()
    {
        mBAStreakWin.Value = 0;
    }

    //进入下一奖励进度
    public void NextRewardProgress()
    {
        if (!ProgressEnd)
        {
            mBACurrentGoal.Value -= REWARD_TARGET_GOALS[mBARewardProgress.Value];
            ++mBARewardProgress.Value;
        }
    }
}
