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
    /// ��ǵ�ǰ���������ĵȼ���1 ��ʼ�����ݵĽ����Ǵ�0��ʼ
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
        mCurFilePath = Path.Combine(Application.persistentDataPath, GameDefine.GameConst.BPCurrentJson.FileName);

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

        //�汾�Աȴ���д����...
        // �°汾ֱ�Ӵ���ɰ汾����Ϸ���ݵĸ��·ŵ�������¿���


        /*        // ���ԣ�������ɾ��
                ReloadBattlePassActivity();*/
        //���ݳ���
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
    ///  ս����µ�ʱ��ֱ����Ĭ��json���浱ǰjson,ͬʱ��չ�������¼
    /// </summary>
    public void ReloadBattlePassActivity()
    {
        // ���ս��߼�����ͽ���(������ȡ��ս���)
        mGameWinNum.Value = 0;
        mRewardLevel.Value = 1;
        mVipRewardGotLevel.Value = 0;
        mFreeRewardGotLevel.Value = 0;
        mIsVip.Value = false;
        mJsonFileUtility.LoadFromJson(mDelFilePath, jsonData =>
        {
            BattlePassData tempData = JsonConvert.DeserializeObject<BattlePassData>(jsonData);
            if (!File.Exists(mCurFilePath))
                using (File.Create(mCurFilePath)) { }
            mJsonFileUtility.SaveToJson(mCurFilePath, tempData);
            mBPDate = tempData;
        });
    }
    /// <summary>
    /// ���Ӽ�������������ﵽ��һ��������ʱ�������Ӽ���,�޸������������ռ���
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
    /// ��ֵ����
    /// </summary>
    public void HightBattlePassActivation()
    {
        mIsVip.Value = true;
    }
    /// <summary>
    /// ���ӽ�������ȡ����
    /// </summary>
    /// <param name="isVipPack">��ȡ�Ľ������� ��ΪHightLevel����Ϊ</param>
    public void AddRewardGotLevel(bool isVipPack, int level)
    {
        if (isVipPack)
            mVipRewardGotLevel.Value += m2BitTool.Get2BitValue(level);
        else
            mFreeRewardGotLevel.Value += m2BitTool.Get2BitValue(level);
    }


}
