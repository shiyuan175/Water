using GameDefine;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
