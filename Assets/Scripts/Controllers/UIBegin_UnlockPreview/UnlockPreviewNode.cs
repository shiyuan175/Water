using DG.Tweening;
using QFramework;
using QFramework.Example;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPreviewNode : MonoBehaviour ,ICanGetUtility
{
    public enum RewardSource
    {
       DailyReward,
       GameGlobalData
    }

    public enum RewardField
    {
        //对应奖励字段
        IsClaim_UnlockScene1Reward,
        IsClaim_UnlockScene2Reward,
        IsClaim_UnlockScene3Reward,
    }

    [System.Serializable]
    public struct RewardFieldConfig
    {
        public RewardSource RewardSource;
        public RewardField RewardField;
    }

    [SerializeField] private RewardFieldConfig mRewardFieldConfig;
    [SerializeField] private GiftPackSO mUnlockOverPackSO;

    [SerializeField] private Image mImgPreview;
    [SerializeField] private Image mBlueFrame;
    [SerializeField] private Image mLock;
    [SerializeField] private Button mGreyFrame;
    [SerializeField] private Transform mUnlockOverReward;
    [SerializeField] private Transform mUnlockOverBuffLock;
    [SerializeField] private TextMeshProUGUI[] mRewardText;
    [SerializeField] private Sprite mNewSprite;

    [Header("场景索引(从0开始)、当前场景部件数、上个场景部件数")]
    [SerializeField] private int mSceneIdx;
    [SerializeField] private int mThisScenePartTotal;
    [SerializeField] private int mPrevScenePartTotal;

    private Button mUnlockOverBuffButton;
    private SceneUnlockModel mSceneUnlockModel;
    private GameGlobalModel mGameGlobalModel;

    public void Init(SceneUnlockModel sceneUnlockModel, GameGlobalModel gameGlobalModel)
    {
        mSceneUnlockModel = sceneUnlockModel;
        mGameGlobalModel = gameGlobalModel;
        if (sceneUnlockModel.SceneIndex >= mSceneIdx)
        {
            mBlueFrame.DOFade(1, 0f);
            mGreyFrame.Hide();
            mLock.Hide();
        }
    }

    public void CheckUnlockFinish()
    {
        //当前场景是否激活
        if (mSceneUnlockModel.SceneIndex < mSceneIdx && mSceneIdx > 0)
        {
            mUnlockOverReward.Hide();
            return;
        }
        else
            mUnlockOverReward.Show();

        //本场景是否解锁完
        var unlockOver = mSceneUnlockModel.GetSceneUnitIndex(mSceneIdx) >= mThisScenePartTotal;
        if (unlockOver)
        {
            mImgPreview.sprite = mNewSprite;
            mUnlockOverBuffLock.Hide();
        }

        //解锁完成每日奖励领取(每日奖励是否领取)
        if (mRewardFieldConfig.RewardSource is RewardSource.DailyReward)
        {
            if (IsRewardClaimed())
            {
                mUnlockOverReward.Hide();
                return;
            }
            else
                mUnlockOverReward.Show();

            mUnlockOverBuffButton ??= mUnlockOverReward.GetComponent<Button>();
            if (!unlockOver)
                mUnlockOverBuffButton.interactable = false;
            else
                mUnlockOverBuffButton.interactable = true;
        }
    }
   
    private void Start()
    {
        if (mSceneUnlockModel.SceneIndex < mSceneIdx && mSceneIdx > 0)
            mGreyFrame.onClick.AddListener(UnlockEvent);

        foreach (TextMeshProUGUI textMeshProUGUI in mRewardText)
        {
            textMeshProUGUI.font = LevelManager.Instance.redFont;
        }

        if (mRewardFieldConfig.RewardSource is RewardSource.DailyReward)
        {
            mUnlockOverReward.TryGetComponent(out Button rewardButton);
            rewardButton?.onClick.AddListener(ClaimRewardEvent);
        }
    }

    private void UnlockEvent()
    {
        if (mSceneUnlockModel.GetSceneUnitIndex(mSceneIdx - 1) >= mPrevScenePartTotal)
        {
            UIKit.OpenPanel<UIMask>();
            mGreyFrame.onClick.RemoveListener(UnlockEvent);
            mGreyFrame.interactable = false;
            mSceneUnlockModel.UpdateSceneIdx(mSceneIdx);
            mBlueFrame.DOFade(1, 1.5f);
            mGreyFrame.image.DOFade(0, 1.2f).OnComplete(() => {
                    mGreyFrame.Hide();
                    mUnlockOverReward.Show();
                    mLock.Hide();
                    UIKit.ClosePanel<UIMask>();
                });
        }
    }

    private void ClaimRewardEvent()
    {
        if (mUnlockOverPackSO is null) return;

        this.GetUtility<RewardGrantUtility>().GrantReward(mUnlockOverPackSO);
        SetRewardClaimed();
        RewardUIManager.Instance.PlayRewardAnim(mUnlockOverPackSO.Coins, true, null, mUnlockOverPackSO);
        mUnlockOverReward.Hide();
        mUnlockOverReward.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 获取字段值
    /// </summary>
    /// <returns></returns>
    private bool IsRewardClaimed()
    {
        switch (mRewardFieldConfig.RewardSource)
        {
            case RewardSource.DailyReward:
                return GetFieldValue(mGameGlobalModel.DailyRewardJsonData, mRewardFieldConfig.RewardField);
            case RewardSource.GameGlobalData:
                return GetFieldValue(mGameGlobalModel.GameGlobalJsonData, mRewardFieldConfig.RewardField);
            default:
                return true;
        }
    }

    private bool GetFieldValue(object obj, RewardField field)
    {
        var fieldInfo = obj.GetType().GetField(field.ToString());
        if (fieldInfo == null) return true;
        return (bool)fieldInfo.GetValue(obj);
    }

    /// <summary>
    /// 保存字段值
    /// </summary>
    private void SetRewardClaimed()
    {
        object targetJsonData = null;
        switch (mRewardFieldConfig.RewardSource)
        {
            case RewardSource.DailyReward:
                targetJsonData = mGameGlobalModel.DailyRewardJsonData;
                break;
            case RewardSource.GameGlobalData:
                targetJsonData = mGameGlobalModel.GameGlobalJsonData;
                break;
        }
        if (targetJsonData == null) return;

        var fieldInfo = targetJsonData.GetType().GetField(mRewardFieldConfig.RewardField.ToString());
        if (fieldInfo == null) return;
        fieldInfo.SetValue(targetJsonData, true);

        if (mRewardFieldConfig.RewardSource == RewardSource.DailyReward)
            mGameGlobalModel.SaveDailyRewardJson();
        else if (mRewardFieldConfig.RewardSource == RewardSource.GameGlobalData)
            mGameGlobalModel.SaveGameGlobalJson();
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
