using System.Collections;
using System.Collections.Generic;
using JsonFileData;
using QFramework;
using QFramework.Example;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MSANodeCtrl : MonoBehaviour
{
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

    public void Awake()
    {
        mTxtScore_Red.font = LevelManager.Instance.redFont;
    }

    public void InitRobot(int rank, MSARobotsData msaData)
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
    }

    public void InitPlayer(int rank, MSAPlayer msaData)
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
}
