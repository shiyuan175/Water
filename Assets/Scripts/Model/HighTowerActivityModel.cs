using System.Collections.Generic;
using System.Linq;
using QFramework;

public class HighTowerActivityModel : AbstractModel ,ICanGetModel
{
    private const string HTA_STREAK_WIN_NUM_SIGN = "C_HTAStreakWinNum";
    /// <summary>
    /// ��ʤĿ��(�����ȡ����,0ռλ)
    /// </summary>
    private readonly int[] REWARD_STAGES = { 0, 2, 5, 8, 11, 17, 22, 28, 33, 42, 60 };

    public IReadOnlyList<int> RewardStages => REWARD_STAGES;
    public int HTAStreakWinNum => mHTAStreakWin.Value;
    /// <summary>
    ///��һ�׶ν�������(��1��ʼ)
    /// </summary>
    public int NextRewardStageIndex => mNextRewardStageIndex;

    /* obsolete parameter
     /// <summary>
     /// ������һ�׶ν������м�����ʤ
     /// </summary>
     public int WinRemainingToNextReward => RewardStages[mNextRewardStageIndex] - mHTAStreakWin.Value;
     /// <summary>
     /// ��ǰ�׶κ���һ�׶εļ��ֵ
     /// </summary>
     public int CurrentRewardStageGap =>
      mNextRewardStageIndex > 0 && mNextRewardStageIndex < RewardStages.Count
          ? RewardStages[mNextRewardStageIndex] - RewardStages[mNextRewardStageIndex - 1]
          : 0;
    */

    private BindableProperty<int> mHTAStreakWin;
    private int mNextRewardStageIndex;
    private GameGlobalModel mStagemodel;
    private SaveDataUtility storage;

    protected override void OnInit()
    {
        mStagemodel = this.GetModel<GameGlobalModel>();
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
        mHTAStreakWin.Value += mStagemodel.SettlementMultiple;
        RecalculateNextRewardStageIndex(mNextRewardStageIndex); 
    }

    public void HTAStreakLose()
    {
        //����һ����λ
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
