using DG.Tweening;
using QFramework.Example;
using QFramework;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 注释显示数字
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

    private const string NORMAl_APPEND = "animation_blue1";
    private const string NORMAL_DISABLE = "animation_blue2";
    private const string ORIGINAL_APPEND = "animation_purple1";
    private const string ORIGINAL_DISABLE = "animation_purple2";
    private void Awake()
    {
        skeletonAnimationCom = skeletonAnimation.GetComponent<SkeletonAnimation>();
    }
    /// <summary>
    /// 删除泡沐
    /// </summary>
    /// <param name="isOriginal">是否是原始泡沐</param>
    public void BubbleDead(bool isOriginal = false)
    {
        // 没有泡沐，不执行消失动画
        if (!skeletonAnimation.activeSelf)
            return;
        TrackEntry track;
        if (isOriginal)
        {
            track = skeletonAnimationCom.AnimationState.SetAnimation(0, ORIGINAL_DISABLE, false);
            skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        }
        else
        {
            track = skeletonAnimationCom.AnimationState.SetAnimation(0, NORMAL_DISABLE, false);
            skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        }

        track.Complete += track =>
        {
            skeletonAnimation.SetActive(false);
        };

        skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;

    
    }

    /// <summary>
    /// 生成泡沐
    /// </summary>
    /// <param name="time">计数</param>
    /// <param name="isOriginal"></param>
    public void BubbleAppend(bool isOriginal = false,int time=0)
    {
        // 动画
        if (skeletonAnimation.activeSelf)
            return;
        skeletonAnimation.SetActive(true);
        TrackEntry track;
        if (isOriginal)
        {
            track = skeletonAnimationCom.AnimationState.SetAnimation(0, ORIGINAL_APPEND, false);
            skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        }       
        else
        {
            track = skeletonAnimationCom.AnimationState.SetAnimation(0, NORMAl_APPEND, false);
            skeletonAnimationCom.GetComponent<MeshRenderer>().sortingOrder += 2;
        }
    }
}
