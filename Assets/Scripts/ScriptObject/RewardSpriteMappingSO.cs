using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/RewardSpriteMapping")]
public class RewardSpriteMappingSO : ScriptableObject
{
    [System.Serializable]
    public class NormalRewardSpriteEntry
    {
        public NormalRewardsType rewardType;
        public Sprite sprite;
    }

    [SerializeField] private List<NormalRewardSpriteEntry> normalRewardSprites = new();

    private Dictionary<NormalRewardsType, Sprite> normalLookup;

    public void Initialize()
    {
        normalLookup = new();
        foreach (var entry in normalRewardSprites)
            normalLookup[entry.rewardType] = entry.sprite;
    }
  
    public Sprite GetRewardSprite<T>(T rewardType) where T : Enum
    {
        return normalLookup.TryGetValue((NormalRewardsType)(object)rewardType, out Sprite sprite) ? sprite : null;
    }
}
