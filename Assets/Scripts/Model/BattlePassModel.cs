using JsonFileData;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassModel : AbstractModel, ICanGetUtility
{

    private readonly string BP_GAMEWIN_NUM = "H_BPGameWinNum";
    private JsonFileUtility mJsonFileUtility;
    private BattlePassData mBPDate;
    private SaveDataUtility mStorage;
    private BindableProperty<int> mGameWinNum;
    
    protected override void OnInit()
    {
        mJsonFileUtility = this.GetUtility<JsonFileUtility>();
        mGameWinNum.SetValueWithoutEvent(mStorage.LoadIntValue(BP_GAMEWIN_NUM));
        mGameWinNum.Register(value =>
        {
            mStorage.SaveInt(BP_GAMEWIN_NUM, value);
        });
    }
}
