
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTableADActivityModel : AbstractModel
{
    public int CurrentTurnTableCount => mCurrentTurnTableCount.Value;

    private const string TURN_TABLE_COUNT = "H_TurnTableCount";

    private BindableProperty<int> mCurrentTurnTableCount;
    private SaveDataUtility storage;

    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        mCurrentTurnTableCount = new BindableProperty<int>();
        mCurrentTurnTableCount.SetValueWithoutEvent(storage.LoadIntValue(TURN_TABLE_COUNT));
        mCurrentTurnTableCount.Register(value =>
        {
            storage.SaveInt(TURN_TABLE_COUNT, value);

        });
    }

    public void AddTurnTableCount()
    {
        ++mCurrentTurnTableCount.Value;
    }

    public void RefreshTurnTableCount()
    {
        mCurrentTurnTableCount.Value = 0;
    }
}
