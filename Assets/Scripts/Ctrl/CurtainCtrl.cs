using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;
using Spine;

public class CurtainCtrl : MonoBehaviour
{
    [SerializeField]
    GameObject spineGo;
    [SerializeField]
    SkeletonGraphic spine;

    private string[] animationNamess =
    {
        "animation_4",
        "animation_3",
        "animation_2",
        "animation_1"
    };

    private void OnDisable()
    {
        // 回归默认状态
        spineGo.SetActive(false);
        /*        spine.Skeleton.SetToSetupPose();*/
    }

    public void SetCurtain(int stage)
    {
        if (stage < 0)
            stage = 0;
        spine.AnimationState.SetAnimation(0, animationNamess[stage], false);
        // 是否要disable
    }
    public void ClearCurtain()
    {
        spineGo.SetActive(false);
    }
    public void InitCurtain(int stage)
    {
        //　初始状态
        spineGo.SetActive(true);
        TrackEntry trackEntry;
        // 最顶层不需要用动画切换状态
        if (stage == animationNamess.Count())
        {
            trackEntry = spine.AnimationState.SetAnimation(0, animationNamess[stage - 1], false);
            trackEntry.TimeScale = 0;  // 关键：时间缩放为0，完全暂停
            trackEntry.Loop = false;   // 不循环
            return;
        }

        // 强制设置spine播放的状态
        trackEntry = spine.AnimationState.SetAnimation(0, animationNamess[stage], false);
        trackEntry.TrackTime = trackEntry.AnimationEnd;
        spine.Update(0);
    }
}
