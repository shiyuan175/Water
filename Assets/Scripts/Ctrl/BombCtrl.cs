using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using QFramework;
using QFramework.Example;
using DG.Tweening;

public class BombCtrl : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI timeText;

    [SerializeField]
    GameObject bombSpine;
    [SerializeField]
    SkeletonGraphic skeletonGraphic;

    [SerializeField]
    GameObject skeletonAnimation;
    Transform originTransfomer;
 
    // 炸弹爆炸，通过spineui和ani实现，ani负责动画渲染，ui复杂游戏渲染
    public void BombBoom()
    {
       
        // UIKit.OpenPanel<UIRetry>();
        skeletonGraphic.AnimationState.Complete += CloseUI;
        // 可以优化回调为spine事件
   
        bombSpine.SetActive(true);
        skeletonAnimation.SetActive(true);
        skeletonAnimation.GetComponent<SkeletonAnimation>().GetComponent<MeshRenderer>().sortingOrder += 10;
        bombSpine.transform.localScale=new(0.1f,0.1f,0.1f);
        skeletonGraphic.AnimationState.SetAnimation(0, "attack", true);
        Camera mainCamera = Camera.main;
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));
        bombSpine.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        skeletonAnimation.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        Debug.Log("yesyesyes");

    }
    public void SetBomb(bool isBomb= false, string time="")
    {
        var currentTrackEntry = skeletonGraphic.AnimationState.GetCurrent(0);
        if (currentTrackEntry != null&& currentTrackEntry.Animation.Name == "attack")
        {
            
            Debug.Log(currentTrackEntry.Animation.Name);
            return;
        }
        Debug.Log("当前播放的动画名称: " + currentTrackEntry);
        bombSpine.SetActive(isBomb);
        timeText.text = time;
        skeletonGraphic.AnimationState.SetAnimation(0, "idle", false);


    }

    public void CloseUI(Spine.TrackEntry trackEntry)
    {
        Debug.Log("123");
        UIKit.OpenPanel<UIRetry>();
        skeletonGraphic.AnimationState.SetAnimation(0, "idle", false);
        bombSpine.SetActive(false);
        bombSpine.transform.localPosition = Vector3.zero;
        bombSpine.transform.localScale = new(0.8f, 0.8f, 0.8f);
        skeletonAnimation.transform.localPosition = Vector3.zero;
        skeletonGraphic.AnimationState.Complete -= CloseUI;
        

    }
}
