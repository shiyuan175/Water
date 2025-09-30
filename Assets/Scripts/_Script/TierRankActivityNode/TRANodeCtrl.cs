using JsonFileData;
using QFramework.Example;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TRANodeCtrl : MonoBehaviour
{
    [SerializeField] private Image mTierRankIcon;
    [SerializeField] private Image mAvatar;
    [SerializeField] private Image mAvatarFrame;
    [SerializeField] private Image mRankFrame;
    [SerializeField] private Image mStreakWinFrame;

    [SerializeField] private Sprite mRankFrame_Player;
    [SerializeField] private Sprite mRankFrame_Robot;
    [SerializeField] private Sprite mStreakWin_Player;
    [SerializeField] private Sprite mStreakWin_Robot;

    [SerializeField] private TextMeshProUGUI mTxtName;
    [SerializeField] private TextMeshProUGUI mTxtStreakWin_Red;

    private void Awake()
    {
        mTxtStreakWin_Red.font = LevelManager.Instance.redFont;
    }

    public void InitRobot(TRARobotsData traData ,Sprite tierRankSprite)
    {
        mAvatar.sprite = AvatarManager.Instance.GetAvatarSprite(true, traData.Avatar);
        mAvatarFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false, traData.AvatarFrame);
        mRankFrame.sprite = mRankFrame_Robot;
        mStreakWinFrame.sprite = mStreakWin_Robot;

        mTxtName.text = traData.Name;
        mTxtStreakWin_Red.text = $"{traData.StreamWinNum}";
        mTierRankIcon.sprite = tierRankSprite;
    }

    public void InitPlayer(TRAPlayer traData, Sprite tierRankSprite)
    {
        mAvatar.sprite = AvatarManager.Instance.GetAvatarSprite(true);
        mAvatarFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false);
        mRankFrame.sprite = mRankFrame_Player;
        mStreakWinFrame.sprite = mStreakWin_Player;

        mTxtName.text = traData.PlayerName;
        mTxtStreakWin_Red.text = $"{traData.StreamWinNum}";
        mTierRankIcon.sprite = tierRankSprite;
    }
}
