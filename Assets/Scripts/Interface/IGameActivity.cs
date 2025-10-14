using System;

//奖励触发式活动状态
public enum GameActivityStatus
{
    Locked,
    Inactive,
    Active,
    CoolingDown,
    WaitStart,
    None
}

//奖励结算式活动状态
public enum SettlementActivityStatus
{
    Locked,
    Inactive,
    Active,
    Finished,
    //CoolingDown,
    WaitStart,
    None,
}

public interface IGameActivity 
{
    string ActivityID { get; }

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

    void StartActivity();

    string GetActivityReamingTime();

    void Tick();
}
