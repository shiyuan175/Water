using System;
using System.Collections;
using System.Collections.Generic;
using GameDefine;
using UnityEngine;

public class HideWaterCtrl : MonoBehaviour
{
    public List<GameObject> blackWaterGos;
    private int _hideType = 0;

    public void SetHideShow(HideWaterType hideType)
    {
        if (blackWaterGos[_hideType])
            blackWaterGos[_hideType]?.SetActive(false);
        _hideType = (int)hideType;
        if (blackWaterGos[_hideType])
            blackWaterGos[_hideType]?.SetActive(true);
    }
}