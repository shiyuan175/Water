using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameActivityStateCtrl : MonoBehaviour, ICanGetUtility
{
    [SerializeField] private int mUnlockLevel;
    [SerializeField] private Sprite mUnlockSprite;
    [SerializeField] private Image mStateImg;
    
    private SaveDataUtility mSaveDataUtility;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    private void Awake()
    {
        mSaveDataUtility = this.GetUtility<SaveDataUtility>();
    }

    private void OnEnable()
    {
        var _curLevel = mSaveDataUtility.GetCurrentLevel();
        if (_curLevel >= mUnlockLevel && mStateImg.sprite != mUnlockSprite)
            mStateImg.sprite = mUnlockSprite;
    }
}