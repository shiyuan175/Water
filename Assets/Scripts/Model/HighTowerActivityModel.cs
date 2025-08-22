using System.Collections.Generic;
using System.Linq;
using QFramework;

public class HighTowerActivityModel : AbstractModel
{
    private const string HTA_STREAK_WIN_NUM_SIGN = "C_HTAStreakWinNum";
    /// <summary>
    /// 连胜目标(到达获取奖励,0占位)
    /// </summary>
    private readonly int[] REWARD_STAGES = { 0, 2, 5, 8, 11, 17, 22, 28, 33, 42, 60 };

    public IReadOnlyList<int> RewardStages => REWARD_STAGES;
    public int HTAStreakWinNum => mHTAStreakWin.Value;
    public int NextRewardStageIndex => mNextRewardStageIndex;
    /// <summary>
    /// 距离下一阶段奖励还有几次连胜
    /// </summary>
    public int WinRemainingToNextReward => RewardStages[mNextRewardStageIndex] - mHTAStreakWin.Value;
    /// <summary>
    /// 当前阶段和下一阶段的间隔值
    /// </summary>
    public int CurrentRewardStageGap =>
     mNextRewardStageIndex > 0 && mNextRewardStageIndex < RewardStages.Count
         ? RewardStages[mNextRewardStageIndex] - RewardStages[mNextRewardStageIndex - 1]
         : 0;

    private BindableProperty<int> mHTAStreakWin;
    //最小值会取到1
    private int mNextRewardStageIndex;
    private SaveDataUtility storage;

    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();

        mHTAStreakWin = new BindableProperty<int>();
        mHTAStreakWin.SetValueWithoutEvent(storage.LoadIntValue(HTA_STREAK_WIN_NUM_SIGN, 0));
        mHTAStreakWin.Register(value =>
        {
            storage.SaveInt(HTA_STREAK_WIN_NUM_SIGN, value);
        });

        RecalculateNextRewardStageIndex();
    }

    public void ReloadHighTowerActivity()
    {
        mHTAStreakWin.Value = 0;
        RecalculateNextRewardStageIndex();
    }

    public void HTAStreakWin()
    {
        ++mHTAStreakWin.Value;
        RecalculateNextRewardStageIndex(mNextRewardStageIndex); 
    }

    public void HTAStreakLose()
    {
        //掉落一个档位
        mHTAStreakWin.Value = mNextRewardStageIndex > 0 ?
            REWARD_STAGES[mNextRewardStageIndex - 1] : REWARD_STAGES[0];
    }

    private void RecalculateNextRewardStageIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < REWARD_STAGES.Length; i++)
        {
            if (mHTAStreakWin.Value < REWARD_STAGES[i])
            {
                mNextRewardStageIndex = i;
                return;
            }
        }
    }
}
