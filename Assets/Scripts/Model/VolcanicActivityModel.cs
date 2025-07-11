using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class VolcanicActivityModel : AbstractModel, ICanGetModel
{
    private const string VA_STREAK_WIN_NUM_SIGN = "g_VolcanicActivityStreakWinNum";
    private const string VA_COUNT_PLAY_NUM_SIGN = "g_VolcanicActivityCountPlayerNum";
    private const int VA_REWARD_COUNT_COINS = 10000;

    private SaveDataUtility stroge;

    //火山活动连胜次数
    private BindableProperty<int> mVAStreakWinNum;
    public int VAStreakWinNum => mVAStreakWinNum.Value;

    //火山活动人数
    private BindableProperty<int> mVACountPlayerNum;
    public int VACurrentPlayerNum => mVACountPlayerNum.Value;

    public int GetVARewardCoins => VA_REWARD_COUNT_COINS / mVACountPlayerNum.Value;

    protected override void OnInit()
    {
        stroge = this.GetUtility<SaveDataUtility>();
        mVAStreakWinNum = new BindableProperty<int>();
        mVACountPlayerNum = new BindableProperty<int>();

        mVAStreakWinNum.SetValueWithoutEvent(stroge.LoadIntValue(VA_STREAK_WIN_NUM_SIGN));
        mVAStreakWinNum.Register(value =>
        {
            stroge.SaveInt(VA_STREAK_WIN_NUM_SIGN, value);
        });
        mVACountPlayerNum.SetValueWithoutEvent(stroge.LoadIntValue("g_VolcanicActivityCountPlayNum", 100));
        mVACountPlayerNum.Register(value =>
        {
            stroge.SaveInt(VA_COUNT_PLAY_NUM_SIGN, value);
        });
    }

    public void ReloadVolcanicActivity()
    {
        mVAStreakWinNum.Value = 0;
        mVACountPlayerNum.Value = 100;
    }

    public void AddVAStreakWin()
    {
        ++mVAStreakWinNum.Value;
        //每次通过一关，随机减少人数，可能基于当前连胜次数计算
        //然后保证最后通关的人数在一个区间内
    }
}
