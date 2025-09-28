using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnLimitNode : MonoBehaviour,ICanGetUtility
{
    [SerializeField] private SpecialRewardsType mType;
    [SerializeField] private TextMeshProUGUI mCountDownTxts;
    private string mSign;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    private void Awake()
    {
        mSign = GameEnum.GetDescription(mType);
        if (mSign == null || CountDownTimerManager.Instance.IsTimerFinished(mSign)|| this.GetUtility<SaveDataUtility>().GetCurrentLevel()<=(int)GameDefine.UnLockMechanism.EnterLevelSelectProps)
            gameObject.Hide();
    }

    private void Update()
    {
        if (mSign != null && !CountDownTimerManager.Instance.IsTimerFinished(mSign)&&this.GetUtility<SaveDataUtility>().GetCurrentLevel() > (int)GameDefine.UnLockMechanism.EnterLevelSelectProps)
        {
            mCountDownTxts.text = CountDownTimerManager.Instance.GetRemainingTimeText(mSign);
        }
        else
            gameObject.Hide();
    }
}
