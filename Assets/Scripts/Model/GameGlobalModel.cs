using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using QFramework;
using GameDefine;
using JsonFileData;
using Newtonsoft.Json;

public class GameGlobalModel : AbstractModel
{
    #region 内部字段、常量

    private const string REMAINING_STARS = "A_RemainingStars";
    private const string ITEM_SIGN = "A_GameItem";
    private const string CONTINUE_WIN_NUM_SIGN = "A_WaterContinueWinNum";
    private const string IN_GAME_RANK_STREAK_WIN_SIGN = "A_InGameRankStreakWinNum";
    private const string GOLD_COINS_MULTIPLE_STREAK_WIN_SIGN = "A_GoldCoinsMultipleStreakWinNum";
    private const string REMOVE_HIDE_STREAK_WIN_SIGN = "A_RemoveHideStreakWinNum";
    private const string VOLUME_SETTING_SIGN = "A_WaterVolumeSetting";
    private const string HISTORY_BEST_RANK = "E_HistoryBestRank";
    
    private readonly string mGameGlobalDelJson = Path.Combine(Application.streamingAssetsPath, GameConst.GAME_GLOBAL_DEFAULT_JSON);
    private readonly string mGameGlobalCurJson = Path.Combine(Application.persistentDataPath, GameConst.GAME_GLOBAL_CURRENT_JSON);
    
    private const int DOUBLE = 2;
    
    //当前星星数
    private BindableProperty<int> mRemainingStars;
    //连胜
    private BindableProperty<int> mCountinueWinNum;
    //游戏段位的连胜
    private BindableProperty<int> mInGameRankStreakWin;
    //1.5倍金币的连胜
    private BindableProperty<int> mGoldCoinsMultipleStreakWin;
    //连胜去黑
    private BindableProperty<int> mRemoveHideStreakWin;
    //历史最高段位
    private BindableProperty<int> mHistoryBestRank;
    // 1.5倍结算金币
    private float mGoldCoinsMultiple => mGoldCoinsMultipleStreakWin.Value > GameConst.TEN_CONTINUE_WIN_NUM ? 1.5f : 1;
    
    private SaveDataUtility storage;
    private JsonFileUtility mJsonFileUtility; 
    private GameGlobalData mGameGlobalData;
    
    #endregion

    #region 外部访问
    //道具字典
    public BindableDictionary<int, int> ItemDic;
    //全局Json字段
    public GameGlobalData GameGlobalJsonData => mGameGlobalData;  

    //金币倍率
    public float GoldCoinsMultiple
    {
        get
        {
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameConst.DOUBLE_COIN_SIGN))
                return DOUBLE * mGoldCoinsMultiple;

