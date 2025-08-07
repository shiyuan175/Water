using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GiftPack", menuName = "Game/Gift Pack")]
public class GiftPackSO : ScriptableObject, IPackSoInterface
{
    //商城礼包专用
    [Header("道具ID")]
    [SerializeField] private string PackID;
    
    [SerializeField] private int coins;
    [SerializeField] private List<ItemReward> items;
    [Header("无限体力(单位：分钟)")]
    [SerializeField] private int unlimitedHp;
    [Header("无广告")]
    [SerializeField] private bool removeAds;

    public int Coins => coins;
    public int UnlimitedHp => unlimitedHp;
    public bool RemoveAds => removeAds;
    public string ID => PackID;
    public IReadOnlyList<ItemReward> ItemReward => items;
    
    private List<SpecialReward> emptySpeciaRewards = new();
    IReadOnlyList<SpecialReward> IPackSoInterface.SpecialRewards => emptySpeciaRewards;
}