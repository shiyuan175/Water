using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public interface IPackSoInterface
{
    public IReadOnlyList<ItemReward> ItemReward { get; }

    public IReadOnlyList<SpecialReward> SpecialRewards { get; }

    public int Coins { get; }
}

public enum NormalRewardsType
{
    StepBack = 1,
    RemoveHide = 2,
    AddOneBottle = 3,
    AddHalfBottle = 4,
    RemoveAll = 5,
    S_AddOneBottle = 6,
    S_RemoveOneBottleHideWater = 7,
    S_RemoveOneDebuffBottle = 8,

    // 增加体力上限
    StaminaCap_2 = 9,
    StaminaCap_3 = 10,
    StaminaCap_5 = 11,

    //仅做表现
    //金币
    AddCoins = 20,
}

//时效Buff
public enum SpecialRewardsType
{
    RemoveAds = 0,
    DoubleCoin = 1,
    UnlimitedHp = 2,
    DoubleBuff = 3,
    Unlimited_S_AddOneBottle = 6,
    Unlimited_S_RemoveOneBottleHideWater = 7,
    Unlimited_S_RemoveOneDebuffBottle = 8,

    // 减少体力恢复时长(分钟)
    ReduceLiveRecoverTime_5 = 9,
    ReduceLiveRecoverTime_9 = 10,

    //仅做表现(不用于发放奖励)
    //三个进关选择道具(三个时长需相同)
    Unlimited_S_ALL = 20,
    ForeverDailyReward_ByGiftPack3 = 21,
}

[System.Serializable]
public class SpecialReward
{
    [SerializeField] private SpecialRewardsType specialRewardType;
    [Tooltip("分钟")]
    [SerializeField] private int duration;

    public SpecialRewardsType SpecialRewardType => specialRewardType;
    public int Duration => duration;
}

[System.Serializable]
public class ItemReward
{
    [SerializeField] private NormalRewardsType normalRewardsType;

    [Tooltip("道具数量")]
    [SerializeField] private int quantity;

    public NormalRewardsType NormalRewardsType => normalRewardsType;
    public int Quantity => quantity;
}
