using QFramework;
using UnityEngine;

public class GiftPackCtrl : MonoBehaviour
{
    public GiftPackSO GiftPackSO;
    public AbilityGiftPackSO AbilityPackSO;
    public GameObject ProductObj;

    public void DisableProduct()
    {
        if (ProductObj != null) 
            ProductObj.Hide();
    }
}
