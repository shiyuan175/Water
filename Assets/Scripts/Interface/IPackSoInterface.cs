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
    S_AddOneBottle = 6,
    S_RemoveHide = 7,
    S_ChangeWater = 8,
}

public enum SpecialRewardsType
{
    RemoveAds = 0,
    DoubleCoin = 1,
    UnlimitedHp = 2,
    [Description("DoubleSettlement")]
    UnlimitedDoubleBuff = 3,
    [Description("UnLimitAddBottleItem")]
    Unlimited_S_AddOneBottle = 6,
    [Description("UnLimitRemoveHideItem")]
    Unlimited_S_RemoveHide = 7,
    [Description("UnLimitChangeWaterItem")]
    Unlimited_S_ChangeWater = 8,
    
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