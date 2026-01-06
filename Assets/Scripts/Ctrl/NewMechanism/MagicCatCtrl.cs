using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System.Security.Cryptography;
using GameDefine;
using QFramework;
using System.IO;

public class MagicCtrl : MonoBehaviour,ICanRegisterEvent
{
    [SerializeField]
    SkeletonDataAsset whiteCar;
    [SerializeField]
    SkeletonDataAsset blackCar;
    SkeletonGraphic spine;
    private const string IDLE_POSI = "animation_01";
    private const string WHITE_MOVE = "animation_2";
    private const string BLACK_MOVE = "animation_02";
    bool isWiterCar;
    public void Init(GlobalMechanism mechanism)
    {
      
        spine = GetComponent<SkeletonGraphic>();
 
        switch (mechanism)
        {
            case GlobalMechanism.WhiteMagicCar:
                spine.skeletonDataAsset = whiteCar;
                isWiterCar = true;
                break;
            case GlobalMechanism.BlackMagicCar:
                spine.skeletonDataAsset = blackCar;
                isWiterCar = false;
                break;
        }
        spine.enabled = true;
        spine.raycastTarget = false;
        spine.maskable = false;
        ShowIdleAnimation();
        

        StringEventSystem.Global.Register("MagicCatEven", ShowMoveAnimation);
    }
    
    public void OnDisable()
    {
        StringEventSystem.Global.UnRegister("MagicCatEven", ShowMoveAnimation);
    }
    //
    public void ShowIdleAnimation()
    {
        TrackEntry track = spine.AnimationState.SetAnimation(0, IDLE_POSI, true);
    }

    public void ShowMoveAnimation()
    {
        TrackEntry track;
        if (isWiterCar)
        { 
            track = spine.AnimationState.SetAnimation(0, WHITE_MOVE, false);
        }        
        else
        {
            track = spine.AnimationState.SetAnimation(0, BLACK_MOVE, false);
        }
        track.Complete+=  track=>
        {
            spine.AnimationState.SetAnimation(0, IDLE_POSI, true);
        };
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
