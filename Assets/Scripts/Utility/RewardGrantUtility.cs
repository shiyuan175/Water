using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGrantUtility : IUtility, ICanGetModel
{
    public void GrantReward(IPackSoInterface rewardPackSO)
    {
        StageModel _StageModel = this.GetModel<StageModel>();
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

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
