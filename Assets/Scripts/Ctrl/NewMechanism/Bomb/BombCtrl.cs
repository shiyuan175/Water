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
using UnityEditor.Rendering;
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
    SkeletonGraphic spine;
    [SerializeField]
    GameObject newMechine;
    [SerializeField]
    GameObject skeletonAnimation;
    [SerializeField]
    SkeletonDataAsset normalBomb;
    [SerializeField]
    SkeletonDataAsset flyBomb;
    float delayTime = 0.1f;
    // 炸弹爆炸，通过spineui和ani实现，ani负责动画渲染，ui复杂游戏渲染

    private void Awake()
    {
    }
    private void OnDisable()
    {
        bombSpine.SetActive(false);
    }
    public void BombBoom()
    {
        spine.skeletonDataAsset = normalBomb;
        spine.Initialize(true);
        timeText.text = "";
        Vector3 oldScale = bombSpine.LocalScale();    
        spine.skeletonDataAsset = normalBomb;
        bombSpine.SetActive(true);
        UIKit.OpenPanel<UIMask>();
        bombSpine.transform.DOScale(oldScale * 5, 0.3f).SetEase(Ease.OutQuad);

        TrackEntry track = spine.AnimationState.SetAnimation(0, "combine", false);
        track.Complete += track =>
        {
            if (!UIKit.GetPanel<UIRetry>())
                UIKit.OpenPanel<UIRetry>();

            UIKit.ClosePanel<UIMask>();
            LevelManager.Instance.haveBooming = false;
            bombSpine.SetActive(false);
            bombSpine.transform.localScale = oldScale;
        };
        Camera mainCamera = Camera.main;
        var animation = spine.SkeletonData.FindAnimation("combine");
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));
        Vector3 finalPosition = new Vector3(targetPosition.x, targetPosition.y, bombSpine.transform.position.z);
        bombSpine.transform.DOMove(finalPosition, 0.3f).SetEase(Ease.OutQuad);
    }
    




    public void BombIsFinish()
    {
        spine.skeletonDataAsset = normalBomb;
        spine.Initialize(true);
        /*  Debug.Log(spine);
        if (spine != null)
        {
            Debug.Log(message: "wqewqe");
            // ��ȡ SkeletonData
            var skeletonData = spine.skeletonDataAsset.GetSkeletonData(false);
            if (skeletonData != null)
            {
                // ��ȡ���ж����б�
                var animations = skeletonData.Animations;
                Debug.Log($" {animations.Count} ");

                foreach (var animation in animations)
                {
                    Debug.Log($": {animation.Name}, : {animation.Duration}");
                }
            }
            else
            {
                Debug.LogError(" SkeletonData");
            }
        }*/
       
        bombSpine.SetActive(value: true); 
        TrackEntry track = spine.AnimationState.SetAnimation(0, "bomp_remove", false);
        track.Complete += track =>
        {
            bombSpine.SetActive(false);
        };

    }

    public void BombFling()
    {
        Transform parentParent = transform.parent?.parent;
        timeText.text = ""; 
        // 复制当前的Spine对象
        GameObject bombCopy = Instantiate(bombSpine, parentParent);
        bombCopy.transform.localScale = Vector3.one;
        // 获取复制对象的Spine组件
        SkeletonGraphic spineCopy = bombCopy.GetComponent<SkeletonGraphic>();
        bombSpine.SetActive(false);
        
        Debug.Log(spineCopy);
        // 设置Spine数据并初始化
        spineCopy.skeletonDataAsset = flyBomb;
        spineCopy.Initialize(true);

        // 播放动画
        TrackEntry track = spineCopy.AnimationState.SetAnimation(0, "flap", false);
        track.Complete += track =>
        {
            // 动画完成后销毁复制的对象
            Destroy(bombCopy);
        };
        /*spine.skeletonDataAsset = flyBomb;
        spine.Initialize(true);
        bombSpine.SetActive(true);
        TrackEntry track = spine.AnimationState.SetAnimation(0, "flap", false);
        track.Complete += track =>
        {
            bombSpine.SetActive(false);
        };*/
    }
    
    public void SetBomb(bool isBomb = false, string time = "", string aniType = "combine")
    {
        spine.skeletonDataAsset = normalBomb;
        spine.Initialize(true);
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
        var currentTrackEntry = spine.AnimationState.GetCurrent(0);
        if (currentTrackEntry != null && (currentTrackEntry.Animation.Name == "combine"
            || currentTrackEntry.Animation.Name == "bomp_remove" || currentTrackEntry.Animation.Name == "flap"))
        {
            return;
        }

        bombSpine.SetActive(isBomb);
        timeText.text = time;
    }

}
