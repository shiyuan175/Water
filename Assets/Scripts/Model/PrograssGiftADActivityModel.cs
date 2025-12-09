using JsonFileData;
using Newtonsoft.Json;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PrograssGiftADActivityModel : AbstractModel
{
    public int RewardLevel => mRewardLevel.Value;
    public int GiftLevel => mGiftLevel.Value;
    private readonly string PG_REWARD_LEVEl = "H_PGRewardLevel";
    private readonly string PG_GIFT_LEVEl = "H_PGGiftLevel";
    public PGData mPGData;
    public int TempLevel;
    private string mDelFilePath;
    private string mCurFilePath;
    private BindableProperty<int> mRewardLevel;
    private BindableProperty<int> mGiftLevel;
    private JsonFileUtility mJsonFileUtility;
   
    private SaveDataUtility mStorage;
    protected override void OnInit()
    {
        mStorage = this.GetUtility<SaveDataUtility>();
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        mRewardLevel = new BindableProperty<int>();
        mGiftLevel = new BindableProperty<int>();

        mDelFilePath = Path.Combine(Application.streamingAssetsPath, GameDefine.GameConst.PGDefaultJson);
        mCurFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.PGCurrentJson);

        mRewardLevel.SetValueWithoutEvent(mStorage.LoadIntValue(PG_REWARD_LEVEl,0));
        mRewardLevel.Register(value =>
        { 
            mStorage.SaveInt(PG_REWARD_LEVEl, value);
        });
        mRewardLevel.SetValueWithoutEvent(mStorage.LoadIntValue(PG_GIFT_LEVEl, 0));
        mRewardLevel.Register(value =>
        {
            mStorage.SaveInt(PG_GIFT_LEVEl, value);
        });

    }

    public void LoadPGActivity()
    { 
        mPGData = null;
        if (!File.Exists(mCurFilePath))
            ReloadPGActivity();

        mJsonFileUtility.LoadFromJson(mCurFilePath, jsonData =>
        {
            mPGData = JsonConvert.DeserializeObject<PGData>(jsonData);
  
        });
    }

    public void ReloadPGActivity()
    {
        mRewardLevel.Value = 0;
        mJsonFileUtility.LoadFromJson(mDelFilePath, jsonData =>
        {
            PGData tempData = JsonConvert.DeserializeObject<PGData>(jsonData);
            mJsonFileUtility.SaveToJson(mCurFilePath, tempData);
        });
    }

    public void AddRewardLevel()
    {
        mRewardLevel.Value++;
        TempLevel++;
    }
    public void AddGiftLevel()
    {
        mGiftLevel.Value++;
    }

}
