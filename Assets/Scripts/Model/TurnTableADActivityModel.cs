
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTableADActivityModel : AbstractModel
{
    public int CurrentTurnTableCount => mCurrentTurnTableCount;



    private int mCurrentTurnTableCount;
    private SaveDataUtility storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        mCurrentTurnTableCount = storage.GetTurnTableCount();
    }

    public void TurnTableAnimationEnd()
    {
        mCurrentTurnTableCount++;
        ChangeCount();
    }

    public void TurnTableTimeEnd()
    {
        mCurrentTurnTableCount = 0;
        ChangeCount();
    }

    private void ChangeCount()
    {
        storage.SaveTurnTableCount(mCurrentTurnTableCount);
    }
}
