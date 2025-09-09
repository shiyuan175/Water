using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class SceneUnlockCtrl : MonoBehaviour
{
    [SerializeField] Image mBgImg;
    [SerializeField] Image[] mUnitImgs;
    [SerializeField] Sprite[] mUnitSprites;

    public int UnitCount => mUnitImgs.Length;

    public void Awake()
    {
        GameDefine.GameUtils.SotrArray(mUnitImgs);
    }

    public void UpdateUnitSprite(int targetIndex)
    {
        for (int i = 0; i < targetIndex; i++)
        {
            mUnitImgs[i].sprite = mUnitSprites[i];
            if (mUnitImgs[i] != mBgImg)
                mUnitImgs[i].SetNativeSize();
        }
    }
}
