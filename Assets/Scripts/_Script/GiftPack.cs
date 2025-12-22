using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftPack : MonoBehaviour
{
    public GiftPackSO giftPack;
    public GameObject productObj;

    public void DisableProduct()
    {
        if (productObj != null) 
            productObj.Hide();
    }
}
