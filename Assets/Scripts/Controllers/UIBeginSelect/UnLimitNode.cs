using GameDefine;
using QFramework;
using System.Collections;
using TMPro;
using UnityEngine;

public class UnLimitNode : MonoBehaviour,ICanGetModel
{
    [SerializeField] private SpecialRewardsType mType;
    [SerializeField] private TextMeshProUGUI mCountDownTxts;

    private GameGlobalModel mGlobalModel;
    //private string mSign;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    private void Awake()
    {
        mGlobalModel = this.GetModel<GameGlobalModel>();
        if (mGlobalModel.IsTimerFinished(mGlobalModel.GameGlobalJsonData.TimedBuffData, mType.ToString()))
            gameObject.Hide();
    }

    private void Update()
    {
        if (!mGlobalModel.IsTimerFinished(mGlobalModel.GameGlobalJsonData.TimedBuffData, mType.ToString()))
        {
            mCountDownTxts.text = mGlobalModel.GetRemainingTimeText(
                mGlobalModel.GameGlobalJsonData.TimedBuffData, mType.ToString());
        }
        else
            gameObject.Hide();
    }
}
