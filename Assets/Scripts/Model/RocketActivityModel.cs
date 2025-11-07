using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class RocketActivityModel : AbstractModel
{
    private const int RA_PLAYER_PROGRESS_SHIFT = 6;
    private const int RA_ROBOT1_PROGRESS_SHIFT = 3;
    private const int RA_PROGRESS_MASK = 0b111;
    private const int RA_MAX_STREAK_WIN_NUM = 7;
    private const string RA_STREAK_WIN_NUM_SIGN = "B_RocketActivityStreakWinNum";
    private const string DAILY_REFRESH_COUNT = "B_RocketActivityDailyRefreshCount";

    public int RAMAxStreakWinNum => RA_MAX_STREAK_WIN_NUM;
    public int PlayerStreakWin => (mRAStreakWinNum.Value >> RA_PLAYER_PROGRESS_SHIFT) & RA_PROGRESS_MASK;
    public int Robot1StreakWin => (mRAStreakWinNum.Value >> RA_ROBOT1_PROGRESS_SHIFT) & RA_PROGRESS_MASK;
    public int Robot2StreakWin => mRAStreakWinNum.Value & RA_PROGRESS_MASK;
    public int DailyUsedRefreshCount => mDailyUsedRefreshCount.Value;
    public int RA_Max_StreakWin_Num => RA_MAX_STREAK_WIN_NUM;

    public bool PlayerWin => PlayerStreakWin >= RA_MAX_STREAK_WIN_NUM;
    public bool RobotWin => Robot1StreakWin >= RA_MAX_STREAK_WIN_NUM || Robot2StreakWin >= RA_MAX_STREAK_WIN_NUM;

    //二进制数据(高三位玩家进度，中间三位机器人1进度，低3位机器人2进度)
    private BindableProperty<int> mRAStreakWinNum;
    private BindableProperty<int> mDailyUsedRefreshCount;
    private SaveDataUtility storage;

    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        mDailyUsedRefreshCount = new BindableProperty<int>();
        mRAStreakWinNum = new BindableProperty<int>();

        mRAStreakWinNum.SetValueWithoutEvent(storage.LoadIntValue(RA_STREAK_WIN_NUM_SIGN, 0));
        mRAStreakWinNum.Register(value =>
        {
            storage.SaveInt(RA_STREAK_WIN_NUM_SIGN, value);
        });

        mDailyUsedRefreshCount.SetValueWithoutEvent(storage.LoadIntValue(DAILY_REFRESH_COUNT, 1));
        mDailyUsedRefreshCount.Register(value =>
        {
            storage.SaveInt(DAILY_REFRESH_COUNT, value);
        });
    }

    /// <summary>
    /// 活动数据重置
    /// </summary>
    public void RefreshRocketActivityData()
    {
        ++mDailyUsedRefreshCount.Value;
        mRAStreakWinNum.Value = 0;
    }

    /// <summary>
    /// 连胜数据处理
    /// </summary>
    public void RAStreakWin()
    {
        int _player = PlayerStreakWin + 1;
        // 限制最大值(每个进度最多三位)，防止超位造成高位数据丢失
        _player = _player > RA_MAX_STREAK_WIN_NUM ? RA_MAX_STREAK_WIN_NUM : _player;
        //按位存储
        mRAStreakWinNum.Value = (_player << RA_PLAYER_PROGRESS_SHIFT) | (SetRobot1StreakWin() << RA_ROBOT1_PROGRESS_SHIFT) | SetRobot2StreakWin();
    }

    /// <summary>
    /// 失败数据处理
    /// </summary>
    public void RAFail()
    {
        //玩家进度清空
        mRAStreakWinNum.Value = 0 | (Robot1StreakWin << RA_ROBOT1_PROGRESS_SHIFT) | Robot2StreakWin;
    }

    public void ResetDailyRefreshCount()
    {
        mDailyUsedRefreshCount.Value = 1;
    }

    private int SetRobot1StreakWin()
    {
        //机器人一(玩家胜率≈80%)
        //20 % 通过一关   
        //15 % 通过两关   
        //5 % 通过三关    
        //int _robot1 = Robot1StreakWin;
        //float _rand = Random.value;
        //if (_rand < 0.05f) 
        //    _robot1 = Mathf.Min(_robot1 + 3, RA_MAX_STREAK_WIN_NUM);
        //else if (_rand < 0.20f)
        //    _robot1 = Mathf.Min(_robot1 + 2, RA_MAX_STREAK_WIN_NUM);
        //else if (_rand < 0.40f) 
        //    _robot1 = Mathf.Min(_robot1 + 1, RA_MAX_STREAK_WIN_NUM);
        //return _robot1;

        // 机器人一(玩家胜率≈70%)
        //22 % 通过一关      
        //15 % 通过两关     
        //5 % 通过三关
        int _robot1 = Robot1StreakWin;
        float _rand = Random.value;
        if (_rand < 0.05f)
            _robot1 = Mathf.Min(_robot1 + 3, RA_MAX_STREAK_WIN_NUM);
        else if (_rand < 0.20f)
            _robot1 = Mathf.Min(_robot1 + 2, RA_MAX_STREAK_WIN_NUM);
        else if (_rand < 0.42f)
            _robot1 = Mathf.Min(_robot1 + 1, RA_MAX_STREAK_WIN_NUM);
        return _robot1;

    }

    private int SetRobot2StreakWin()
    {
        ////机器人二(玩家胜率≈80%)
        ////30% 通过一关
        ////8% 通过两关
        ////2% 通过四关
        //int _robot2 = Robot2StreakWin;
        //float _rand = Random.value;
        //if (_rand < 0.02f)
        //    _robot2 = Mathf.Min(_robot2 + 4, RA_MAX_STREAK_WIN_NUM);
        //else if (_rand < 0.1f)
        //    _robot2 = Mathf.Min(_robot2 + 2, RA_MAX_STREAK_WIN_NUM);
        //else if (_rand < 0.40f)
        //    _robot2 = Mathf.Min(_robot2 + 1, RA_MAX_STREAK_WIN_NUM);
        //return _robot2;

        //机器人二
        //30% 通过一关
        //10% 通过两关
        //5.5% 通过四关
        int _robot2 = Robot2StreakWin;
        float _rand = Random.value;
        if (_rand < 0.055f)
            _robot2 = Mathf.Min(_robot2 + 4, RA_MAX_STREAK_WIN_NUM);
        else if (_rand < 0.155f)
            _robot2 = Mathf.Min(_robot2 + 2, RA_MAX_STREAK_WIN_NUM);
        else if (_rand < 0.455f)
            _robot2 = Mathf.Min(_robot2 + 1, RA_MAX_STREAK_WIN_NUM);
        return _robot2;
    }
}
