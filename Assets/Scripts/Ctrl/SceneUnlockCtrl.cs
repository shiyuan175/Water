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

    private bool mFirstStart = true;

    public int UnitCount => mUnitImgs.Length;

    public void UpdateUnitSprite(int targetIndex)
    {
        if (mFirstStart)
        {
            GameDefine.GameUtils.SotrArray(mUnitImgs);
            mFirstStart = false;
        }

        for (int i = 0; i < targetIndex; i++)
        {
            mUnitImgs[i].sprite = mUnitSprites[i];
            if (mUnitImgs[i] != mBgImg)
                mUnitImgs[i].SetNativeSize();
        }
    }
}
