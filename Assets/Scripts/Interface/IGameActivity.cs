public enum GameActivityStatus
{
    Locked,
    Active,
    CoolingDown
}

public interface IGameActivity
{
    string ActivityID { get; }
    GameActivityStatus ActivityStatus { get; }

    void StartActivity();
    void FailActivity();
    void RestartActivity();
    void StreakWin();

    void Tick();
}
