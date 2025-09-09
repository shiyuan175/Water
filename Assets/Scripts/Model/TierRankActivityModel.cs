using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class TierRankActivityModel : AbstractModel
{
    public int StreakWinNum => mStreakWinNum.Value;


    private const string TRA_STREAK_WIN_NUM = "E_TRAStreakWinNum";
    private const string HISTORY_BEST_RANK = "E_TRAHistoryBestRank";

    private SaveDataUtility mSaveDataUtility;

    private BindableProperty<int> mStreakWinNum;
    //历史最高段位
    private BindableProperty<int> mHistoryBestRank;

    protected override void OnInit()
    {
        mSaveDataUtility = this.GetUtility<SaveDataUtility>();
        mStreakWinNum = new BindableProperty<int>();
        mHistoryBestRank = new BindableProperty<int>();

        mStreakWinNum.SetValueWithoutEvent(mSaveDataUtility.LoadIntValue(TRA_STREAK_WIN_NUM));
        mStreakWinNum.Register(value =>
        {
            mSaveDataUtility.SaveInt(TRA_STREAK_WIN_NUM, value);
        });

        mHistoryBestRank.SetValueWithoutEvent(mSaveDataUtility.LoadIntValue(HISTORY_BEST_RANK));
        mHistoryBestRank.Register(value =>
        {
            mSaveDataUtility.SaveInt(HISTORY_BEST_RANK, value);
        });
    }

    public void StreakWin()
    {
        if (mSaveDataUtility.GetCurrentLevel() >= GameDefine.GameConst.WIN_STREAK_BEGIN_LEVEL)
            ++mStreakWinNum.Value;
    }

    public void ResetStreakWinNum()
    {
        mStreakWinNum.Value = 0;
    }

    public bool CompareWithHistoryBestRank(int rank)
    {
        if (rank > mHistoryBestRank.Value)
        {
            ++mHistoryBestRank.Value;
            return true;
        }

        return false;
    }
}
