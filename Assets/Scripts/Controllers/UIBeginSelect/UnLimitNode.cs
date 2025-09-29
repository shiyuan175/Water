using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnLimitNode : MonoBehaviour
{
    [SerializeField] private SpecialRewardsType mType;
    [SerializeField] private TextMeshProUGUI mCountDownTxts;
    private string mSign;


    private void Awake()
    {
        mSign = GameEnum.GetDescription(mType);
        if (mSign == null || CountDownTimerManager.Instance.IsTimerFinished(mSign))
            gameObject.Hide();
    }

    private void Update()
    {
        if (mSign != null && !CountDownTimerManager.Instance.IsTimerFinished(mSign))
        {
            mCountDownTxts.text = CountDownTimerManager.Instance.GetRemainingTimeText(mSign);
        }
        else
            gameObject.Hide();
    }
}
