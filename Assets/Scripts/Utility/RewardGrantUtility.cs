using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class RewardGrantUtility : IUtility, ICanGetModel
{
    public void GrantReward(IPackSoInterface rewardPackSO)
    {
        StageModel _StageModel = this.GetModel<StageModel>();
        CoinManager.Instance.AddCoin(rewardPackSO.Coins);
        foreach (var item in rewardPackSO.ItemReward)
        {
            _StageModel.AddItem((int)item.NormalRewardsType, item.Quantity);
        }
        foreach (var item in rewardPackSO.SpecialRewards)
        {
            switch (item.SpecialRewardType)
            {
                case SpecialRewardsType.RemoveAds:
                    Debug.Log("È¥³ý¹ã¸æÂß¼­ÔÝ¿Õ");
                    break;
                case SpecialRewardsType.DoubleCoin:
                    CountDownTimerManager.Instance.AddTimer(GameDefine.GameConst.DOUBLE_COIN_SIGN, item.Duration);
                    break;
                case SpecialRewardsType.UnlimitedHp:
                    HealthManager.Instance.SetUnLimitHp(item.Duration);
                    break;
            }
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
