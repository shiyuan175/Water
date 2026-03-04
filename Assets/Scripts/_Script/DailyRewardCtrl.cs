using System;
using GameGlobalJson;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardCtrl : MonoBehaviour, ICanGetModel, ICanGetUtility
{
    [SerializeField] private GiftPackSO mDailyReward_ByGiftPack;
    [SerializeField] private GameObject mLockPanelNode;
    [SerializeField] private Button mClaimBtn;

    private GameGlobalModel mGameGlobalModel;
    private RewardGrantUtility mRewardGrantUtility;

    private void OnEnable()
    {
        if (mGameGlobalModel == null) return;
        
        var mClaimMark =
            (bool)mGameGlobalModel.GetFieldValue(mGameGlobalModel.DailyRewardJsonData, mDailyReward_ByGiftPack.ID);
        if (mClaimMark) mClaimBtn.interactable = false;
    }

    private void Start()
    {
        mGameGlobalModel = this.GetModel<GameGlobalModel>();
        mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();

        if (!(bool)mGameGlobalModel.GetFieldValue(mGameGlobalModel.GameGlobalJsonData.GiftPackPurchases, "gift_3"))
        {
            mLockPanelNode.Show();
            StringEventSystem.Global.Register("gift_3", OnPaySuccess).UnRegisterWhenGameObjectDestroyed(this);
        }

        var mClaimMark =
            (bool)mGameGlobalModel.GetFieldValue(mGameGlobalModel.DailyRewardJsonData, mDailyReward_ByGiftPack.ID);
        switch (mClaimMark)
        {
            case true:
                mClaimBtn.interactable = false;
                break;
            case false:
                mClaimBtn.onClick.AddListener(() =>
                {
                    mRewardGrantUtility.GrantReward(mDailyReward_ByGiftPack);
                    mClaimBtn.interactable = false;
                    mGameGlobalModel.SetFieldAndSave(JsonType.DailyRewardJson, mGameGlobalModel.DailyRewardJsonData,
                        mDailyReward_ByGiftPack.ID, true);
                    RewardUIManager.Instance.PlayRewardAnim(mDailyReward_ByGiftPack.Coins, true, null,
                        mDailyReward_ByGiftPack);
                    mClaimBtn.onClick.RemoveAllListeners();
                });
                break;
        }
    }

    private void OnPaySuccess()
    {
        mLockPanelNode.Hide();
        StringEventSystem.Global.UnRegister("gift_3", OnPaySuccess);
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
