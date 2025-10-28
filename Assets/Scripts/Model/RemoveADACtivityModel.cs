using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveADACtivityModel : AbstractModel
{
    public bool IsBuy =>mIsBuy.Value;
    private readonly string REMOVEAD_IS_BUY = "H_RemoveADIsBuy";

    private BindableProperty<bool> mIsBuy;
    private SaveDataUtility mStorage;
    protected override void OnInit()
    {
        mStorage = this.GetUtility<SaveDataUtility>();
        mIsBuy = new BindableProperty<bool>();
        mIsBuy.SetValueWithoutEvent(mStorage.LoadBoolValue(REMOVEAD_IS_BUY, false));
        mIsBuy.Register(value =>
        {
            mStorage.SaveBool(REMOVEAD_IS_BUY, value);
        });
    }

    public void BuyGift()
    {
        mIsBuy.Value = true;
    }

   
}
