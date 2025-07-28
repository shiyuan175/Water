using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using GameDefine;

public class StageModel : AbstractModel
{
    private const string ITEM_SIGN = "g_WaterSceneItem";
    private const string COUNTINUE_WIN_NUM_SIGN = "g_WaterCountinueWinNum";
    private const string VOLUME_SETTING_SIGN = "g_WaterVolumeSetting";
    private const int DOUBLE_COIN = 2;

    private SaveDataUtility storage;

    //道具字典
    public BindableDictionary<int, int> ItemDic;

    //连胜
    public int CountinueWinNum => mCountinueWinNum.Value;
    private BindableProperty<int> mCountinueWinNum;

    //金币倍率
    public float GoldCoinsMultiple
    {
        get
        {
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameConst.DOUBLE_COIN_SIGN))
                return DOUBLE_COIN * mGoldCoinsMultiple;

            else return mGoldCoinsMultiple;
        }
    }
    private float mGoldCoinsMultiple = 1;

    //静音
    public bool VolumeSetting
    {
        get => storage.LoadBoolValue(VOLUME_SETTING_SIGN, true);
        set => storage.SaveBool(VOLUME_SETTING_SIGN, value);
    }


    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();

        ItemDic = new BindableDictionary<int, int>();
        mCountinueWinNum = new BindableProperty<int>();

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

    public void AddCountinueWinNum()
    {
        if (storage.GetCurrentLevel() >= GameConst.WIN_STREAK_BEGIN_LEVEL)
            mCountinueWinNum.Value++;
        
        //大于10连胜生效(不含10连胜/本次过关不生效)
        mGoldCoinsMultiple = mCountinueWinNum.Value > GameConst.CONTINUE_WIN_NUM_COIN ? 1.5f : 1;
    }

    public void ResetCountinueWinNum()
    {
        mCountinueWinNum.Value = 0;
        mGoldCoinsMultiple = 1;
    }
}
