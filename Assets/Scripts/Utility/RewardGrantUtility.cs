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
                case NormalRewardsType.StaminaCap_2:
                    mGameGlobalModel.AddMaxHp(2);
                    break;

                case NormalRewardsType.StaminaCap_3:
                    mGameGlobalModel.AddMaxHp(3);
                    break;

                case NormalRewardsType.StaminaCap_5:
                    mGameGlobalModel.AddMaxHp(5);
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
                case SpecialRewardsType.ReduceLiveRecoverTime_5:
                    mGameGlobalModel.ReduceHpRecoverTimer(item.Duration);
                    break;
                case SpecialRewardsType.ReduceLiveRecoverTime_9:
                    mGameGlobalModel.ReduceHpRecoverTimer(item.Duration);
                    break;

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
                    case NormalRewardsType.StaminaCap_2:
                        mGameGlobalModel.AddMaxHp(2);
                        break;

                    case NormalRewardsType.StaminaCap_3:
                        mGameGlobalModel.AddMaxHp(3);
                        break;

                    case NormalRewardsType.StaminaCap_5:
                        mGameGlobalModel.AddMaxHp(5);
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
                    case SpecialRewardsType.ReduceLiveRecoverTime_5:
                        mGameGlobalModel.ReduceHpRecoverTimer(item.itemQuantity);
                        break;
                    case SpecialRewardsType.ReduceLiveRecoverTime_9:
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
