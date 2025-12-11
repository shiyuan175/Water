using GameDefine;
using JsonFileData;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGrantUtility : IUtility, ICanGetModel
{
    GameGlobalModel _gameGlobalModel;
    public void GrantReward(IPackSoInterface rewardPackSO)
    {
        _gameGlobalModel = this.GetModel<GameGlobalModel>();
        CoinManager.Instance.AddCoin(rewardPackSO.Coins);
        //����ȥ����߼������� rewardPackSO.RemoveAdsForever

        foreach (var item in rewardPackSO.ItemReward)
        {
            _gameGlobalModel.AddItem((int)item.NormalRewardsType, item.Quantity);
        }
        foreach (var item in rewardPackSO.SpecialRewards)
        {
            switch (item.SpecialRewardType)
            {
                case SpecialRewardsType.RemoveAds:
                    Debug.Log("ȥ������߼��ݿ�");
                    break;
                case SpecialRewardsType.DoubleCoin:
                    CountDownTimerManager.Instance.AddTimer(GameDefine.GameConst.DOUBLE_COIN_SIGN, item.Duration);
                    break;
                case SpecialRewardsType.UnlimitedHp:
                    HealthManager.Instance.SetUnLimitHp(item.Duration);
                    break;

                //������ Description ������Ĭ�ϴ����߼�
                default:
                    CountDownTimerManager.Instance.AddTimer(GameEnum.GetDescription(item.SpecialRewardType), item.Duration);
                    break;
            }
        }
    }
    public void GrantReward(RewardItem[] rewardItems)
    {
        _gameGlobalModel = this.GetModel<GameGlobalModel>();
        foreach (var item in rewardItems)
        {
            string rewardString = item.itemType;
            SpecialRewardsType _rewardEnum1;
            if (Enum.TryParse<SpecialRewardsType>(rewardString, out _rewardEnum1))
            {
                switch (_rewardEnum1)
                {
                    case SpecialRewardsType.RemoveAds:
                        Debug.Log("ȥ������߼��ݿ�");
                        break;
                    case SpecialRewardsType.DoubleCoin:
                        CountDownTimerManager.Instance.AddTimer(GameDefine.GameConst.DOUBLE_COIN_SIGN, item.itemQuantity);
                        break;
                    case SpecialRewardsType.UnlimitedHp:
                        HealthManager.Instance.SetUnLimitHp(item.itemQuantity);
                        break;

                    //������ Description ������Ĭ�ϴ����߼�
                    default:
                        CountDownTimerManager.Instance.AddTimer(GameEnum.GetDescription(_rewardEnum1), item.itemQuantity);
                        break;
                }
            }

            NormalRewardsType _rewardEnum2;
            if (Enum.TryParse<NormalRewardsType>(rewardString, out _rewardEnum2))
            {
                _gameGlobalModel.AddItem((int)_rewardEnum2, item.itemQuantity);
            }
        }
    }
    

    
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
