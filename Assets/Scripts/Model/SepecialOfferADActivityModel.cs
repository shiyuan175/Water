using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SepecialOfferADActivityModel : AbstractModel
{
    public bool SepecialOfferIsbuy => mSepecialOfferIsbuyl.Value;

    private const string SEPECIAL_OFFER_ISBUY = "H_SepecialOfferIsBuy";
    private BindableProperty<bool> mSepecialOfferIsbuyl;
    private SaveDataUtility storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        mSepecialOfferIsbuyl.SetValueWithoutEvent(storage.LoadBoolValue(SEPECIAL_OFFER_ISBUY));
        mSepecialOfferIsbuyl.Register(value =>
        {
            storage.SaveBool(SEPECIAL_OFFER_ISBUY, value);
        });
    }
    public void ChangeSepecialOfferIsbuy(bool isbuy)
    {
        mSepecialOfferIsbuyl.Value = isbuy;
    }
}
