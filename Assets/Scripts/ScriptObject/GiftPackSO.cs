using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GiftPack", menuName = "Game/Gift Pack")]
public class GiftPackSO : ScriptableObject, IPackSoInterface
{
    [Header("道具ID")]
    [SerializeField] private string PackID;
    
    [SerializeField] private int coins;
    [Header("永久去广告")]
    [SerializeField] private bool removeAdsForever;
    [SerializeField] private List<ItemReward> items;
    [SerializeField] private List<SpecialReward> specialsItem;

    public int Coins => coins;
    public bool RemoveAdsForever => removeAdsForever;
    public string ID => PackID;
    public IReadOnlyList<ItemReward> ItemReward => items;
    public IReadOnlyList<SpecialReward> SpecialRewards => specialsItem;

}