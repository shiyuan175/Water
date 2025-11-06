using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using QFramework;
using QFramework.Example;
using DG.Tweening;
using Spine;
using UnityEngine.UI;
/// <summary>
/// 炸弹标记 0 表示没有，100表示正常消失 ，200 表示飞天消失
/// </summary>
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
    SkeletonAnimation skeletonAnimationCom;
    [SerializeField]
    SkeletonDataAsset normalBomb;
    [SerializeField]
    SkeletonDataAsset flyBomb;
    // 炸弹爆炸，通过spineui和ani实现，ani负责动画渲染，ui复杂游戏渲染

    private void Awake()
    {
        skeletonAnimationCom = skeletonAnimation.GetComponent<SkeletonAnimation>();
    }

    public void BombBoom()
    {
        
        skeletonAnimation.SetActive(true);
        UIKit.OpenPanel<UIMask>();
        TrackEntry track = skeletonAnimationCom.AnimationState.SetAnimation(0, "combine", false);
        track.Complete += track =>
        {
            if(!UIKit.GetPanel<UIRetry>())
                UIKit.OpenPanel<UIRetry>();
            UIKit.ClosePanel<UIMask>();

            skeletonAnimationCom.AnimationState.SetAnimation(0, "idle",true);

            bombSpine.transform.localPosition = Vector3.zero;

            skeletonAnimation.transform.localPosition = Vector3.zero;
            skeletonAnimation.SetActive(false);
        };
        
        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        Camera mainCamera = Camera.main;
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));
       
        skeletonAnimation.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        bombSpine.SetActive(false);
    }

    public void BombIsFinish()
    {       
        skeletonAnimation.SetActive(true);
        
        skeletonAnimationCom.AnimationState.SetAnimation(0, "bomp_remove", false);
        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        bombSpine.SetActive(false);
    } 

    public void BombFling()
    {
        skeletonAnimationCom.skeletonDataAsset = flyBomb;
        skeletonAnimation.SetActive(true);
        skeletonAnimationCom.AnimationState.SetAnimation(0, "flap", false);
        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        bombSpine.SetActive(false);
    }
    public void SetBomb(bool isBomb = false, string time = "", string aniType = "combine")
    {
        if (aniType == "bomp_remove")
        {
            BombIsFinish();
            return;
        }
        if(aniType == "flap")
        {
            BombFling();
            return;
        }
        var currentTrackEntry = skeletonGraphic.AnimationState.GetCurrent(0);
        if (currentTrackEntry != null && (currentTrackEntry.Animation.Name == "combine"
            || currentTrackEntry.Animation.Name == "bomp_remove" || currentTrackEntry.Animation.Name == "flap") )
        {
             return;
        }
    
        bombSpine.SetActive(isBomb);
        timeText.text = time;
        // skeletonGraphic.AnimationState.SetAnimation(0, "idle", false);
    }
}
