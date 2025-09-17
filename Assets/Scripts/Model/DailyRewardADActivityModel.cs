using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardADActivityModel : AbstractModel
{
    private const string DR_STAGE_NUM_SIGN = "H_DailyRewardADActivityStageNum";

    public int CurrentWatchADCount => mCurrentWatchADCount.Value;

    private IBindableProperty<int> mCurrentWatchADCount;
    private SaveDataUtility storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        mCurrentWatchADCount = new BindableProperty<int>();
        mCurrentWatchADCount.Register(value =>
        {
            storage.SaveInt(DR_STAGE_NUM_SIGN, value);
        });
        mCurrentWatchADCount.SetValueWithoutEvent(storage.LoadIntValue(DR_STAGE_NUM_SIGN,0));
    }
    public void AddStageNum()
    {
        mCurrentWatchADCount.Value++;
    }

    public void ClearStageNum()
    {
        mCurrentWatchADCount.Value = 0;  
    }

}
