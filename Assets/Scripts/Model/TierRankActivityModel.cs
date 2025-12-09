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
    public TRActivityData TRAData => mTRAData;

    private string mDelFilePath;
    private string mCurFilePath;
    private JsonFileUtility mJsonFileUtility;
    private TRActivityData mTRAData;

    protected override void OnInit()
    {
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        mDelFilePath = Path.Combine(Application.streamingAssetsPath, GameDefine.GameConst.TRA_DEFAULT_JSON);
        mCurFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.TRA_CURRENT_JSON);
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
                _tempBool = Random.Range(0f, 1f) < 0.8f;
            else if (mTRAData.TRARobots[i].ID == 2)
                _tempBool = Random.Range(0f, 1f) < 0.5f;
           
            if (_tempBool)
                ++mTRAData.TRARobots[i].StreamWinNum;
        }

        //降序
        mTRAData.TRARobots.Sort((a, b) => b.StreamWinNum.CompareTo(a.StreamWinNum));
        if (TRAData.Player.StreamWinNum > TRAData.TRARobots[0].StreamWinNum 
            && !CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.TRA_HALF_ONE_HOUR_RANK))
            mTRAData.Player.IsRewardSettled = false;

        SaveJson();
    }

    public void Fail()
    {
        mTRAData.Player.StreamWinNum = 0;
        mTRAData.Player.IsRewardSettled = true;
        SaveJson();
    }

    public void MarkRewardAsSettled()
    {
        mTRAData.Player.IsRewardSettled = true;
        SaveJson();
    }

    public void SaveJson()
    {
        mJsonFileUtility.SaveToJson(mCurFilePath, mTRAData);
    }
}
