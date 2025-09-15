using JsonFileData;
using Newtonsoft.Json;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TierRankActivityModel : AbstractModel
{
    public int StreakWinNum => mTRAData.Player.StreamWinNum;
    public bool FirstHourTierRank => mFirstHourTierRank.Value;
    public TRActivityData TRAData => mTRAData;

    //5次连胜晋升一个段位,总段位数9(起始0)
    private const int WIN_STREAK_RANKLEVEL_INTERVAL = 5;
    private const int MAX_RANK_SPRITE_INDEX = 8;
    private const string HISTORY_BEST_RANK = "E_TRAHistoryBestRank";
    private const string FIRST_HOUR_TIER_RANK = "E_FirstHourTierRank";

    private string mDelFilePath;
    private string mCurFilePath;
    private SaveDataUtility mSaveDataUtility;
    private JsonFileUtility mJsonFileUtility;
    private TRActivityData mTRAData;

    private BindableProperty<int> mHistoryBestRank;
    private BindableProperty<bool> mFirstHourTierRank;

    protected override void OnInit()
    {
        mSaveDataUtility = this.GetUtility<SaveDataUtility>();
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        mDelFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.TRADefaultJson.FileName);
        mCurFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.TRACurrentJson.FileName);

        mHistoryBestRank = new BindableProperty<int>();
        mFirstHourTierRank = new BindableProperty<bool>();

        mHistoryBestRank.SetValueWithoutEvent(mSaveDataUtility.LoadIntValue(HISTORY_BEST_RANK));
        mHistoryBestRank.Register(value =>
        {
            mSaveDataUtility.SaveInt(HISTORY_BEST_RANK, value);
        });

        mFirstHourTierRank.SetValueWithoutEvent(mSaveDataUtility.LoadBoolValue(FIRST_HOUR_TIER_RANK));
        mFirstHourTierRank.Register(value =>
        {
            mSaveDataUtility.SaveBool(FIRST_HOUR_TIER_RANK, value);
        });
    }

    public void LoadTRAData()
    {
        mTRAData = null;

        if (!File.Exists(mCurFilePath))
            ReloadTRAData();

        //版本对比
        //...

        //数据持有
        mJsonFileUtility.LoadFromJson(mCurFilePath, jsonData =>
        {
            mTRAData = JsonConvert.DeserializeObject<TRActivityData>(jsonData);
        });
    }

    public void ReloadTRAData()
    {
        mJsonFileUtility.LoadFromJson(mDelFilePath ,jsonData =>
        {
            TRActivityData tempData = JsonConvert.DeserializeObject<TRActivityData>(jsonData);

            if (!File.Exists(mCurFilePath))
                using (File.Create(mCurFilePath)) { }

            tempData.TRARobots.Sort((a, b) => b.StreamWinNum.CompareTo(a.StreamWinNum));
            mJsonFileUtility.SaveToJson(mCurFilePath, tempData);
        });
    }

    public void StreakWin()
    {
        ++mTRAData.Player.StreamWinNum;
        //机器人规则
        //一位80%概率增加连胜，一位50%概率增加连胜，剩余不增加
        for (int i = 0; i < mTRAData.TRARobots.Count; i++)
        {
            var _tempBool = false;

            if (mTRAData.TRARobots[i].ID == 1)
                _tempBool = Random.Range(0, 1) < 0.8f;
            else if (mTRAData.TRARobots[i].ID == 2)
                _tempBool = Random.Range(0, 1) < 0.5f;

            if (_tempBool)
                ++mTRAData.TRARobots[i].StreamWinNum;
        }

        //降序
        mTRAData.TRARobots.Sort((a, b) => b.StreamWinNum.CompareTo(a.StreamWinNum));
        if (TRAData.Player.StreamWinNum > TRAData.TRARobots[0].StreamWinNum)
            mTRAData.Player.IsRewardSettled = false;

        mJsonFileUtility.SaveToJson(mCurFilePath, mTRAData);
    }

    public void Fail()
    {
        mTRAData.Player.StreamWinNum = 0;
        mTRAData.Player.IsRewardSettled = true;
        mJsonFileUtility.SaveToJson(mCurFilePath, mTRAData);
    }

    public int GetTierRankIndex(int streakWin)
    {
        int _rankIndex = Mathf.Max(0, (streakWin - 1) / WIN_STREAK_RANKLEVEL_INTERVAL);
        return _rankIndex >= MAX_RANK_SPRITE_INDEX ? MAX_RANK_SPRITE_INDEX : _rankIndex;
    }

    public bool CompareWithHistoryBestRank()
    {
        if (GetTierRankIndex(StreakWinNum) > mHistoryBestRank.Value)
        {
            ++mHistoryBestRank.Value;
            return true;
        }

        return false;
    }

    public void MarkRewardAsSettled()
    {
        mTRAData.Player.IsRewardSettled = true;
        mJsonFileUtility.SaveToJson(mCurFilePath, mTRAData);
    }

    public void MarkFirstHourTierRank()
    {
        mFirstHourTierRank.Value = false;
    }
}
