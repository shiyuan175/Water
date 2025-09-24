using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardPackSO", menuName = "Game/Reward Pack")]
public class RewardPackSO : ScriptableObject, IPackSoInterface
{
    [SerializeField] private int coins;
    [SerializeField] private bool removeAdsForever;
    [SerializeField] private List<ItemReward> items;
    [SerializeField] private List<SpecialReward> specialReward;

    public int Coins => coins;
    public bool RemoveAdsForever => removeAdsForever;
    public IReadOnlyList<ItemReward> ItemReward => items;
    public IReadOnlyList<SpecialReward> SpecialRewards => specialReward;
}