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
    public bool RemoveAdsForever { get; }
}

public enum NormalRewardsType
{
    StepBack = 1,
    RemoveHide = 2,
    AddOneBottle = 3,
    AddHalfBottle = 4,
    RemoveAll = 5,
    S_AddOneHalfBottle = 6,
    S_RemoveOneBottleHideWater = 7,
    S_RemoveOneDebuffBottle = 8,

    //金币，只用于做表现
    AddCoins = 20,
}

public enum SpecialRewardsType
{
    RemoveAds = 0,
    DoubleCoin = 1,
    UnlimitedHp = 2,
    [Description("DoubleSettlement")]
    UnlimitedDoubleBuff = 3,
    [Description("UnLimitAddOneHalfBottle")]
    Unlimited_S_AddOneHalfBottle = 6,
    [Description("UnLimitRemoveOneBottleHideWater")]
    Unlimited_S_RemoveOneBottleHideWater = 7,
    [Description("UnLimitRemoveOneDebuffBottle")]
    Unlimited_S_RemoveOneDebuffBottle = 8,

    //表示三个进关选择道具(三个时长需相同)
    //目前只用于做表现(不用于发放奖励)
    Unlimited_S_ALL = 20,
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