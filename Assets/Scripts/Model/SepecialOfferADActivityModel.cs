using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SepecialOfferADActivityModel : AbstractModel
{

    private const string SEPECIAL_OFFER_ISBUY = "H_SepecialOfferIsBuy";
    private SaveDataUtility storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<SaveDataUtility>();
        
    }
}
