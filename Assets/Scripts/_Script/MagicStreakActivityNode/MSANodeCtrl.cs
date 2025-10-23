using System.Collections;
using System.Collections.Generic;
using JsonFileData;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MSANodeCtrl : MonoBehaviour
{
    //1-5名道具奖励，6-20金币奖励
    private const int REWARD_RANKING = 5;
    private const int COIN_REWARD_RANKING = 20;

    [SerializeField] private Sprite[] mRankSprites;
    [SerializeField] private Sprite mRankPlayerFrameBg;
    [SerializeField] private Sprite mRankPlayerScoreBg;
    [SerializeField] private Sprite mRankRobotFrameBg;
    [SerializeField] private Sprite mRankRobotScoreBg;

    [SerializeField] private Image mRankFrameBg;
    [SerializeField] private Image mScoreBg;
    [SerializeField] private Image mRankIcon;
    [SerializeField] private Image mAvatar;
    [SerializeField] private Image mAvatarFrame;

    [SerializeField] private TextMeshProUGUI mTxtRank;
    [SerializeField] private TextMeshProUGUI mTxtName;
    [SerializeField] private TextMeshProUGUI mTxtScore_Red;


    [Header("Box")]
    [SerializeField] private Button mBtnBox;
    [SerializeField] private GameObject mImgRewardPanel;
    [SerializeField] private Transform mRewardParNode;
    [SerializeField] private Sprite mBoxSprite;
    [SerializeField] private Sprite mCoinSprite;

    private void Awake()
    {
        mTxtScore_Red.font = LevelManager.Instance.redFont;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && mImgRewardPanel.gameObject.activeSelf)
        {
            mImgRewardPanel.Hide();
            RewardUIManager.Instance.RecyleAll();
        }
    }

    public void InitRobot(int rank, MSARobotsData msaData, IPackSoInterface rankingPackSO)
    {
        mRankFrameBg.sprite = mRankRobotFrameBg;
        mScoreBg.sprite = mRankRobotScoreBg;
        mAvatar.sprite = AvatarManager.Instance.GetAvatarSprite(true, msaData.Avatar);
        mAvatarFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false, msaData.AvatarFrame);

        if (rank >= 1 && rank <= 3)
        {
            mRankIcon.Show();
            mRankIcon.sprite = mRankSprites[rank - 1];
        }
        else
        {
            mTxtRank.Show();
            mTxtRank.text = $"{rank}";
        }
           
        mTxtName.text = msaData.Name;
        mTxtScore_Red.text = $"{msaData.Score}";

        SetupRewardBox(rank, rankingPackSO);
    }

    public void InitPlayer(int rank, MSAPlayer msaData, IPackSoInterface rankingPackSO)
    {
        mRankFrameBg.sprite = mRankPlayerFrameBg;
        mScoreBg.sprite = mRankPlayerScoreBg;;
        mAvatar.sprite = AvatarManager.Instance.GetAvatarSprite(true);
        mAvatarFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false);

        if (rank >= 1 && rank <= 3)
        {
            mRankIcon.Show();
            mRankIcon.sprite = mRankSprites[rank - 1];
        }
        else
        {
            mTxtRank.Show();
            mTxtRank.text = $"{rank}";
        }

        mTxtName.text = msaData.PlayerName;
        mTxtScore_Red.text = $"{msaData.Score}";

        SetupRewardBox(rank, rankingPackSO);
    }

    public void DisInit()
    {
        mRankIcon.Hide();
        mTxtRank.Hide();
        mRankIcon.sprite = null;
        mAvatar.sprite = null;
        mAvatarFrame.sprite = null;
        mRankFrameBg.sprite = null;
        mScoreBg.sprite = null;

        mTxtRank.text = null;
        mTxtName.text = null;
        mTxtScore_Red.text = null;
    }

    private void SetupRewardBox(int ranking, IPackSoInterface rankingPackSO)
    {
        if (ranking <= REWARD_RANKING)
        {
            mBtnBox.Show();
            mBtnBox.image.sprite = mBoxSprite;
            mBtnBox.interactable = true;
            mBtnBox.onClick.RemoveAllListeners();
            mBtnBox.onClick.AddListener(() =>
            {
                var _state = mImgRewardPanel.gameObject.activeSelf;
                if (!_state)
                {
                    mImgRewardPanel.Show();
                    DisplayRewards(rankingPackSO);
                }
            });
        }
        else if (ranking <= COIN_REWARD_RANKING)
        {
            mBtnBox.Show();
            mBtnBox.image.sprite = mCoinSprite;
            mBtnBox.interactable = false;
        }
    }

    private void DisplayRewards(IPackSoInterface rankingPackSO)
    {
        for (int i = 0; i < rankingPackSO.SpecialRewards.Count; i++)
        {
            int _idx = i;
            var _img = RewardUIManager.Instance.Allocate();
            _img.transform.SetParent(mRewardParNode);
            _img.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            _img.TryGetComponent(out PropRewardPoolNode _node);
            if (_node == null)
                _node = _img.gameObject.AddComponent<PropRewardPoolNode>();

            Sprite _rewardSprite = RewardUIManager.Instance.GetRewardSprite(rankingPackSO.SpecialRewards[_idx].SpecialRewardType);
            _node.Init(_rewardSprite, Vector2.zero, rankingPackSO.SpecialRewards[_idx].Duration, true);
        }

        for (int i = 0; i < rankingPackSO.ItemReward.Count; i++)
        {
            int _idx = i;
            var _img = RewardUIManager.Instance.Allocate();
            _img.transform.SetParent(mRewardParNode);
            _img.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            _img.TryGetComponent(out PropRewardPoolNode _node);
            if (_node == null)
                _node = _img.gameObject.AddComponent<PropRewardPoolNode>();

            Sprite _rewardSprite = RewardUIManager.Instance.GetRewardSprite(rankingPackSO.ItemReward[_idx].NormalRewardsType);
            _node.Init(_rewardSprite, Vector2.zero, rankingPackSO.ItemReward[_idx].Quantity, false);
        }
    }
}