            else return mGoldCoinsMultiple;
        }
    }
    //双倍结算Buff(部分活动积分/星星获取)
    public int SettlementMultiple => CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.UnlimitedDoubleBuff)) ? 1 : DOUBLE;
    //静音
    public bool VolumeSetting
    {
        get => storage.LoadBoolValue(VOLUME_SETTING_SIGN, true);
        set => storage.SaveBool(VOLUME_SETTING_SIGN, value);
    }
    public int RemainingStars => mRemainingStars.Value;
    public int CountinueWinNum => mCountinueWinNum.Value;
    public int InGameRankStreakWinNum => mInGameRankStreakWin.Value;
    public int GoldCoinsMultipleStreakWinNum => mGoldCoinsMultipleStreakWin.Value;
    public int RemoveHideStreakWinNum => mRemoveHideStreakWin.Value;

    #endregion

    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        
        //加载全局Json文件
        LoadGlobalJson();
        #region BindableProperty

        ItemDic = new BindableDictionary<int, int>();
        mCountinueWinNum = new BindableProperty<int>();
        mRemainingStars = new BindableProperty<int>();
        mHistoryBestRank = new BindableProperty<int>();
        mInGameRankStreakWin = new BindableProperty<int>();
        mGoldCoinsMultipleStreakWin = new BindableProperty<int>();
        mRemoveHideStreakWin = new BindableProperty<int>();

        //若无存档则以(当前关卡 - 1)初始化星星数，以兼容旧版本逻辑
        mRemainingStars.SetValueWithoutEvent(storage.LoadIntValue(REMAINING_STARS, storage.GetCurrentLevel() - 1));
        mRemainingStars.Register(value =>
        {
            storage.SaveInt(REMAINING_STARS, value);
        });

        //NormalRewardsType 为道具ID
        for (int i = 1; i <= GameDefine.GameConst.ITEM_COUNT; i++)
        {
            int del = 3;
            var key = $"{ITEM_SIGN}{i}";
            if (i > 5)
                del = 4;
           ItemDic[i] = storage.LoadIntValue(key, del);
        }
        ItemDic.OnReplace.Register((itemID, oldValue, newValue) =>
        {
            storage.SaveInt($"{ITEM_SIGN}{itemID}", newValue);
            this.SendEvent(new RefreshItemEvent() { itemID = itemID });
            //Debug.Log($"道具ID：{itemID} 数量更新为:{newValue},发送事件通知...");
        });

        mCountinueWinNum.SetValueWithoutEvent(storage.LoadIntValue(CONTINUE_WIN_NUM_SIGN));
        mCountinueWinNum.Register(value =>
        {
            storage.SaveInt(CONTINUE_WIN_NUM_SIGN, value);

        });

        mHistoryBestRank.SetValueWithoutEvent(storage.LoadIntValue(HISTORY_BEST_RANK));
        mHistoryBestRank.Register(value =>
        {
            storage.SaveInt(HISTORY_BEST_RANK, value);
        });

        mInGameRankStreakWin.SetValueWithoutEvent(storage.LoadIntValue(IN_GAME_RANK_STREAK_WIN_SIGN));
        mInGameRankStreakWin.Register(value =>
        {
            storage.SaveInt(IN_GAME_RANK_STREAK_WIN_SIGN, value);
        });

        mGoldCoinsMultipleStreakWin.SetValueWithoutEvent(storage.LoadIntValue(GOLD_COINS_MULTIPLE_STREAK_WIN_SIGN));
        mGoldCoinsMultipleStreakWin.Register(value =>
        {
            storage.SaveInt(GOLD_COINS_MULTIPLE_STREAK_WIN_SIGN, value);
        });

        mRemoveHideStreakWin.SetValueWithoutEvent(storage.LoadIntValue(REMOVE_HIDE_STREAK_WIN_SIGN));
        mRemoveHideStreakWin.Register(value =>
        {
            storage.SaveInt(REMOVE_HIDE_STREAK_WIN_SIGN, value);
        });
        
        #endregion
    }

    public void UsedStar(int value)
    {
        if(mRemainingStars.Value >= value)
            mRemainingStars.Value -= value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="addNum"></param>
    /// 1 回退 2 取消黑色 3 加瓶子 4 加一格瓶子 5 取消所有限制 6 加一格瓶子 7取消两根黑色 8随机颜色</param>
    public void AddItem(int itemID, int addNum)
    {
        if (ItemDic.ContainsKey(itemID))
        {
            ItemDic[itemID] += addNum;
        }
        else
        {
            ItemDic[itemID] = addNum;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemID">Item ID</param>
    /// <param name="reduceNum">Reduce Item Num</param>
    public void ReduceItem(int itemID, int reduceNum)
    {
        if (ItemDic.ContainsKey(itemID))
        {
            ItemDic[itemID] = Mathf.Max(0, ItemDic[itemID] - reduceNum);
        }
    }

    /// <summary>
    /// 过关处理
    /// </summary>
    public void PassLevel()
    {
        mRemainingStars.Value += SettlementMultiple;
        var _curLevel = storage.GetCurrentLevel();

        if (_curLevel >= GameConst.WIN_STREAK_BEGIN_LEVEL)
            ++mCountinueWinNum.Value;

        if (_curLevel >= GameConst.IN_GAME_RANK_BEGIN_LEVEL)
            ++mInGameRankStreakWin.Value;

        if (_curLevel > (int)UnLockMechanism.TimesGoldCoin)
            ++mGoldCoinsMultipleStreakWin.Value;

        if (_curLevel > (int)UnLockMechanism.RemoveHideWinStreakLevel)
            ++mRemoveHideStreakWin.Value;
    }

    public void ResetCountinueWinNum()
    {
        mCountinueWinNum.Value = 0;
        mInGameRankStreakWin.Value = 0;
        mGoldCoinsMultipleStreakWin.Value = 0;
        mRemoveHideStreakWin.Value = 0;
    }

    public bool CompareWithHistoryBestRank(int playRankIdx)
    {
        if (playRankIdx > mHistoryBestRank.Value)
        {
            ++mHistoryBestRank.Value;
            return true;
        }

        return false;
    }

    public void LoadGlobalJson()
    {
        if (mGameGlobalData != null) return;
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        
        if (!File.Exists(mGameGlobalCurJson))
        {
            mJsonFileUtility.LoadFromJson(mGameGlobalDelJson, jsonData =>
            {
                mGameGlobalData = JsonConvert.DeserializeObject<GameGlobalData>(jsonData);
                mJsonFileUtility.SaveToJson(mGameGlobalCurJson, mGameGlobalData);
            });
        }
        else
        {
            //版本对比
            var localV = mJsonFileUtility.GetFileVersion(mGameGlobalCurJson);
            var dev = mJsonFileUtility.GetFileVersion(mGameGlobalDelJson);
            if (localV < dev)
                mJsonFileUtility.AutoFixFields(mGameGlobalCurJson, mGameGlobalDelJson);
            
            mJsonFileUtility.LoadFromJson(mGameGlobalCurJson, jsonData =>
            {
                mGameGlobalData = JsonConvert.DeserializeObject<GameGlobalData>(jsonData);
            });
        }
    }
    
    //增加体力上限
    public void AddMaxHp(int value)
    {
        GameGlobalJsonData.MaxHp += value;
        HealthManager.Instance.RecalculateRecoverEndTime();
        mJsonFileUtility.SaveToJson(mGameGlobalCurJson, GameGlobalJsonData);	
    }

    //减少体力恢复时长
    public void ReduceHpRecoverTimer(int timer)
    {
        //每体力恢复时长最少为60s
        if (GameGlobalJsonData.HpRecoverTimer <= 60) return;

        GameGlobalJsonData.HpRecoverTimer = Mathf.Max(GameGlobalJsonData.HpRecoverTimer - timer, 60);
        HealthManager.Instance.RecalculateRecoverEndTime();
        mJsonFileUtility.SaveToJson(mGameGlobalCurJson, GameGlobalJsonData);
    }
    
    //反射写入字段值
    //示例：SetFieldAndSave<bool>(nameof(GameGlobalJsonData.IsClaim_UnlockScene1Reward), false);
    public void SetFieldAndSave<T>(string fieldName, T value)
    {
        var type = GameGlobalJsonData.GetType();
        var field = type.GetField(fieldName);
        var property = type.GetProperty(fieldName);

        if (field != null)
            field.SetValue(GameGlobalJsonData, value);
        else if (property != null && property.CanWrite)
            property.SetValue(GameGlobalJsonData, value);
        else return;

        mJsonFileUtility.SaveToJson(mGameGlobalCurJson, GameGlobalJsonData);
    }
}
