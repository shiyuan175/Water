using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using GameDefine;

public class StageModel : AbstractModel
{
    private const string REMAINING_STARS = "A_RemainingStars";
    private const string ITEM_SIGN = "A_GameItem";
    private const string COUNTINUE_WIN_NUM_SIGN = "g_WaterCountinueWinNum";
    private const string VOLUME_SETTING_SIGN = "g_WaterVolumeSetting";
    private const int DOUBLE = 2;

    private SaveDataUtility storage;

    //道具字典
    public BindableDictionary<int, int> ItemDic;
    //当前星星数
    private BindableProperty<int> mRemainingStars;
    //连胜
    private BindableProperty<int> mCountinueWinNum;
    private float mGoldCoinsMultiple => mCountinueWinNum.Value > GameConst.CONTINUE_WIN_NUM_COIN ? 1.5f : 1;

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
    public int SettlementMultiple => CountDownTimerManager.Instance.IsTimerFinished(GameConst.DOUBLE_SETTLEMENT_SIGN) ? 1 : DOUBLE;
    //静音
    public bool VolumeSetting
    {
        get => storage.LoadBoolValue(VOLUME_SETTING_SIGN, true);
        set => storage.SaveBool(VOLUME_SETTING_SIGN, value);
    }
    public int RemainingStars => mRemainingStars.Value;
    public int CountinueWinNum => mCountinueWinNum.Value;

    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();

        ItemDic = new BindableDictionary<int, int>();
        mCountinueWinNum = new BindableProperty<int>();
        mRemainingStars = new BindableProperty<int>();

        //若无存档则以(当前关卡 - 1)初始化星星数，以兼容旧版本逻辑
        mRemainingStars.SetValueWithoutEvent(storage.LoadIntValue(REMAINING_STARS, storage.GetCurrentLevel() - 1));
        mRemainingStars.Register(value =>
        {
            storage.SaveInt(REMAINING_STARS, value);
        });

        for (int i = 1; i <= GameDefine.GameConst.ITEM_COUNT; i++)
        {
            var key = $"{ITEM_SIGN}{i}";
            ItemDic[i] = storage.LoadIntValue(key, 4);
        }
        ItemDic.OnReplace.Register((itemID, oldValue, newValue) =>
        {
            storage.SaveInt($"{ITEM_SIGN}{itemID}", newValue);
            this.SendEvent<RefreshItemEvent>();
            //Debug.Log($"道具ID：{itemID} 数量更新为:{newValue},发送事件通知...");
        });

        mCountinueWinNum.SetValueWithoutEvent(storage.LoadIntValue(COUNTINUE_WIN_NUM_SIGN));
        mCountinueWinNum.Register(value =>
        {
            storage.SaveInt(COUNTINUE_WIN_NUM_SIGN, value);

        });
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

        if (storage.GetCurrentLevel() >= GameConst.WIN_STREAK_BEGIN_LEVEL)
            mCountinueWinNum.Value++;
    }

    public void ResetCountinueWinNum()
    {
        mCountinueWinNum.Value = 0;
    }
}
