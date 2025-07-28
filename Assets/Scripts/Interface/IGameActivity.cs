using System;

public enum GameActivityStatus
{
    Locked,
    Active,
    CoolingDown,
    WaitStart,
}

public interface IGameActivity 
{
    string ActivityID { get; }
    GameActivityStatus ActivityStatus { get; }

    /// <summary>
    /// 连胜
    /// </summary>
    void StreakWin();

    /// <summary>
    /// 失败
    /// </summary>
    void Fail();

    /// <summary>
    /// 活动冷却
    /// </summary>
    void CoolDownActivity();

    /// <summary>
    /// 重启活动
    /// </summary>
    void RestartActivity();

    void StartActivity()
    {
        
    }

    /// <summary>
    /// 动态重启活动
    /// </summary>
    void Tick();
}
