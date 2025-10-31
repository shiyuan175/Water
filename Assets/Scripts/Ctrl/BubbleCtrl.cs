using DG.Tweening;
using QFramework.Example;
using QFramework;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BubbleCtrl : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI timeText;

    [SerializeField]
    GameObject bubbleSpine;
    [SerializeField]
    SkeletonGraphic skeletonGraphic;

    [SerializeField]
    GameObject skeletonAnimation;
    SkeletonAnimation skeletonAnimationCom;
    // 通过spineui和ani实现，ani负责动画渲染，ui复杂游戏渲染

    private void Awake()
    {
        skeletonAnimationCom = skeletonAnimation.GetComponent<SkeletonAnimation>();
    }

    public void BubbleDead()
    {
        // 关闭UI显示，展东动画
        bubbleSpine.SetActive(false);
        skeletonAnimation.SetActive(true);

        UIKit.OpenPanel<UIMask>();
        TrackEntry track = skeletonAnimationCom.AnimationState.SetAnimation(0, "combine", false);
        track.Complete += track =>
        {
            skeletonAnimationCom.AnimationState.SetAnimation(0, "idle", false);
            skeletonAnimation.SetActive(false);
        };

        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;

        bubbleSpine.SetActive(false);
    }
    public void BubbleAppend()
    {
        bubbleSpine.SetActive(true);
        skeletonAnimation.SetActive(false);
        // 动画
    }
}
