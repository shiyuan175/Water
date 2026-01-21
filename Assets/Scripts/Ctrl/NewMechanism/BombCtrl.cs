using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using QFramework;
using QFramework.Example;
using DG.Tweening;
using Spine;
/// <summary>
/// 炸弹标记 0 表示没有，100表示正常消失 ，200 表示飞天消失
/// </summary>
public class BombCtrl : MonoBehaviour, ICanSendEvent, ICanGetUtility
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
    private void OnEnable()
    {
        spine.AnimationState?.ClearTracks();
        spine.skeletonDataAsset = null;
        spine.Initialize(true);
    }
    public void BombBoom()
    {
        Transform parentParent = LevelManager.Instance.mSpineIniPar;
        spine.skeletonDataAsset = normalBomb;
        spine.Initialize(true);
        bombSpine.SetActive(false);
        timeText.text = "";
        
        GameObject bombCopy = Instantiate(bombSpine, bombSpine.transform.parent);
        bombCopy.SetActive(true);

        Camera mainCamera = Camera.main;
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));
        Vector3 finalPosition = new Vector3(targetPosition.x, targetPosition.y, bombCopy.transform.position.z);

        SkeletonGraphic spineCopy = bombCopy.GetComponent<SkeletonGraphic>();
        UIKit.OpenPanel<UIMask>();
        TrackEntry track = spineCopy.AnimationState.SetAnimation(0, "combine", false);
        track.Complete += track =>
        {
            /*if (!UIKit.GetPanel<UIRetry>())
                UIKit.OpenPanel<UIRetry>();*/
           
            LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetCurrentLevel());
            
            UIKit.ClosePanel<UIMask>();
            spine.enabled = false;
            spine.AnimationState?.ClearTracks();
            spine.skeletonDataAsset = null;
            spine.Initialize(true);
            bombSpine.SetActive(false);
        }; 

        bombCopy.transform.DOScale(bombCopy.transform.localScale * 5, 0.3f).SetEase(Ease.OutQuad);
        bombCopy.transform.DOMove(finalPosition, 0.3f).SetEase(Ease.OutQuad)
          .OnComplete(() =>
          {
              // 记录当前的世界坐标
              Vector3 worldPosition = bombCopy.transform.position;
              Vector3 worldScale = bombCopy.transform.lossyScale;

              // 改变父对象
              bombCopy.transform.SetParent(parentParent);

              // 恢复世界坐标和缩放
              bombCopy.transform.position = worldPosition;

              // 如果需要保持缩放，可以这样设置
              // bombCopy.transform.localScale = worldScale;
              // 或者重置为合适的本地缩放
              /*bombCopy.transform.localScale = Vector3.one;*/
              
          });
           
    }

    public void BombIsFinish()
    {
        spine.skeletonDataAsset = normalBomb;
        spine.Initialize(true);
       
        bombSpine.SetActive( true); 
        TrackEntry track = spine.AnimationState.SetAnimation(0, "bomp_remove", false);
        track.Complete += track =>
        {
            bombSpine.SetActive(false);
            spine.enabled = false;
            spine.AnimationState?.ClearTracks();
            spine.skeletonDataAsset = null;
            spine.Initialize(true);
        };

    }

    public void BombFling()
    {
        Transform parentParent = transform.parent?.parent.parent;
        timeText.text = ""; 
        // 复制当前的Spine对象
        GameObject bombCopy = Instantiate(bombSpine, parentParent);
        bombCopy.transform.localScale = Vector3.one;
        // 获取复制对象的Spine组件
        SkeletonGraphic spineCopy = bombCopy.GetComponent<SkeletonGraphic>();
        bombSpine.SetActive(false);

        // 设置Spine数据并初始化
        spineCopy.skeletonDataAsset = flyBomb;
        spineCopy.Initialize(true);

        // 播放动画
        TrackEntry track = spineCopy.AnimationState.SetAnimation(0, "flap", false);
        track.Complete += track =>
        {
            bombSpine.SetActive(false);
            spine.enabled = false;
            spine.AnimationState?.ClearTracks();
            spine.skeletonDataAsset = null;
            spine.Initialize(true);
            // 动画完成后销毁复制的对象
            Destroy(bombCopy);
        };
    }
    
    public void SetBomb(bool isBomb = false, string time = "", string aniType = "combine",bool isFly = false)
    {

        bombSpine.SetActive(true);
        timeText.text = time;
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
            BombFling();
            return;
        }
        var currentTrackEntry = spine.AnimationState?.GetCurrent(0);
        if (currentTrackEntry != null && (currentTrackEntry.Animation.Name == "combine"
            || currentTrackEntry.Animation.Name == "bomp_remove" || currentTrackEntry.Animation.Name == "flap"
            || currentTrackEntry.Animation.Name == "idle")) 
        {
            return;
        }
        if(isFly)
            spine.skeletonDataAsset = flyBomb;
        else
            spine.skeletonDataAsset = normalBomb;
        timeText.text = time;
        spine.enabled = isBomb;
        spine.Initialize(true);
        bombSpine.SetActive(isBomb);
        spine.AnimationState.SetAnimation(0,"idle",true);
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
