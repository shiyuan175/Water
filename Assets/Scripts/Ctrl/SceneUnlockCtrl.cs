using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class SceneUnlockCtrl : MonoBehaviour
{
    [SerializeField] Image[] mUnitImgs;
    [SerializeField] Sprite[] mUnitSprites;

    public int UnitCount => mUnitImgs.Length;

    public void UpdateUnitSprite(int targetIndex)
    {
        GameDefine.GameUtils.SotrArray(mUnitImgs);

        for (int i = 0; i < targetIndex; i++)
        {
            mUnitImgs[i].sprite = mUnitSprites[i];
            mUnitImgs[i].SetNativeSize();
        }
    }
}
