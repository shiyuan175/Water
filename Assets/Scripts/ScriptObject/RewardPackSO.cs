using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardPackSO", menuName = "Game/Reward Pack")]
public class RewardPackSO : ScriptableObject, IPackSoInterface
{
    [SerializeField] private int coins;
    [SerializeField] private List<ItemReward> items;
    [SerializeField] private List<SpecialReward> specialReward;

    public int Coins => coins;
    public IReadOnlyList<ItemReward> ItemReward => items;
    public IReadOnlyList<SpecialReward> SpecialRewards => specialReward;


    //接口实现
    IReadOnlyList<SpecialReward> IPackSoInterface.SpecialRewards => specialReward;
    IReadOnlyList<ItemReward> IPackSoInterface.ItemReward => items;
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
