using System;
using Game.Water;
using QFramework;
using UnityEngine;

namespace Game.Water
{
    public class RewardUIManager : MonoSingleton<RewardUIManager>
    {
        [SerializeField] private RewardSpriteMappingSO mRewardSpriteMappingSO;

        public override void OnSingletonInit()
        {
            mRewardSpriteMappingSO.Initialize();
        }

        public Sprite GetRewardSprite<T>(T rewardType) where T : Enum
        {
            return mRewardSpriteMappingSO.GetRewardSprite<T>(rewardType);
        }
    }
}
