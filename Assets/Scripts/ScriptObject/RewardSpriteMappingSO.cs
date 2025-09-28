using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[CreateAssetMenu(menuName = "Game/RewardSpriteMapping")]
public class RewardSpriteMappingSO : ScriptableObject
{
    [System.Serializable]
    public class SpecialRewardSpriteEntry
    {
        public SpecialRewardsType rewardType;
        public Sprite sprite;
    }

    [System.Serializable]
    public class NormalRewardSpriteEntry
    {
        public NormalRewardsType rewardType;
        public Sprite sprite;
    }

    [SerializeField] private List<NormalRewardSpriteEntry> normalRewardSprites = new();
    [SerializeField] private List<SpecialRewardSpriteEntry> specialRewardSprites = new();

    private Dictionary<SpecialRewardsType, Sprite> specialLookup;
    private Dictionary<NormalRewardsType, Sprite> normalLookup;

    public void Initialize()
    {
        specialLookup = new();
        foreach (var entry in specialRewardSprites)
            specialLookup[entry.rewardType] = entry.sprite;

        normalLookup = new();
        foreach (var entry in normalRewardSprites)
            normalLookup[entry.rewardType] = entry.sprite;
    }
  
    public Sprite GetRewardSprite<T>(T rewardType) where T : Enum
    {
        if (typeof(T) == typeof(SpecialRewardsType))
        {
            return specialLookup.TryGetValue((SpecialRewardsType)(object)rewardType, out var sprite) ? sprite : null;
        }
        else if (typeof(T) == typeof(NormalRewardsType))
        {
            return normalLookup.TryGetValue((NormalRewardsType)(object)rewardType, out var sprite) ? sprite : null;
        }

        return null;
    }
    public Sprite GetRewardSprite(string rewardString)
    {
        SpecialRewardsType _rewardEnum1;
        if (Enum.TryParse<SpecialRewardsType>(rewardString,out _rewardEnum1))
        {
            return GetRewardSprite<SpecialRewardsType>(_rewardEnum1);
        }

        NormalRewardsType _rewardEnum2;
        if (Enum.TryParse<NormalRewardsType>(rewardString, out _rewardEnum2))
        {
            return GetRewardSprite<NormalRewardsType>(_rewardEnum2);
        }
        return null;
    }
}
