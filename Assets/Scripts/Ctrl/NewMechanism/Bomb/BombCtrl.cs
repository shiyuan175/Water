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
    GameObject newMechine;
    [SerializeField]
    GameObject skeletonAnimation;
    Transform originTransfomer;
    SkeletonAnimation skeletonAnimationCom;
    [SerializeField]
    SkeletonDataAsset normalBomb;
    [SerializeField]
    SkeletonDataAsset flyBomb;
    GameObject newAnimation;
    float delayTime = 0.1f;
    // 炸弹爆炸，通过spineui和ani实现，ani负责动画渲染，ui复杂游戏渲染

    private void Awake()
    {
        /*skeletonAnimationCom = skeletonAnimation.GetComponent<SkeletonAnimation>();*/

        /*newAnimation = Instantiate(skeletonAnimation,skeletonAnimation.transform);
        newAnimation.transform.SetParent(transform.parent); // true = 保持世界坐标
        newAnimation.SetActive(true);*/
    }
    private void OnDisable()
    {
        bombSpine.SetActive( false);
    }
    public void BombBoom()
    {
        newAnimation = Instantiate(skeletonAnimation, skeletonAnimation.transform);
        newAnimation.LocalScale(Vector3.one);
        newAnimation.transform.SetParent(transform.parent); // true = 保持世界坐标
        newAnimation.SetActive(true);
        skeletonAnimationCom = newAnimation.GetComponent<SkeletonAnimation>();
        /*skeletonAnimation.SetActive(true);*/
        UIKit.OpenPanel<UIMask>();
        TrackEntry track = skeletonAnimationCom.AnimationState.SetAnimation(0, "combine", false);
        track.Complete += track =>
        {
            if (!UIKit.GetPanel<UIRetry>())
                UIKit.OpenPanel<UIRetry>();
            UIKit.ClosePanel<UIMask>();

            skeletonAnimationCom.AnimationState.SetAnimation(0, "idle", true);

            bombSpine.transform.localPosition = Vector3.zero;

            /*skeletonAnimation.transform.localPosition = Vector3.zero;
            skeletonAnimation.SetActive(false);*/
            Destroy(newAnimation);
            LevelManager.Instance.haveBooming = false;
        };

        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        Camera mainCamera = Camera.main;
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));

        newAnimation.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        bombSpine.SetActive(false);
    }

    public void BombIsFinish()
    {

        newAnimation = Instantiate(skeletonAnimation, skeletonAnimation.transform);
        newAnimation.LocalScale(Vector3.one);
        newAnimation.transform.SetParent(transform.parent); // true = 保持世界坐标
        newAnimation.SetActive(true);
        skeletonAnimationCom = newAnimation.GetComponent<SkeletonAnimation>();
        /*  skeletonAnimation.SetActive(true);*/

        TrackEntry track = skeletonAnimationCom.AnimationState.SetAnimation(0, "bomp_remove", false);
        track.Complete += track =>
        {
            Destroy(newAnimation);
        };
        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        bombSpine.SetActive(false);
    }

    public void BombFling()
    {
        newAnimation = Instantiate(skeletonAnimation, skeletonAnimation.transform);
        newAnimation.LocalScale(Vector3.one);
        newAnimation.transform.SetParent(transform.parent.parent); // true = 保持世界坐标
        newAnimation.SetActive(true);
        skeletonAnimationCom = newAnimation.GetComponent<SkeletonAnimation>();
       /* Debug.Log(skeletonAnimationCom);
        Debug.Log(flyBomb);
        if (flyBomb != null)
        {
            Debug.Log("wqewqe");
            // 获取 SkeletonData
            var skeletonData = flyBomb.GetSkeletonData(false);
            if (skeletonData != null)
            {
                // 获取所有动画列表
                var animations = skeletonData.Animations;
                Debug.Log($"flyBomb 中共有 {animations.Count} 个动画:");

                foreach (var animation in animations)
                {
                    Debug.Log($"动画名称: {animation.Name}, 时长: {animation.Duration}秒");
                }
            }
            else
            {
                Debug.LogError("无法获取 SkeletonData");
            }
        }*/
        skeletonAnimationCom.skeletonDataAsset = flyBomb;
        skeletonAnimationCom.Initialize(true);
      
        TrackEntry track = skeletonAnimationCom.AnimationState.SetAnimation(0, "flap", false);
        track.Complete += track =>
        {
            Destroy(newAnimation);
        };
        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        bombSpine.SetActive(false);
    }
    
    public void SetBomb(bool isBomb = false, string time = "", string aniType = "combine")
    {
        
        if (aniType == "bomp_remove")
        {
            ActionKit.Delay(delayTime, () =>
            {
                BombIsFinish();

            }).Start(this);
         
            return;
        }
        if (aniType == "flap")
        {
            /*ActionKit.Delay(delayTime, () =>
            {
                BombFling();
            }).Start(this);*/
            BombFling();
            return;
        }
        var currentTrackEntry = skeletonGraphic.AnimationState.GetCurrent(0);
        if (currentTrackEntry != null && (currentTrackEntry.Animation.Name == "combine"
            || currentTrackEntry.Animation.Name == "bomp_remove" || currentTrackEntry.Animation.Name == "flap"))
        {
            return;
        }

        bombSpine.SetActive(isBomb);
        timeText.text = time;
        // skeletonGraphic.AnimationState.SetAnimation(0, "idle", false);
    }
   /* public void SetBomb(int bombCount)
    {
        switch(bombCount)
        {
            case BOMBING_SIGN:
                BombBoom();
                break;  
        }
            
    }*/
}
