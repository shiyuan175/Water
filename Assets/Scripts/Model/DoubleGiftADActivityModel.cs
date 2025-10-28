using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DoubleGiftADActivityModel : AbstractModel
{
    public bool IsBuy => mIsBuy.Value;
    public bool GiftIsGot => mGiftIsGot.Value;
    private const string DG_IS_BUY_SIGN = "H_DGisBuy";
    private const string DG_GIFT_IS_GOT = "H_DGGiftisGot";
    private SaveDataUtility mStorage;
    private IBindableProperty<bool> mIsBuy;
    private IBindableProperty<bool> mGiftIsGot;
    protected override void OnInit()
    {
        mStorage = this.GetUtility<SaveDataUtility>();
        mIsBuy = new BindableProperty<bool>();
        mIsBuy.SetValueWithoutEvent(mStorage.LoadBoolValue(DG_IS_BUY_SIGN, false));
        mIsBuy.Register(value =>
            {
            mStorage.SaveBool(DG_IS_BUY_SIGN, value);
        });
        
        mGiftIsGot = new BindableProperty<bool>();
        mGiftIsGot.SetValueWithoutEvent(mStorage.LoadBoolValue(DG_GIFT_IS_GOT, false));
        mGiftIsGot.Register(value =>
            {
            mStorage.SaveBool(DG_GIFT_IS_GOT, value);
        });
    }

    public void SetIsBuy()
    {
        mIsBuy.Value = true;
    }

    public void GetGift()
    {
        mGiftIsGot.Value = true;
    }
    public void ClearData()
    {
        mIsBuy.Value = false;
        mGiftIsGot.Value = false;
    }
}
