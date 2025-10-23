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
    private readonly string BP_REWARD_LEVEl = "H_PGRewardLevel";
    public PGData mPGData;
    public int TempLevel;
    private string mDelFilePath;
    private string mCurFilePath;
    private BindableProperty<int> mRewardLevel;
    private JsonFileUtility mJsonFileUtility;
   
    private SaveDataUtility mStorage;
    protected override void OnInit()
    {
        mStorage = this.GetUtility<SaveDataUtility>();
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        mRewardLevel = new BindableProperty<int>();

        mDelFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.PGDefaultJson.FileName);
        mCurFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.PGCurrentJson.FileName);

        mRewardLevel.SetValueWithoutEvent(mStorage.LoadIntValue(BP_REWARD_LEVEl,0));
        mRewardLevel.Register(value =>
        { 
            mStorage.SaveInt(BP_REWARD_LEVEl, value);
        });
      

    }

    public void LoadPGActivity()
    { 
        mPGData = null;
        if (!File.Exists(mCurFilePath))
            ReloadPGActivity();

        // ²âÊÔ£¬·¢²¼ÐèÉ¾³ý
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
            if (!File.Exists(mCurFilePath))
                using (File.Create(mCurFilePath)) { }
            mJsonFileUtility.SaveToJson(mCurFilePath, tempData);
        });
    }

    public void AddRewardLevel()
    {
        mRewardLevel.Value++;
        TempLevel++;
    }
}
