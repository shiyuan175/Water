using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class VolcanicActivityModel : AbstractModel, ICanGetModel
{
    private const string VA_STREAK_WIN_NUM_SIGN = "F_VAStreakWinNum";
    private const string VA_COUNT_PLAY_NUM_SIGN = "F_VACountPlayerNum";
    private const string VA_ACTIVATE_STATE_SIGN = "F_VAActivateState";
    private const string VA_DAILY_REFRESH_COUNT_SIGN = "F_VADailyRefreshCount";
    private const int VA_REWARD_COUNT_COINS = 10000;

    private SaveDataUtility storage;

    //火山活动连胜次数
    private BindableProperty<int> mVAStreakWinNum;
    public int VAStreakWinNum => mVAStreakWinNum.Value;

    //火山活动人数
    private BindableProperty<int> mVACountPlayerNum;
    public int VACurrentPlayerNum => mVACountPlayerNum.Value;

    //每日刷新次数
    private BindableProperty<int> mVADailyUsedRefreshCount;
    public int VADailyUsedRefreshCount => mVADailyUsedRefreshCount.Value;

    //活动是否激活
    private BindableProperty<bool> mVAActivateState;
    public bool VAActivateState => mVAActivateState.Value;

    public int GetVARewardCoins => VA_REWARD_COUNT_COINS / mVACountPlayerNum.Value;

    protected override void OnInit()
    {
        //Debug.Log("数据初始化");
        storage = this.GetUtility<SaveDataUtility>();
        mVAStreakWinNum = new BindableProperty<int>();
        mVACountPlayerNum = new BindableProperty<int>();
        mVADailyUsedRefreshCount = new BindableProperty<int>();
        mVAActivateState = new BindableProperty<bool>();

        mVAStreakWinNum.SetValueWithoutEvent(storage.LoadIntValue(VA_STREAK_WIN_NUM_SIGN));
        mVAStreakWinNum.Register(value =>
        {
            storage.SaveInt(VA_STREAK_WIN_NUM_SIGN, value);
        });
        
        mVACountPlayerNum.SetValueWithoutEvent(storage.LoadIntValue(VA_COUNT_PLAY_NUM_SIGN, 100));
        mVACountPlayerNum.Register(value =>
        {
            storage.SaveInt(VA_COUNT_PLAY_NUM_SIGN, value);
        });

        mVADailyUsedRefreshCount.SetValueWithoutEvent(storage.LoadIntValue(VA_DAILY_REFRESH_COUNT_SIGN, 1));
        mVADailyUsedRefreshCount.Register(value =>
        {
            storage.SaveInt(VA_DAILY_REFRESH_COUNT_SIGN, value);
        });

        mVAActivateState.SetValueWithoutEvent(storage.LoadBoolValue(VA_ACTIVATE_STATE_SIGN, false));
        mVAActivateState.Register(value =>
        {
            storage.SaveBool(VA_ACTIVATE_STATE_SIGN, value);
        });
    }

    public void MarkActivateState()
    {
        mVAActivateState.Value = true;
    }

    /// <summary>
    /// 每日重置
    /// </summary>
    public void RefreshVolcanicActivity()
    {
        mVAStreakWinNum.Value = 0;
        mVACountPlayerNum.Value = 100;
        mVADailyUsedRefreshCount.Value = 1;
        mVAActivateState.Value = false;
    }

    /// <summary>
    /// 活动重置(每日三次)
    /// </summary>
    public void ReloadVolcanicActivity()
    {
        mVAStreakWinNum.Value = 0;
        mVACountPlayerNum.Value = 100;
    }

    public void AddDailyUsedRefreshCount()
    {
        ++mVADailyUsedRefreshCount.Value;
    }

    public void AddVAStreakWin()
    {
        ++mVAStreakWinNum.Value;
        if (mVAStreakWinNum.Value >= 7)
            mVAStreakWinNum.Value = 7;
        RandomReducePlayerNum();
    }

    public void VA_Fail()
    {
        RandomReducePlayerNum();
        mVAStreakWinNum.Value = 0;
    }

    private void RandomReducePlayerNum()
    {
        //策划文档
        //7	  6到12人
        //6   14到23人
        //5   27到35人
        //4   37到44人
        //3   48到53人
        //2   56到64人
        //1   66到72人

        // mVAStreakWinNum 由活动逻辑限制在 [0,7] 范围内
        switch (mVAStreakWinNum.Value)
        {
            case 1:
                mVACountPlayerNum.Value = Random.Range(66, 73);
                break;
            case 2:
                mVACountPlayerNum.Value = Random.Range(56, 65);
                break;
            case 3:
                mVACountPlayerNum.Value = Random.Range(48, 54);
                break;
            case 4:
                mVACountPlayerNum.Value = Random.Range(37, 45);
                break;
            case 5:
                mVACountPlayerNum.Value = Random.Range(27, 36);
                break;
            case 6:
                mVACountPlayerNum.Value = Random.Range(14, 24);
                break;
            case 7:
                mVACountPlayerNum.Value = Random.Range(6, 13);
                break;
            default:
                Debug.Log("Unexpected StreakWinNum!");
                break;
        }
    }
}
