using GameDefine;
using JsonFileData;
using QFramework;
using System;

public class RewardGrantUtility : IUtility, ICanGetModel
{
    private GameGlobalModel mGameGlobalModel;

    public void GrantReward(IPackSoInterface rewardPackSO)
    {
        mGameGlobalModel ??= this.GetModel<GameGlobalModel>();
        CoinManager.Instance.AddCoin(rewardPackSO.Coins);

        if (rewardPackSO is AbilityGiftPackSO abilitySO)
            abilitySO.GrantPrivilegeAbility(mGameGlobalModel);

        foreach (var item in rewardPackSO.ItemReward)
        {
            switch (item.NormalRewardsType)
            {
                case NormalRewardsType.StaminaCap:
                    mGameGlobalModel.AddMaxHp(item.Quantity);
                    break;

                default:
                    mGameGlobalModel.AddItem((int)item.NormalRewardsType, item.Quantity);
                    break;
            }
        }

        foreach (var item in rewardPackSO.SpecialRewards)
        {
            switch (item.SpecialRewardType)
            {
                case SpecialRewardsType.UnlimitedHp:
                    HealthManager.Instance.SetUnLimitHp(item.Duration);
                    break;
                case SpecialRewardsType.ReduceLiveRecoverTime:
                    mGameGlobalModel.ReduceHpRecoverTimer(item.Duration);
                    break;

                //有配置 Description 特性走默认处理逻辑
                default:
                    mGameGlobalModel.AddTimerToJson(mGameGlobalModel.GameGlobalJsonData.TimedBuffData,
                        item.SpecialRewardType.ToString(), item.Duration);
                    //CountDownTimerManager.Instance.AddTimer(GameEnum.GetDescription(item.SpecialRewardType), item.Duration);
                    break;
            }
        }
    }

    public void GrantReward(RewardItem[] rewardItems)
    {
        mGameGlobalModel ??= this.GetModel<GameGlobalModel>();
        foreach (var item in rewardItems)
        {
            string rewardString = item.itemType;

            if (Enum.TryParse<NormalRewardsType>(rewardString, out NormalRewardsType normalReward))
            {
                switch (normalReward)
                {
                    case NormalRewardsType.StaminaCap:
                        mGameGlobalModel.AddMaxHp(item.itemQuantity);
                        break;

                    default:
                        mGameGlobalModel.AddItem((int)normalReward, item.itemQuantity);
                        break;
                }
            }

            if (Enum.TryParse<SpecialRewardsType>(rewardString, out SpecialRewardsType specialReward))
            {
                switch (specialReward)
                {
                    case SpecialRewardsType.UnlimitedHp:
                        HealthManager.Instance.SetUnLimitHp(item.itemQuantity);
                        break;
                    case SpecialRewardsType.ReduceLiveRecoverTime:
                        mGameGlobalModel.ReduceHpRecoverTimer(item.itemQuantity);
                        break;

                    default:
                        mGameGlobalModel.AddTimerToJson(mGameGlobalModel.GameGlobalJsonData.TimedBuffData,
                            specialReward.ToString(), item.itemQuantity);
                        break;
                }
            }
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
