using JsonFileData;
using Newtonsoft.Json;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BattlePassModel : AbstractModel, ICanGetUtility
{
    public BattlePassData BPDate => mBPDate;
    public int GameWinNum => mGameWinNum.Value;
    public int CurrentGetConditions => mCurrentGetConditions;
    public int FreeRewardGotLevel => mFreeRewardGotLevel.Value;
    public int VipRewardGorLevel => mVipRewardGotLevel.Value;
    /// <summary>
    /// 标记当前奖励所到的等级从1 开始，数据的奖励是从0开始
    /// </summary>
    public int RewardLevel => mRewardLevel.Value;
    public bool IsVip => mIsVip.Value;
    private readonly string BP_GAMEWIN_NUM = "H_BPGameWinNum";
    private readonly string BP_IS_VIP = "H_BPIsVip";
    private readonly string BP_REWARD_LEVEL = "H_RewardLevel";
    private readonly string BP_FREEREWARDGOT_LEVEL = "H_FreeRewardGotLevel";
    private readonly string BP_VIPREWARDGOT_LEVEL = "H_VipRewardGotLevel";

    private string mDelFilePath;
    private string mCurFilePath;

    private JsonFileUtility mJsonFileUtility;
    private BattlePassData mBPDate;
    private SaveDataUtility mStorage;
    private TwoBitUtility m2BitTool;

    private int mCurrentGetConditions;
    private BindableProperty<int> mGameWinNum;
    private BindableProperty<int> mRewardLevel;
    private BindableProperty<int> mFreeRewardGotLevel;
    private BindableProperty<int> mVipRewardGotLevel;
    private BindableProperty<bool> mIsVip;

    protected override void OnInit()
    {
        mStorage = this.GetUtility<SaveDataUtility>();
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        m2BitTool = this.GetUtility<TwoBitUtility>();
        mGameWinNum = new BindableProperty<int>();
        mRewardLevel = new BindableProperty<int>();
        mFreeRewardGotLevel = new BindableProperty<int>();
        mVipRewardGotLevel = new BindableProperty<int>();
        mIsVip = new BindableProperty<bool>();

        mDelFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.BPDefaultJson.FileName);
        mCurFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.BP_CURRENT_JSON);

        mGameWinNum.SetValueWithoutEvent(mStorage.LoadIntValue(BP_GAMEWIN_NUM, 0));
        mGameWinNum.Register(value =>
        {
            mStorage.SaveInt(BP_GAMEWIN_NUM, value);
        });

        mIsVip.SetValueWithoutEvent(mStorage.LoadBoolValue(BP_IS_VIP, false));
        mIsVip.Register(value =>
        {
            mStorage.SaveBool(BP_IS_VIP, value);
        });
        mRewardLevel.SetValueWithoutEvent(mStorage.LoadIntValue(BP_REWARD_LEVEL, 1));
        mRewardLevel.Register(value =>
        {
            mStorage.SaveInt(BP_REWARD_LEVEL, value);
        });

        mFreeRewardGotLevel.SetValueWithoutEvent(mStorage.LoadIntValue(BP_FREEREWARDGOT_LEVEL, 0));
        mFreeRewardGotLevel.Register(value =>
        {
            mStorage.SaveInt(BP_FREEREWARDGOT_LEVEL, value);
        });

        mVipRewardGotLevel.SetValueWithoutEvent(mStorage.LoadIntValue(BP_VIPREWARDGOT_LEVEL, 0));
        mVipRewardGotLevel.Register(value =>
        {
            mStorage.SaveInt(BP_VIPREWARDGOT_LEVEL, value);
        });

    }
    public void LoadBattlePassActivity()
    {
        mBPDate = null;

        if (!File.Exists(mCurFilePath))
            ReloadBattlePassActivity();

        //数据持有
        mJsonFileUtility.LoadFromJson(mCurFilePath, jsonData =>
        {
            mBPDate = JsonConvert.DeserializeObject<BattlePassData>(jsonData);
        });

        if (RewardLevel < BPDate.Rewards.Length)
            mCurrentGetConditions = BPDate.Rewards[RewardLevel].GetConditions;
        else
            mCurrentGetConditions = GameDefine.GameConst.MAX_INT;
    }

    /// <summary>
    ///  战令更新的时候，直接用默认json代替当前json,同时清空过关数记录
    /// </summary>
    public void ReloadBattlePassActivity()
    {
        // 清除战令高级激活和进度(奖励领取，战令级别)
        mGameWinNum.Value = 0;
        mRewardLevel.Value = 1;
        mVipRewardGotLevel.Value = 0;
        mFreeRewardGotLevel.Value = 0;
        mIsVip.Value = false;
        mJsonFileUtility.LoadFromJson(mDelFilePath, jsonData =>
        {
            BattlePassData tempData = JsonConvert.DeserializeObject<BattlePassData>(jsonData);
            mJsonFileUtility.SaveToJson(mCurFilePath, tempData);
            mBPDate = tempData;
        });
    }
    /// <summary>
    /// 增加计数，如果计数达到下一级的条件时，就增加级别,修改完成条件，清空计数
    /// </summary>
    public void AddGameWinCount()
    {
        mGameWinNum.Value++;
        if (mGameWinNum.Value >= mCurrentGetConditions)
        {
            mGameWinNum.Value = 0;
            mRewardLevel.Value++;
            if (RewardLevel < BPDate.Rewards.Length)
                mCurrentGetConditions = BPDate.Rewards[RewardLevel].GetConditions;
            else
                mCurrentGetConditions = GameDefine.GameConst.MAX_INT;
        }
    }
    /// <summary>
    /// 充值开启
    /// </summary>
    public void HightBattlePassActivation()
    {
        mIsVip.Value = true;
    }
    /// <summary>
    /// 增加奖励的领取进度
    /// </summary>
    /// <param name="isVipPack">领取的奖励类型 真为HightLevel，假为</param>
    public void AddRewardGotLevel(bool isVipPack, int level)
    {
        if (isVipPack)
            mVipRewardGotLevel.Value += m2BitTool.Get2BitValue(level);
        else
            mFreeRewardGotLevel.Value += m2BitTool.Get2BitValue(level);
    }


}
