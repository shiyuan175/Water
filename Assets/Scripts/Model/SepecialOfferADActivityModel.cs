using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SepecialOfferADActivityModel : AbstractModel
{

    public bool IsBuy => mIsBuy.Value;
    private const string SEPECIAL_OFFER_ISBUY = "H_SepecialOfferIsBuy";
    private SaveDataUtility storage;
    
    private BindableProperty<bool> mIsBuy;
    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        mIsBuy = new BindableProperty<bool>();
        mIsBuy.SetValueWithoutEvent(storage.LoadBoolValue(SEPECIAL_OFFER_ISBUY, false));
        mIsBuy.Register(value =>
        {
            storage.SaveBool(SEPECIAL_OFFER_ISBUY,value);
        });
    }
    public void BuyGift()
    {
        mIsBuy.Value = true;
    }
    public void ReStartActivity()
    {
        mIsBuy.Value = false;
    }
}
