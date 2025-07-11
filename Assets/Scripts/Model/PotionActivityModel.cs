using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class PotionActivityModel : AbstractModel, ICanGetModel
{
    //五档奖励
    private const int MAX_PROGRESS = 5;
    private const string POTION_ACTIVITY_GOAL_SIGN = "PotionActivityGoal";
    private const string POTION_ACTIVITY_PROGRESS_SIGN = "PotionActivityProgress";
    private const string POTION_ACTIVITY_TOTAL_GOAL_SIGN = "PotionActivityTotalGoal";

    private StageModel stageModel;
    private SaveDataUtility saveDataUtility;

    //连胜积分
    public int PotionActivityGoal => mPotionActivityGoal.Value;
    private BindableProperty<int> mPotionActivityGoal;

    //获得的总积分(用于排行榜、排名依赖于该积分)
    private BindableProperty<int> mPotionActivityTotalGoal;
    public int PotionActivityTotalGoal => mPotionActivityTotalGoal.Value;

    //活动进度(第几个宝箱)
    public int PotionActivityProgress => mPotionActivityProgress.Value;
    private BindableProperty<int> mPotionActivityProgress;

    //活动进度是否结束(五档奖励领取完成)
    public bool PotionActivityProgressEnd => mPotionActivityProgress.Value >= MAX_PROGRESS;

    //五档连胜所加积分
    public int WinStreakPoints => stageModel.CountinueWinNum switch
    {
        >= 5 => 100,
        4 => 25,
        3 => 10,
        2 => 5,
        1 => 1,
        _ => 0
    };

    //活动连胜档位映射(用于索引)
    public int WinStreakLevel => stageModel.CountinueWinNum switch
    {
        >= 5 => 4,
        4 => 3,
        3 => 2,
        2 => 1,
        1 => 0,
        _ => 0
    };

    protected override void OnInit()
    {
        stageModel = this.GetModel<StageModel>();
        saveDataUtility = this.GetUtility<SaveDataUtility>();
        mPotionActivityGoal = new BindableProperty<int>();
        mPotionActivityTotalGoal = new BindableProperty<int>();
        mPotionActivityProgress = new BindableProperty<int>();

        mPotionActivityGoal.SetValueWithoutEvent(saveDataUtility.LoadIntValue(POTION_ACTIVITY_GOAL_SIGN));
        mPotionActivityGoal.Register(value =>
        {
            saveDataUtility.SaveInt(POTION_ACTIVITY_GOAL_SIGN, value);
        });

        mPotionActivityTotalGoal.SetValueWithoutEvent(saveDataUtility.LoadIntValue(POTION_ACTIVITY_TOTAL_GOAL_SIGN));
        mPotionActivityTotalGoal.Register(value =>
        {
            saveDataUtility.SaveInt(POTION_ACTIVITY_TOTAL_GOAL_SIGN, value);
        });

        mPotionActivityProgress.SetValueWithoutEvent(saveDataUtility.LoadIntValue(POTION_ACTIVITY_PROGRESS_SIGN));
        mPotionActivityProgress.Register(value =>
        {
            saveDataUtility.SaveInt(POTION_ACTIVITY_PROGRESS_SIGN, value);
        });
    }

    public void AddPotionActivityGoal()
    {
        mPotionActivityGoal.Value += WinStreakPoints;
        mPotionActivityTotalGoal.Value += WinStreakPoints;
    }

    public void ReducePotionActivityGoal(int goal)
    {
        mPotionActivityGoal.Value -= goal;
    }

    public void AddPotionActivityProgress()
    {
        mPotionActivityProgress.Value += 1;
    }
}
