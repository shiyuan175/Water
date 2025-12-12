using DG.Tweening;
using QFramework.Example;
using QFramework;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ע����ʾ����
public class BubbleCtrl : MonoBehaviour
{
    [SerializeField] SkeletonGraphic spine;

    // ͨ��spineui��aniʵ�֣�ani���𶯻���Ⱦ��ui������Ϸ��Ⱦ

    private const string NORMAl_APPEND = "animation_blue1";
    private const string NORMAL_DISABLE = "animation_blue2";
    private const string ORIGINAL_APPEND = "animation_purple1";
    private const string ORIGINAL_DISABLE = "animation_purple2";

    private void OnDisable()
    {
        spine.enabled = false;
    }

    /// <summary>
    ///     ɾ������
    /// </summary>
    /// <param name="isOriginal">�Ƿ���ԭʼ����</param>
    public void BubbleDead(bool isOriginal = false)
    {
        // û�����壬��ִ����ʧ����
        if (!spine.enabled)
            return;
        TrackEntry track;
        if (isOriginal)
        {
            track = spine.AnimationState.SetAnimation(0, ORIGINAL_DISABLE, false);
        }
        else
        {
            track = spine.AnimationState.SetAnimation(0, NORMAL_DISABLE, false);
        }

        track.TimeScale = 1.7f;
        track.Complete += track => { spine.enabled = false; };
    }

    /// <summary>
    ///     ��������
    /// </summary>
    /// <param name="time">����</param>
    /// <param name="isOriginal"></param>
    public void BubbleAppend(bool isOriginal = false, int time = 0)
    {
        // ����
        if (spine.enabled)
            return;
        spine.enabled = true;
        TrackEntry track;
        /*if (isOriginal)
        {
            track = spine.AnimationState.SetAnimation(0, ORIGINAL_APPEND, false);
        }
        else
        {
            track = spine.AnimationState.SetAnimation(0, NORMAl_APPEND, false);

        }
        track.TimeScale = 1.7f;*/
        if (isOriginal)
        {
            track = spine.AnimationState.SetAnimation(0, ORIGINAL_DISABLE, false);
        }
        else
        {
            track = spine.AnimationState.SetAnimation(0, NORMAL_DISABLE, false);
        }

        track.TimeScale = 0;
    }
}