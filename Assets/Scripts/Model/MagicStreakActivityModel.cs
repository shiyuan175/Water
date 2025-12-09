using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFileData;
using Newtonsoft.Json;
using QFramework;
using UnityEngine;

public class MagicStreakActivityModel : AbstractModel ,ICanGetUtility ,ICanGetModel
{
    public int StreakWinNum => mStreakWinNum.Value;
    public int CurStageReward => mCurStageReward.Value;
    public bool IsRewardSettled => mIsRewardSettled.Value;
    public MSActivityData MSAData => mMSAData;

    private int WinStreakPoints => mStreakWinNum.Value switch
    {
        >= 
        5 => 100,
        4 => 25,
        3 => 10,
        2 => 5,
        1 => 1,
        _ => 0
    };
    private readonly string MS_STREAK_WIN_NUM_SIGN = "D_MSAStreakWinNum";
    private readonly string MS_IS_REWARD_CLAIMED = "D_MSAIsRewardSettled";
    private readonly string MS_CURRENT_STAGE_REWARD_INDEX = "D_MSACurStageReward";
   
    private string mDelFilePath;
    private string mCurFilePath;
    private JsonFileUtility mJsonFileUtility;
    private MSActivityData mMSAData;
    private SaveDataUtility mStorage;
    private StageModel mStageModel;
    private BindableProperty<int> mStreakWinNum;
    private BindableProperty<int> mCurStageReward;
    private BindableProperty<bool> mIsRewardSettled;
    
    protected override void OnInit()
    {
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        mStorage = this.GetUtility<SaveDataUtility>();
        mStageModel = this.GetModel<StageModel>();
        mDelFilePath = Path.Combine(Application.streamingAssetsPath, GameDefine.GameConst.MSA_DEFAULT_JSON);
        mCurFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.MSA_CURRENT_JSON);

        mStreakWinNum = new BindableProperty<int>();
        mCurStageReward = new BindableProperty<int>();
        mIsRewardSettled = new BindableProperty<bool>();

        mStreakWinNum.SetValueWithoutEvent(mStorage.LoadIntValue(MS_STREAK_WIN_NUM_SIGN));
        mStreakWinNum.Register(value =>
        {
            mStorage.SaveInt(MS_STREAK_WIN_NUM_SIGN, value);
        });

        mCurStageReward.SetValueWithoutEvent(mStorage.LoadIntValue(MS_CURRENT_STAGE_REWARD_INDEX));
        mCurStageReward.Register(value => 
        {
            mStorage.SaveInt(MS_CURRENT_STAGE_REWARD_INDEX, value);
        });

        mIsRewardSettled.SetValueWithoutEvent(mStorage.LoadBoolValue(MS_IS_REWARD_CLAIMED, false));
        mIsRewardSettled.Register(value =>
        {
            mStorage.SaveBool(MS_IS_REWARD_CLAIMED, value);
        });
    }
   
    public void LoadMagicStreakActivity()
    {
        mMSAData = null;

        if (!File.Exists(mCurFilePath))
            ReloadMagicStreakActivity();

        //版本对比代码写在这...
        //var localV = this.GetUtility<JsonFileUtility>().GetFileVersion(mCurFilePath);
        //var dev = this.GetUtility<JsonFileUtility>().GetFileVersion(mDelFilePath);
        //if (localV < dev)
        //{
        //    //版本差异,新字段补充更新...
        //}

        //数据持有
        mJsonFileUtility.LoadFromJson(mCurFilePath, jsonData =>
        {
            mMSAData = JsonConvert.DeserializeObject<MSActivityData>(jsonData);
            //Debug.Log(jsonData);
        });
    }

    public void ReloadMagicStreakActivity()
    {
        mStreakWinNum.Value = 0;
        mCurStageReward.Value = 0;
        mIsRewardSettled.Value = false;
        mJsonFileUtility.LoadFromJson(mDelFilePath, jsonData =>
        {
            MSActivityData tempData = JsonConvert.DeserializeObject<MSActivityData>(jsonData);
            foreach (MSARobotsData item in tempData.MSARobots)
            {
                item.Score = Random.Range(item.MinInitScore, item.MaxInitScore);
            }
            tempData.MSARobots.Sort((a, b) => b.Score.CompareTo(a.Score));
            mJsonFileUtility.SaveToJson(mCurFilePath, tempData);
        });
    }

    public void StreakWin()
    {
        ++mStreakWinNum.Value;
        mMSAData.Player.Score += WinStreakPoints * mStageModel.SettlementMultiple;

        //机器人分数增加规则
        //1     100*H，（H为0-2之间的随机数）
        //2-10  1*A+5*B+10*C+25*D+100*E，（A,B,C,D,E为0-1之间的随机数）
        //11-30 1*A+5*B+10*C             （A,B,C为0-2之间的随机数）
        //31-50 1*A+5*B                  （A,B为0-5之间的随机数）
        foreach (var item in mMSAData.MSARobots)
        {
            if (item.Score < item.LimitScore)
            {
                int _id = item.ID;
                switch (_id)
                {
                    case 1:
                        item.Score += (100 * Random.Range(0, 3));
                        break;
                    case >= 2 and <= 10:
                        item.Score += (1 * Random.Range(0, 2) + 5 * Random.Range(0, 2) + 10 * Random.Range(0, 2) +
                                    25 * Random.Range(0, 2) + 100 * Random.Range(0, 2));
                        break;
                    case >= 11 and <= 30:
                        item.Score += (1 * Random.Range(0, 3) + 5 * Random.Range(0, 3) + 10 * Random.Range(0, 3));
                        break;
                    case >= 31 and <= 50:
                        item.Score += (1 * Random.Range(0, 6) + 5 * Random.Range(0, 6));
                        break;
                }
            }
        }

        mMSAData.MSARobots.Sort((a, b) => b.Score.CompareTo(a.Score));
        mJsonFileUtility.SaveToJson(mCurFilePath, mMSAData);
    }

    public void Fail()
    {
        mStreakWinNum.Value = 0;
    }

    public void MarkNextStageRewardIdnex()
    {
        ++mCurStageReward.Value;
    }

    public void MarkRewardAsSettled()
    {
        mIsRewardSettled.Value = true;
    }
}
