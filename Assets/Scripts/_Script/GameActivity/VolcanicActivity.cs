using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using QFramework;
using UnityEngine;

public class VolcanicActivity : IGameActivity ,ICanGetModel ,ICanGetUtility
{
    private const string VOLCANIC_ACTIVITY_SIGN = "VolcanicActivity";
    private const string VOLCANIC_ACTIVITY_RESTART_SIGN = "VolcanicActivityRestart";
    private const int VA_MAX_STREAK_WIN_NUM = 7;

    private readonly SaveDataUtility mSaveUtility;
    private readonly VolcanicActivityModel mVolcanicActivityModel;

    public int RewardCoins => mVolcanicActivityModel.GetVARewardCoins;
    public int VAStreakWinNum => mVolcanicActivityModel.VAStreakWinNum;
    public int VACurrentPlayerNum => mVolcanicActivityModel.VACurrentPlayerNum;
    public bool EndWin => VAStreakWinNum >= VA_MAX_STREAK_WIN_NUM;

    public string ActivityID => GetType().Name;

    public GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < GameDefine.GameConst.VA_BEGIN_LEVEL)
            {
                return GameActivityStatus.Locked;
            }

            if (!CountDownTimerManager.Instance.IsTimerFinished(VOLCANIC_ACTIVITY_SIGN))
            {
                return GameActivityStatus.Active;
            }

            else
            {
                return GameActivityStatus.CoolingDown;
            }
        }
    }

    public VolcanicActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        mVolcanicActivityModel = this.GetModel<VolcanicActivityModel>();
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public void StartActivity()
    {
        //¿ªÆô»î¶¯
        CountDownTimerManager.Instance.StartTimer(VOLCANIC_ACTIVITY_SIGN, 1440f);
    }

    public void FailActivity()
    {
        CountDownTimerManager.Instance.DeleteTimer(VOLCANIC_ACTIVITY_SIGN);
        CountDownTimerManager.Instance.ResetTimer(VOLCANIC_ACTIVITY_RESTART_SIGN, 60);
    }

    public void StreakWin()
    {
        mVolcanicActivityModel.AddVAStreakWin();
        if (EndWin)
        {
            CountDownTimerManager.Instance.DeleteTimer(VOLCANIC_ACTIVITY_SIGN);
            CountDownTimerManager.Instance.ResetTimer(VOLCANIC_ACTIVITY_RESTART_SIGN, 60);
        }
    }

    public void RestartActivity()
    {
        mVolcanicActivityModel.ReloadVolcanicActivity();
        CountDownTimerManager.Instance.ResetTimer(VOLCANIC_ACTIVITY_SIGN, 1440);
    }

    public void Tick()
    {
        if (ActivityStatus == GameActivityStatus.CoolingDown)
        {
            if (CountDownTimerManager.Instance.IsTimerFinished(VOLCANIC_ACTIVITY_RESTART_SIGN))
            {
                RestartActivity();
            }
        }
    }

}
