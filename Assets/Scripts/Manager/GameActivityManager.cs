using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using QFramework;
using GameDefine;

public class GameActivityManager : MonoSingleton<GameActivityManager>, ICanGetModel, ICanGetUtility
{
    public Dictionary<string, IGameActivity> GameActivity;

    private SaveDataUtility mSaveUtility;
    private CancellationTokenSource mCts;

    protected override void OnDestroy()
    {
        StopTickLoop();
        base.OnDestroy();
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public override void OnSingletonInit()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        GameActivity = new Dictionary<string, IGameActivity>();
        StartTickLoop();

        //根据关卡进度注册活动
        int _curLevel = mSaveUtility.GetCurrentLevel();
        if (_curLevel >= GameDefine.GameConst.VA_BEGIN_LEVEL)
        {
            RegisterActivity<VolcanicActivity>();
        }
        //(段位活动暂关)
        //if (_curLevel >= GameDefine.GameConst.WIN_STREAK_BEGIN_LEVEL)
        //{
        //    RegisterActivity<TierRankActivity>();
        //}
        if (_curLevel >= GameDefine.GameConst.RA_BEGIN_LEVEL)
        {
            RegisterActivity<RocketActivity>();
        }
        if (_curLevel >= GameDefine.GameConst.HTA_BEGIN_LEVEL)
        {
            RegisterActivity<HighTowerActivity>();
        }
        if (_curLevel >= GameDefine.GameConst.TT_AD_BEGIN_LEVEL)
        {
            RegisterActivity<TurnTableADActivity>();
        }
        if (_curLevel >= GameDefine.GameConst.DR_AD_BEGIN_LEVEL)
        {
            RegisterActivity<DailyRewardADActivity>();
            if (!GameUtils.DoesCountDownKeyExist(GameDefine.GameConst.DAILYREWARD_AD_ACTIVITY_SIGN))
            {
                GetActivity<DailyRewardADActivity>().StartActivity();

            }       
        }
        if (_curLevel >= GameDefine.GameConst.BP_AD_BEGIN_LEVEL)
        {
            RegisterActivity<BattlePassADActivity>();
        }
        if (_curLevel >= GameDefine.GameConst.MS_BEGIN_LEVEL)
        {
            RegisterActivity<MagicStreakActivity>();
        }
        //Other Activities can be registered here based on level or other conditions
    }

    //活动注册
    public void RegisterActivity<T>() where T : IGameActivity, new()
    {
        var _id = typeof(T).Name;
        if (GameActivity.ContainsKey(_id)) return;
        var _activity = new T();
        GameActivity.Add(_id, _activity);
    }

    public T GetActivity<T>() where T : IGameActivity
    {
        var _id = typeof(T).Name;
        if (GameActivity.TryGetValue(_id, out var _activity))
        {
            return (T)_activity;
        }
        return default;
    }

    private void StartTickLoop()
    {
        mCts = new CancellationTokenSource();
        _ = TickLoopAsync(mCts.Token);
    }

    private void StopTickLoop()
    {
        mCts?.Cancel();
    }

    private async Task TickLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var activity in GameActivity.Values)
            {
                activity.Tick();
            }

            try
            {
                await Task.Delay(1000, token); 
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
