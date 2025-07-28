using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class VolcanicActivityModel : AbstractModel, ICanGetModel
{
    private const string VA_STREAK_WIN_NUM_SIGN = "g_VolcanicActivityStreakWinNum";
    private const string VA_COUNT_PLAY_NUM_SIGN = "g_VolcanicActivityCountPlayerNum";
    private const int VA_REWARD_COUNT_COINS = 10000;

    private bool isInit = false;
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
        if (!isInit)
        {
            stroge = this.GetUtility<SaveDataUtility>();
            mVAStreakWinNum = new BindableProperty<int>();
            mVACountPlayerNum = new BindableProperty<int>();

            mVAStreakWinNum.SetValueWithoutEvent(stroge.LoadIntValue(VA_STREAK_WIN_NUM_SIGN));
            mVAStreakWinNum.Register(value =>
            {
                stroge.SaveInt(VA_STREAK_WIN_NUM_SIGN, value);
            });
            mVACountPlayerNum.SetValueWithoutEvent(stroge.LoadIntValue(VA_COUNT_PLAY_NUM_SIGN, 100));
            //Debug.Log(mVACountPlayerNum.Value);
            mVACountPlayerNum.Register(value =>
            {
                stroge.SaveInt(VA_COUNT_PLAY_NUM_SIGN, value);
            });
            isInit = true;
        }
    }

    public void ReloadVolcanicActivity()
    {
        //启用时注册活动初始化数据会因生命周期问题还没初始化数据
        //OnInit();
        mVAStreakWinNum.Value = 0;
        mVACountPlayerNum.Value = 100;
    }

    public void AddVAStreakWin()
    {
        ++mVAStreakWinNum.Value;
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
