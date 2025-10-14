using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameActivityStateCtrl : MonoBehaviour
{
    [SerializeField] private Sprite mUnlockSprite;
    [SerializeField] private Image mStateImg;
  
    public void ChangeIcon()
    {
        mStateImg.sprite = mUnlockSprite;
    }
}