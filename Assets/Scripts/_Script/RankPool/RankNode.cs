using System.Collections;
using System.Collections.Generic;
using A_PlayerRankData;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;


public class RankNode : MonoBehaviour ,ICanGetModel
{
    private RankDataModel mRankDataModel;

    public Text TxtRank;
    public Text TxtName;
    public Text TxtScore;

    private void Awake()
    {
        mRankDataModel = this.GetModel<RankDataModel>();
    }

    public void Init(int rank, PlayerRankData rankData)
    {
        TxtRank.text = rank.ToString();
        TxtName.text = rankData.playerName.ToString();
        TxtScore.text = rankData.trophyCount.ToString();
    }

    public void InitPlayer(int rank, int score)
    {
        TxtRank.text = rank.ToString();
        TxtName.text = "YOU";
        TxtName.color = Color.yellow;
        TxtScore.text = score.ToString();
    }

    public void RankingChangeOther(int rank, bool isBackNode, System.Action complete)
    {
        if (!int.TryParse(TxtRank.text, out int startRank))
        {
            startRank = rank;
        }
        float currentValue = startRank;

        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            int currentRank = Mathf.FloorToInt(currentValue);
            int safeIndex = Mathf.Clamp(currentRank - 1, 0, mRankDataModel.RankDataList.Count - 1);
            TxtRank.text = currentRank.ToString();

            var data = mRankDataModel.RankDataList[safeIndex];
            TxtScore.text = data.trophyCount.ToString();
            TxtName.text = data.playerName;
        }, rank, 2f)
        .OnComplete(() =>
        {
            //后排节点排名向下顺延
            if (isBackNode)
                TxtRank.text = (rank + 1).ToString();
            complete?.Invoke();
        });

    }

    public void OnDisable()
    {
        TxtRank.text = "";
        TxtName.text = "";
        TxtName.color = Color.white;
        TxtScore.text = "";
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
