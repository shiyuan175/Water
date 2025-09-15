using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurnTablePackSo", menuName = "Game/TurnTable Pack")]
public class TurnTablePackSo : ScriptableObject, IPackSoInterface
{
    [SerializeField] private int coins;
    [SerializeField] private List<ItemReward> items;
    [SerializeField] private List<SpecialReward> specialReward;
    [SerializeField] private GameDefine.AwardBaseProbability awardLevel;
    public int Coins => coins;
    public IReadOnlyList<ItemReward> ItemReward => items;
    public IReadOnlyList<SpecialReward> SpecialRewards => specialReward;

    public GameDefine.AwardBaseProbability AwardLevel => awardLevel;

    public string ItemCount()
    {
        if (ItemReward.Count > 0)
            return ItemReward[0].Quantity.ToString();
        if (SpecialRewards.Count > 0)
            return specialReward[0].Duration.ToString() + "m";

        return Coins.ToString();
    } 
}
