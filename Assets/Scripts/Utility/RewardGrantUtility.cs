using GameDefine;
using JsonFileData;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class RewardGrantUtility : IUtility, ICanGetModel
{
    StageModel _StageModel;
    public void GrantReward(IPackSoInterface rewardPackSO)
    {
        _StageModel = this.GetModel<StageModel>();
        CoinManager.Instance.AddCoin(rewardPackSO.Coins);
        //永久去广告逻辑待补充 rewardPackSO.RemoveAdsForever

        foreach (var item in rewardPackSO.ItemReward)
        {
            _StageModel.AddItem((int)item.NormalRewardsType, item.Quantity);
        }
        foreach (var item in rewardPackSO.SpecialRewards)
        {
            switch (item.SpecialRewardType)
            {
                case SpecialRewardsType.RemoveAds:
                    Debug.Log("去除广告逻辑暂空");
                    break;
                case SpecialRewardsType.DoubleCoin:
                    CountDownTimerManager.Instance.AddTimer(GameDefine.GameConst.DOUBLE_COIN_SIGN, item.Duration);
                    break;
                case SpecialRewardsType.UnlimitedHp:
                    HealthManager.Instance.SetUnLimitHp(item.Duration);
                    break;

                //有配置 Description 特性走默认处理逻辑
                default:
                    CountDownTimerManager.Instance.AddTimer(GameEnum.GetDescription(item.SpecialRewardType), item.Duration);
                    break;
            }
        }
    }
    public void GrantReward(RewardItem[] rewardItems)
    {
        _StageModel = this.GetModel<StageModel>();
        foreach (var item in rewardItems)
        {
            string rewardString = item.itemType;
            SpecialRewardsType _rewardEnum1;
            if (Enum.TryParse<SpecialRewardsType>(rewardString, out _rewardEnum1))
            {
                switch (_rewardEnum1)
                {
                    case SpecialRewardsType.RemoveAds:
                        Debug.Log("去除广告逻辑暂空");
                        break;
                    case SpecialRewardsType.DoubleCoin:
                        CountDownTimerManager.Instance.AddTimer(GameDefine.GameConst.DOUBLE_COIN_SIGN, item.itemQuantity);
                        break;
                    case SpecialRewardsType.UnlimitedHp:
                        HealthManager.Instance.SetUnLimitHp(item.itemQuantity);
                        break;

                    //有配置 Description 特性走默认处理逻辑
                    default:
                        CountDownTimerManager.Instance.AddTimer(GameEnum.GetDescription(_rewardEnum1), item.itemQuantity);
                        break;
                }
            }

            NormalRewardsType _rewardEnum2;
            if (Enum.TryParse<NormalRewardsType>(rewardString, out _rewardEnum2))
            {
                _StageModel.AddItem((int)_rewardEnum2, item.itemQuantity);
            }
        }
    }
    

    
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
