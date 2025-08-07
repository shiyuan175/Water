using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPackSoInterface
{
    public IReadOnlyList<ItemReward> ItemReward { get; }

    public IReadOnlyList<SpecialReward> SpecialRewards { get; }

    public int Coins { get; }
}

public enum SpecialRewardsType
{
    RemoveAds = 0,
    DoubleCoin = 1,
    UnlimitedHp = 2
}

[System.Serializable]
public class SpecialReward
{
    [SerializeField] private SpecialRewardsType specialRewardType;
    [SerializeField] private int duration;
    [SerializeField] private Sprite rewardSprite;

    public SpecialRewardsType SpecialRewardType => specialRewardType;
    public int Duration => duration;
    public Sprite RewardSprite => rewardSprite;
}

[System.Serializable]
public class ItemReward
{
    [Tooltip("道具索引，1~8")]
    [Range(1, 8)]
    [SerializeField] private int itemIndex;

    [Tooltip("道具数量")]
    [SerializeField] private int quantity;

    // 只读属性
    public int ItemIndex => itemIndex;
    public int Quantity => quantity;
}