using DG.Tweening;
using GameDefine;
using QFramework;
using QFramework.Example;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static LevelCreateCtrl;

public class BottleCtrl : MonoBehaviour, IController, ICanSendEvent, ICanRegisterEvent
{
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    //  已完成、藤曼底座、遮挡布、藤曼、播放动画状态
    public bool isFinish, isFreeze, isClearHide, isNearHide, isPlayAnim;
    //  播放去除动画、正在解锁
    [SerializeField] private bool isClearHideAnim, hasUnlockHidePlayed = false;

    // 区分上下排
    public bool isUp;       

    // 瓶子编号和最大容量
    public int bottleIdx;   
    public int maxNum = 4,  
        limitColor = 0,     // 限制可倒入的颜色(0表示无限制)
        unlockClear = 0;    // 解锁魔法布的颜色编号
    // 用于接水次数计数
    // 修正（待确认）用于动画播放的锁问题
    private int ReceiveCount = 0;

    // 水块颜色、黑水块标志、每层水块的附加状态(结冰、破冰)、水块脚本引用
    public List<int> waters = new List<int>();
    public List<bool> hideWaters = new List<bool>();
    public List<WaterItem> waterItems = new List<WaterItem>();
    public List<BottleWaterCtrl> waterImg = new List<BottleWaterCtrl>();
    public List<int> bombCounts = new List<int>();

    // 操作记录(用于撤销功能)
    public List<BottleRecord> moveRecords = new List<BottleRecord>();

    // 水面动画位置节点(水面位置)、加水位置节点
    public List<Transform> spineNode = new List<Transform>();
    public List<Transform> waterNode = new List<Transform>();

    public Transform
        spineGo,      // 倒水过程水花动画父节点(当前水面位置)
        spineGoPosition, // 专门用于计算spine位置的替代品
        modelGo,      // 瓶子初始点位
        leftMovePlace,// 向该瓶子倒水时的目标位置 
        freezeGo;     // 藤曼底座节点  

    public Animator bottleAnim, fillWaterGoAnim;

    public SkeletonGraphic 
        spine,      // 倒水过程水花动画
        finishSpine,// 完成状态动画
        freezeSpine,// 藤曼底座动画
        bubbleSpine;// 气泡特效动画

    //           水柱顶部、   水柱底部、    容量1瓶子、   容量2的瓶子、 容量3的瓶子、   容量4的瓶子 
    public Image ImgWaterTop, ImgWaterDown, ImgBottleOne, ImgBottleTwo, ImgBottleThree, ImgBottleFour;

    public SkeletonGraphic 
        nearHide,           // 消除遮挡布动画
        clearHide,          // 消除藤蔓动画 
        limitColorSpine;    // 消除颜色限制动画

    // 完成状态动画对象
    public GameObject finishGo;  
   
    // 当前顶部水块的索引
    public int topIdx
    {
        get
        {
            // 通过列表长度动态计算
            return waters.Count - 1;
        }
    }

    public Button bottle;
    // 瓶子的初始属性配置
    BottleProperty originProperty;

    void Start()
    {
        bottle.onClick.AddListener(OnSelectedClick);

        //初始化瓶子位置
        this.RegisterEvent<GameStartEvent>(e =>
        {
            OnCancelSelect();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="property"></param>
    /// <param name="idx"></param>
    public void Init(BottleProperty property, int idx)
    {
        originProperty = property;
        
        ReceiveCount = 0;
        //配置初始容量
        //maxNum = property.numCake;

        isFinish = property.isFinish; 
        isClearHideAnim = false;
        finishGo.SetActive(isFinish);
        bubbleSpine.gameObject.SetActive(false);

        waters = new List<int>(property.waterSet);
        hideWaters = new List<bool>(property.isHide);
        waterItems = new List<WaterItem>(property.waterItem);
        bombCounts = new List<int>(property.bombCounts);

        // 对炸弹队列进行补录
        while (waters.Count != bombCounts.Count)
            bombCounts.Add(0);

        isClearHide = property.isClearHide;
        isNearHide = property.isNearHide;
        isFreeze = property.isFreeze;
        unlockClear = property.lockType;
        limitColor = property.limitColor;
        bottleIdx = idx;
        hasUnlockHidePlayed = false;

        // 针对炸弹是否存在，将对应的位置修改为炸弹。
        if (bombCounts.Count>0)
        {
            for (int i=0; i< bombCounts.Count;i++)
            {
                // 0 表示占位
                if (bombCounts[i]!=0)
                    waterItems[i] = WaterItem.Bomb;
            }

        }




        nearHide.gameObject.SetActive(isNearHide);
        if (nearHide)
        {
            nearHide.AnimationState.SetAnimation(0, "idle", true);
        }

        foreach (var bottle in waterImg)
        {
            bottle.waterImg.fillAmount = 1;
        }

        for (int i = 0; i < waters.Count; i++)
        {
            var color = waters[i];
            if (isClearHide || isNearHide || waterItems[i] == WaterItem.Ice)
            {
                LevelManager.Instance.cantChangeColorList.Add(color);
            }
        }

        if (topIdx < 0)
        {
            spineGo.gameObject.SetActive(false);
        }
        SetBottleColor(true, true);
        int spinePosIdx = topIdx + 1;
        SetNowSpinePos(spinePosIdx);
        //PlaySpineWaitAnim();//重复调用

        foreach (var item in waterItems)
        {
            if (item == WaterItem.Ice)
            {
                LevelManager.Instance.iceBottles.Add(this);
            }
        }
        //CheckFinish();

        freezeGo.gameObject.SetActive(isFreeze);
        //!isFinish针对回退时是否触发
        if (limitColor != 0 && !isFinish)
        {
            limitColorSpine.gameObject.SetActive(true);
            if (limitColor > 0 && limitColor < (int)EIdleAnim.IDLE_MAX)
            {
                limitColorSpine.AnimationState.SetAnimation(0, GameDefine.GameEnum.GetDescription<EIdleAnim>((EIdleAnim)limitColor), false);
            }
        }
        else
        {
            limitColorSpine.gameObject.SetActive(false);
        }


        if (isFreeze)
        {
            freezeSpine.AnimationState.SetAnimation(0, "idle", false);
        }

        for (int i = 0; i < hideWaters.Count; i++)
        {
            if (hideWaters[i])
            {
                LevelManager.Instance.hideBottleList.Add(this);
                break;
            }
        }

        SetMaxBottle();

       
    }

    /// <summary>
    /// 改变颜色
    /// </summary>
    /// <param name="from">被替换</param>
    /// <param name="to">替换</param>
    public void ChangeColor(int from, int to, Transform target)
    {
        for (int i = 0; i < waters.Count; i++)
        {
            if (waters[i] == from)
            {
                StartCoroutine(waterImg[i].ChangeShine());
                StartCoroutine(waterImg[i].ShowThunder(target));
            }
        }
        StartCoroutine(CheckChange(from, to, target));

    }

    /// <summary>
    /// 检测变色
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    IEnumerator CheckChange(int from, int to, Transform target)
    {
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < waters.Count; i++)
        {
            if (waters[i] == from)
            {
                waters[i] = to;
            }
        }  
       

        SetBottleColor();
        PlaySpineWaitAnim();
      
        CheckFinish();
       

        //会重复触发(暂弃)
        if (isFinish)
        {
            CheckFinishChange(to);
        }
    }

    public void CheckFailure()
    {
        // 炸弹是否爆炸判断
        CheckbombCountsDown();
    }

    /// <summary>
    /// 炸弹是否爆炸判断,炸弹的爆炸是优先的，所以计数+1
    /// </summary>
    public void CheckbombCountsDown()
    {
        var moveNum = LevelManager.Instance.moveNum;
        for (int i=0;i< bombCounts.Count; i++)
        {
          
            if (bombCounts[i]<moveNum+1&&bombCounts[i]!=0)
            {
                BomeIsBoom();
            }
        }
    }

    public void BomeIsBoom()
    {
        Debug.Log("炸弹爆炸");

    }

    /// <summary>
    /// 判断瓶子完成后的消除逻辑
    /// </summary>
    /// <param name="color"></param>
    public void CheckFinishChange(int color)
    {
        foreach (var bottle in LevelManager.Instance.nowBottles)
        {
            bottle.CheckUnlockHide(color);
        }
    }

    /// <summary>
    /// 移除黑色水块
    /// </summary>
    public void RemovHide()
    {
        for (int i = 0; i < hideWaters.Count; i++)
        {
            hideWaters[i] = false;
        }
        LevelManager.Instance.hideBottleList.Remove(this);

        SetBottleColor();
        CheckFinish();
    }

    /// <summary>
    /// 清除所有特殊情况
    /// </summary>
    /// <returns></returns>
    public void SetNormal()
    {
        for (int i = 0; i < hideWaters.Count; i++)
        {
            hideWaters[i] = false;
        }

        for (int i = 0; i < waterItems.Count; i++)
        {
            waterItems[i] = WaterItem.None;
        }

        if (isFreeze)
        {
            AudioKit.PlaySound("resources://Audio/ThornBase");
            freezeSpine.AnimationState.SetAnimation(0, "attack", false);
        }

        if (limitColor != 0 && !isFinish)
        {
            if (limitColor > 0 && limitColor < (int)ECombimeAnim.IDLE_MAX)
            {
                limitColorSpine.AnimationState.SetAnimation(0, GameEnum.GetDescription<ECombimeAnim>((ECombimeAnim)limitColor), false);
                limitColor = 0;
            }
        }

        isFreeze = false;
        isNearHide = false;
        isClearHide = false;
        StartCoroutine(CoroutinePlayNearHide(true));
        SetBottleColor(false, true);
        CheckFinish();
    }

    /// <summary>
    /// 增加颜色
    /// </summary>
    /// <returns></returns>
    public void AddColor(int color, Vector3 fromPos)
    {
        if (waters.Count < maxNum)
        {
            waters.Add(color);
            var fx = GameObject.Instantiate(LevelManager.Instance.createFx[color - 1], fromPos, Quaternion.identity);
            fx.transform.SetParent(LevelManager.Instance.gameCanvas);
            //Debug.Log("fx " + waterImg[topIdx].transform.name);
            var useIdx = topIdx;

            var tween = fx.transform.DOMove(waterNode[useIdx].transform.position, 1f);
            tween.OnComplete(() =>
            {
                Destroy(fx);
            })
            .OnUpdate(() =>
            {
                tween.SetTarget(waterNode[useIdx].transform.position);
            });

            waterItems.Add(WaterItem.None);
        }
    }

    public IEnumerator FinishHide()
    {
        isPlayAnim = true;
        yield return new WaitForSeconds(1);
        //Debug.Log("fx " + waters.Count + " " + name);
        CheckItem();
        SetBottleColor();
        CheckFinish();
        foreach (var item in waterImg)
        {
            item.waterImg.fillAmount = 1;
        }

        isPlayAnim = false;
    }

    private void OnSelectedClick()
    {
        if (!isPlayAnim && !LevelManager.Instance.isPlayFxAnim)
        {
            GameCtrl.Instance.OnSelect(this);
        }
    }

    /// <summary>
    /// 判断是否能选中 如果能 则选中
    /// </summary>
    /// <returns></returns>
    public bool OnSelect(bool needUp)
    {
        if ((isFreeze && needUp) || isClearHide || isNearHide || isFinish || isClearHideAnim || ReceiveCount != 0)
        {
            return false;
        }
        if (needUp)
        {
            modelGo.transform.DOLocalMoveY(modelGo.transform.localPosition.y + 100f, 2f / 30f);
        }
        return true;
    }

    /// <summary>
    /// 取消选中
    /// </summary>
    public void OnCancelSelect()
    {
        modelGo.transform.DOLocalMove(Vector3.zero, 2f / 30f);
    }

    /// <summary>
    /// 判断能否倒出
    /// </summary>
    /// <returns></returns>
    public bool CheckMoveOut()
    {
        if (topIdx < 0 || waterItems[topIdx] == WaterItem.Ice)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 取得倒出水的颜色
    /// </summary>
    /// <returns></returns>
    public int GetMoveOutTop()
    {
        if (topIdx < 0)
        {
            return 0;
        }
        return waters[topIdx];
    }

    /// <summary>
    /// 取得倒出水的对应WaterItem
    /// </summary>
    /// <returns></returns>
    public WaterItem GetMoveOutItemTop()
    {
        if (topIdx < 0)
        {
            return WaterItem.None;
        }
        return waterItems[topIdx];
    }

    public bool CheckMoveIn(int color)
    {
        if (topIdx < 0 && limitColor == 0 && !isClearHide)
        {
            return true;
        }

        var top = GetMoveOutTop();

        if (isClearHide || isNearHide || isFinish || GetLeftEmpty() == 0 || (limitColor != 0 && limitColor != color))
        {
            return false;
        }

        //color非道具
        if (color < 1000)
        {
            //color == top 且 top不为空
            if (color != top && top != 0)
            {
                return false;
            }
        }
        else
        {
            //判断自身顶部是否为道具 
            if (top > 1000)
            {
                //相同道具才可放置
                return top == color;
            }
            else
            {
                //color是道具，top不是道具
                return false;   
            }
        }

        return true;
    }

    /// <summary>
    /// 判断临近(荆棘)消除
    /// </summary>
    /// <param name="idx"></param>
    public void CheckNearHide(int idx)
    {
        if (Mathf.Abs(bottleIdx - idx) == 1 
            && LevelManager.Instance.nowBottles[idx].isUp == isUp
            && isNearHide)//只判定isNearHide的瓶子
        {
            foreach (var item in waters)
            {
                LevelManager.Instance.cantChangeColorList.Remove(item);
            }

            SetClearHide();
            CheckFinish();
            StartCoroutine(CoroutinePlayNearHide());
        }
    }

    /// <summary>
    /// 藤曼瓶消除动画表现相关
    /// </summary>
    /// <param name="nowait"></param>
    /// <returns></returns>
    IEnumerator CoroutinePlayNearHide(bool nowait = false)
    {
        if (!nowait)
        {
            yield return new WaitForSeconds(2f);
            AudioKit.PlaySound("resources://Audio/TengMan");
        }
        nearHide.AnimationState.SetAnimation(0, "attack", false);//jingji_xiaoshi
        yield return new WaitForSeconds(1.7f);
        nearHide.gameObject.SetActive(false);
        isNearHide = false;
    }

    /// <summary>
    /// 判断颜色解锁
    /// </summary>
    /// <param name="color"></param>
    public void CheckUnlockHide(int color)
    {
        if (isClearHide && !hasUnlockHidePlayed)
        {
            if (unlockClear == color)
            {
                hasUnlockHidePlayed = true;
                ++LevelManager.Instance.playingHideAnimCount;
                UIKit.OpenPanel<UIPropMask>(UILevel.PopUI);
                foreach (var item in waters)
                {
                    LevelManager.Instance.cantChangeColorList.Remove(item);
                }
                LevelManager.Instance.cantChangeColorList.Remove(color);
                StartCoroutine(HideClearHide());
            }
        }
    }

    /// <summary>
    /// 颜色解锁动画
    /// </summary>
    /// <returns></returns>
    IEnumerator HideClearHide()
    {
        isClearHideAnim = true;
        yield return new WaitForSeconds(1.5f);
        AudioKit.PlaySound("resources://Audio/MagicCloth");

        //加入事件
        TrackEntry trackEntry = null;
        if (unlockClear > 0 && unlockClear < (int)EDisapearAnim.IDLE_MAX)
        {
            trackEntry = clearHide.AnimationState.SetAnimation(0,GameEnum.GetDescription<EDisapearAnim>((EDisapearAnim)unlockClear),false);
        }

        if (trackEntry != null)
        {
            trackEntry.Complete += (entry) =>
            {
                clearHide.gameObject.SetActive(false);
                isClearHideAnim = false;
                --LevelManager.Instance.playingHideAnimCount;
                if (LevelManager.Instance.ISPlayingHideAnim)
                {
                    UIKit.ClosePanel<UIPropMask>();
                }
            };
        }

        isClearHide = false;
        CheckFinish();
    }

    /// <summary>
    /// 设置清除隐藏Spine动画
    /// </summary>
    void SetClearHide()
    {
        if (!isClearHideAnim)
        {
            clearHide.gameObject.SetActive(isClearHide);
            if (isClearHide)
            {
                if (unlockClear > 0 && unlockClear < (int)EIdleAnim.IDLE_MAX)
                {
                    clearHide.AnimationState.SetAnimation(0, GameEnum.GetDescription<EIdleAnim>((EIdleAnim)unlockClear), false);
                }
            }
        }

    }

    /// <summary>
    /// 获得剩余空位
    /// </summary>
    /// <returns></returns>
    public int GetLeftEmpty()
    {
        return maxNum - 1 - topIdx;
    }

    /// <summary>
    /// 倒水到另一个瓶子
    /// </summary>
    /// <param name="other"></param>
    public void MoveTo(BottleCtrl other)
    {
        int moveNum = other.GetLeftEmpty();
        int sameNum = 1;

        for (int i = topIdx - 1; i >= 0; i--)
        {
            if (waters[i] == GetMoveOutTop() && waterItems[i] != WaterItem.Ice)
            {
                sameNum++;
            }
            else
            {
                break;
            }
        }

        if (moveNum > sameNum)
        {
            moveNum = sameNum;
        }
       
        var color = GetMoveOutTop();
        MoveToOtherAnim(other, topIdx, moveNum, color);
        PlayOutAnim(moveNum, topIdx, color);

        for (int i = 0; i < moveNum; i++)
        { 
            int idx = topIdx; 
            // 将炸弹的计时一起传送
            int bombCount = bombCounts.Count>idx ? bombCounts[idx]: 0;
           
          
            other.ReceiveWater(color, GetMoveOutItemTop(), bombCount);
           
            if (waters.Count > 0)
            {
                waterImg[idx].wenhaoFxGo.SetActive(false);
                waterImg[idx].HideGo.SetActive(false);
                waters.RemoveAt(idx);
                waterItems.RemoveAt(idx);
                hideWaters.RemoveAt(idx);

                // 炸弹为空的时候，直接不进行移动
                if (bombCounts.Count>idx)
                    bombCounts.RemoveAt(idx);
            }
            GameCtrl.Instance.control = false;
        }

        //OnCancelSelect();
        other.PlayFillAnim(moveNum, color);
    }

    /// <summary>
    /// 接收水
    /// </summary>
    /// <param name="water"></param>
    /// <param name="item"></param>
    public void ReceiveWater(int water, WaterItem item,int bombCount)
    {
      
        if (water > 0)
        {
            waters.Add(water);
            waterItems.Add(item);
           
            bombCounts.Add(bombCount);
            hideWaters.Add(false);
        }
        CheckFinish();
    }

    /// <summary>
    /// 判断是否完成
    /// </summary>
    /// <param name="isChange"></param>
    public void CheckFinish(bool isChange = false)
    {

        if (topIdx > 0 && !isNearHide && !isClearHide && !isFinish)
        {
            var topColor = waters[topIdx];
            if (maxNum == 4 && topIdx == maxNum - 1)
            {
                for (int i = 3; i >= 0; i--)
                {
                    var water = waters[i];
                    if (water != topColor || waterItems[i] == WaterItem.Ice)
                    {
                        return;
                    }
                }

                OnFinish();
            }
        }

    }

    /// <summary>
    /// 完成后的处理
    /// </summary>
    public void OnFinish()
    {
        isFinish = true;
        LevelManager.Instance.FinishClear(GetMoveOutTop(), bottleIdx);
        StartCoroutine(ShowBreakIce());


        for (int i = 0; i < waterItems.Count; i++)
        {
            if (waterItems[i] == WaterItem.Bomb)
            {
               //  LevelManager.Instance.CancelBomb();弃用
                waterItems[i] = WaterItem.None;
                if(bombCounts.Count>i)
                    bombCounts[i] = 0;
            }
        }

        CheckWaterItem();
        StartCoroutine(ShowFinish());
    }

    /// <summary>
    /// 破冰动画
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowBreakIce()
    {
        yield return new WaitForSeconds(1f);
        for (int i = waterItems.Count - 1; i >= 0; i--)
        {
            if (waterItems[i] == WaterItem.BreakIce)
            {
                var breakTo = LevelManager.Instance.BreakIce();
                
                StartCoroutine(waterImg[i].BreakIce(breakTo));
                waterItems[i] = WaterItem.None;
                CheckWaterItem();
                
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    /// <summary>
    /// 完成动画
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowFinish()
    {
        var trackEntry = finishSpine.AnimationState.GetCurrent(0); // 获取轨道0上的当前动画条目
        if (trackEntry != null)
        {
            //trackEntry.TimeScale = 1f;
            finishSpine.Initialize(true);
        }
        else
        {
            // 如果当前没有动画，直接设置动画
            finishSpine.AnimationState.SetAnimation(0, "animation", false);
        }

        yield return new WaitForSeconds(0.8f);
        AudioKit.PlaySound("resources://Audio/Finish");
        StartCoroutine(PlayBottleCapSound());

        if (isFreeze)
        {
            AudioKit.PlaySound("resources://Audio/ThornBase");
            freezeSpine.AnimationState.SetAnimation(0, "attack", false);
        }

        yield return new WaitForSeconds(0.2f);
        finishGo.SetActive(isFinish);

        yield return new WaitForSeconds(1f);

        if (limitColor != 0)
        {
            if (limitColor > 0 && limitColor < (int)ECombimeAnim.IDLE_MAX)
            {
                limitColorSpine.AnimationState.SetAnimation(0, GameEnum.GetDescription<ECombimeAnim>((ECombimeAnim)limitColor), false);
            }
        }

        bubbleSpine.gameObject.SetActive(true);
        var trackEntry1 = bubbleSpine.AnimationState.GetCurrent(0); // 获取轨道0上的当前动画条目
        if (trackEntry1 != null)
        {
            //trackEntry.TimeScale = 1f;
            bubbleSpine.Initialize(true);
        }
        else
        {
            // 如果当前没有动画，直接设置动画
            bubbleSpine.AnimationState.SetAnimation(0, "maopao", false);
        }

    }

    /// <summary>
    /// 播放瓶盖声音
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayBottleCapSound()
    {
        yield return new WaitForSeconds(1f);
        AudioKit.PlaySound("resources://Audio/BottleCap");

    }

    /// <summary>
    /// 破冰
    /// </summary>
    public void UnlockIceWater()
    {
        CheckWaterItem();
        CheckFinish();
    }

    /// <summary>
    /// 找冰
    /// </summary>
    /// <returns></returns>
    public BottleWaterCtrl FindIceWater()
    {
        //从上往下找
        for (int i = waterItems.Count - 1; i >= 0; i--)
        {
            if (waterItems[i] == WaterItem.Ice)
            {
                waterItems[i] = WaterItem.None;
                return waterImg[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 判断黑色水块
    /// </summary>
    /// <param name="isFirst"></param>
    public void CheckHide(bool isFirst = false)
    {
        if (hideWaters.Count > waters.Count)
        {
            while (hideWaters.Count > waters.Count)
            {
                hideWaters.RemoveAt(hideWaters.Count - 1);
            }
        }
        else if (hideWaters.Count < waters.Count)
        {
            while (hideWaters.Count < waters.Count)
            {
                hideWaters.Add(false);
            }
        }

        if (!isFirst)
        {
            if (hideWaters.Count > 0 && waters.Count > 0)
            {
                //最上层的黑水块显示
                hideWaters[waters.Count - 1] = false;
                //黑水块的颜色与顶层是否相同(相同显示)
                for (int i = waters.Count - 1; i >= 0; i--)
                {
                    if ((topIdx >= 0) && (waters[i] == waters[topIdx]))
                    {
                        hideWaters[i] = false;
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }

        //判断该瓶子是否还存在黑水块
        bool hasHide = false;
        for (int i = 0; i < hideWaters.Count; i++)
        {
            if (hideWaters[i])
            {
                hasHide = true;
                break;
            }
        }

        if (!hasHide)
        {
            LevelManager.Instance.hideBottleList.Remove(this);
        }
    }

    /// <summary>
    /// 设置水块颜色
    /// </summary>
    /// <param name="isFirst"></param>
    /// <param name="nowaitHide"></param>
    public void SetBottleColor(bool isFirst = false, bool nowaitHide = false)
    {
        CheckHide(isFirst);

        //已完成，清除黑水块
        if (isFinish)
        {
            for (int i = 0; i < hideWaters.Count; i++)
            {
                hideWaters[i] = false;
            }
        }
        
        // 遍历每层水块，设置颜色和状态
        for (int i = 0; i < waters.Count; i++)
        {
            // 计算颜色索引，减一是因为颜色编号从 1 开始，而数组索引从 0 开始
            var useColor = waters[i] - 1;
            // 普通颜色水块
            if (useColor < 1000)
            {
                //Debug.Log("UseColor " + useColor);
                waterImg[i].SetColorState(ItemType.UseColor, LevelManager.Instance.waterColor[useColor], topIdx == i);
            }
            // 特殊道具水块
            else
            {
                // 根据道具类型设置对应的显示和动画
                waterImg[i].SetColorState((ItemType)waters[i], LevelManager.Instance.ItemColor, topIdx == i);
            }

            //将隐藏水块显示
            if (hideWaters.Count > 0)
            {
                SetHideShow(true, i);
            }
            waterImg[i].waterColor = useColor;
        }

        // 更新水块的显示状态
        for (int i = 0; i < waterImg.Count; i++)
        {
            //Debug.Log(name + "显示水 " + (i < waters.Count || waterImg[i].isPlayItemAnim) + " i " + i + "  waters.Count " + waters.Count + " isPlayItemAnim " + waterImg[i].isPlayItemAnim);
            waterImg[i].gameObject.SetActive(i < waters.Count || waterImg[i].isPlayItemAnim);
        }
        // 检查水块的道具状态
        CheckWaterItem();
        // 更新魔法布遮挡状态
        SetClearHide();

        // 更新水面位置
        int spinePosIdx = topIdx + 1;
        SetNowSpinePos(spinePosIdx);
        // 播放等待动画
        PlaySpineWaitAnim();
    }

    /// <summary>
    /// 设置隐藏水块显示
    /// </summary>
    /// <param name="nowaitHide">是否立即触发</param>
    /// <param name="idx"></param>
    public void SetHideShow(bool nowaitHide, int idx = -1)
    {
        if (idx >= 0)
        {
            if (hideWaters.Count > 0)
            {
                waterImg[idx].SetHide(hideWaters[idx], nowaitHide);
            }
        }
        else
        {
            for (int i = 0; i < waters.Count; i++)
            {
                if (hideWaters.Count > 0)
                {
                    waterImg[i].SetHide(hideWaters[i], nowaitHide);
                }
            }
        }
    }

    /// <summary>
    /// 星星特效去除黑水
    /// </summary>
    public void StarSetHideShow()
    {
        for (int i = 0; i < hideWaters.Count; i++)
        {
            if (hideWaters[i])
            {
                waterImg[i].PlayStarBlackWaterEffect();
                hideWaters[i] = false;
            }
        }
    }

    /// <summary>
    /// 判断水块道具
    /// </summary>
    public void CheckWaterItem()
    {
        var moveNum = LevelManager.Instance.moveNum;
        for (int i = 0; i < waterItems.Count; i++)
        {
            if (!waterImg[i].isPlayItemAnim)
            {
                waterImg[i].fireRuneGo.SetActive(false);
                waterImg[i].iceGo.SetActive(false);
            }
            switch (waterItems[i])
            {
                case WaterItem.None:
                    waterImg[i].textItem.text = "";
                    break;
                case WaterItem.Ice:
                    waterImg[i].iceGo.SetActive(true);
                    break;
                case WaterItem.Bomb:

                    // 设置时间
                    waterImg[i].textItem.text = bombCounts[i] - moveNum>0 && hideWaters[i]==false ?(bombCounts[i]- moveNum).ToString() :"";
                    break;
                case WaterItem.BreakIce:
                    waterImg[i].textItem.text = "";
                    waterImg[i].fireRuneGo.SetActive(true);
                    if (waters[i] > 0 && waters[i] < (int)EIdleAnim.IDLE_MAX)
                    {
                        waterImg[i].fireRuneSpine.AnimationState.SetAnimation(0, GameEnum.GetDescription<EIdleAnim>((EIdleAnim)waters[i]), false);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 接水动画
    /// </summary>
    /// <param name="num"></param>
    public void PlayFillAnim(int num, int color)
    {
        StartCoroutine(CoroutinePlayFillAnim(num, color));
    }

    IEnumerator CoroutinePlayFillAnim(int num, int color)
    {
        if (BottleHasItem())
            UIKit.OpenPanel<UIMask>(UILevel.PopUI);

        //isPlayAnim = true;
        ++ReceiveCount;

        float fillAlltime = 0.46f;
        yield return new WaitForSeconds(fillAlltime);
        SetBottleColor();
        //float fillAlltime = 1.33f;
        int startIdx = topIdx + 1 - num;
        if (color < 1000)
        {
            spineGo.gameObject.SetActive(true);
            SetNowSpinePos(startIdx);
            spineGoPosition.DOMove(spineNode[topIdx + 1].position, fillAlltime).SetEase(Ease.Linear);
        }
        else
        {
            if (startIdx >= 0)
            {
                SetNowSpinePos(startIdx);
            }
            //spineGo.transform.position = spineNode[topIdx + 1].position;
        }
        PlaySpineAnim();

        float fillTime = fillAlltime / num;
        if (color > 1000)
        {
            fillTime = 0.1f;
        }
        for (int i = 0; i < num; i++)
        {
            waterImg[startIdx + i].waterImg.fillAmount = 0;
        }

        for (int i = 0; i < num; i++)
        {
            waterImg[startIdx + i].PlayFillAnim(fillTime);
            yield return new WaitForSeconds(fillTime);
        }

        //isPlayAnim = false;
        --ReceiveCount;

        CheckItem();

        CheckFill();
    }

    /// <summary>
    /// 判断是否有水块
    /// </summary>
    void CheckFill()
    {
        for (int i = 0; i < waterImg.Count; i++)
        {
            if (waterImg.Count > i)
            {
                waterImg[i].waterImg.fillAmount = 1;
            }
            else
            {
                waterImg[i].waterImg.fillAmount = 0;
            }
        }
    }

    /// <summary>
    /// 播倒水动画
    /// </summary>
    public void PlaySpineAnim()
    {
        string spineAnimName = "";
        var color = GetMoveOutTop();

        if (color > 0 && color < (int)EDaoShuiAnim.IDLE_MAX)
            spineAnimName = GameEnum.GetDescription<EDaoShuiAnim>((EDaoShuiAnim)color);

        if (color < 1000 && color != 0)
        {
            spine.AnimationState.SetAnimation(0, spineAnimName, false);
        }
    }

    /// <summary>
    /// 入场动画
    /// </summary>
    public void PlaySpineWaitAnim(int useColor = -1)
    {
        string spineAnimName = "";
        int spinePosIdx = topIdx;

        if (topIdx >= 0)
        {
            for (int i = spinePosIdx; i >= 0; i--)
            {
                if (waters[spinePosIdx] < 1000)
                {
                    spinePosIdx = i;
                    break;
                }
            }

            var color = waters[spinePosIdx];
            if (useColor != -1)
            {
                color = useColor;
            }

            if (color > 0 && color < (int)ERuChangAnim.IDLE_MAX)
                spineAnimName = GameEnum.GetDescription<ERuChangAnim>((ERuChangAnim)color);
           
            if (color > 0)
            {
                if (color < 1000)
                {
                    spineGo.gameObject.SetActive(true);
                    spine.AnimationState.SetAnimation(0, spineAnimName, false);

                }
                else
                {
                    spineGo.gameObject.SetActive(false);
                }
            }
            else
            {
                spineGo.gameObject.SetActive(false);
            }

            if (hideWaters[spinePosIdx])
            {
                spine.AnimationState.SetAnimation(0, "ruchanghuangdong_mask", false);
            }
        }

        CheckHide();

    }

    /// <summary>
    /// 设置水面位置
    /// </summary>
    public void SetNowSpinePos(int node)
    {
        var useNode = node;
        //Debug.Log("当前节点 " + node);
        if (useNode - 1 < waters.Count)
        {
            for (int i = node - 1; i >= 0; i--)
            {
                if (waters[i] < 1000)
                {
                    useNode = i + 1;
                    break;
                }
            }
        }

        spineGoPosition.localPosition = spineNode[useNode].localPosition;
    }

    /// <summary>
    /// 水位上升/下降效果
    /// </summary>
    /// <param name="num"></param>
    public void PlayOutAnim(int num, int useIdx, int useColor)
    {
        StartCoroutine(CoroutinePlayOutAnim(num, useIdx, useColor));
    }

    IEnumerator CoroutinePlayOutAnim(int num, int useIdx, int useColor)
    {
        //yield return new WaitForSeconds(1f);
        float fillAlltime = 0.35f;
        yield return new WaitForSeconds(fillAlltime);
        //float fillAlltime = 1.33f;
        spineGo.gameObject.SetActive(true);
        int startIdx = useIdx;
        SetNowSpinePos(startIdx + 1);
        //Debug.Log("移动终点  " + useIdx + " " + num);
        if (useColor > 1000)
        {
            spineGoPosition.transform.localPosition = spineNode[useIdx + 1 - num].localPosition;
            if (topIdx < 0)
            {
                spineGo.gameObject.SetActive(false);
            }
        }
        else
        {
            spineGoPosition.DOLocalMove(spineNode[useIdx + 1 - num].localPosition, fillAlltime).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (topIdx < 0)
                {
                    spineGo.gameObject.SetActive(false);
                }
            });
        }

        PlaySpineWaitAnim(useColor);

        float fillTime = fillAlltime / num;
        if (useColor > 1000)
        {
            fillTime = 0f;
        }
        for (int i = 0; i < num; i++)
        {
            waterImg[startIdx - i].waterImg.fillAmount = 1;
        }

        for (int i = 0; i < num; i++)
        {
            waterImg[startIdx - i].PlayOutAnim(fillTime);
            yield return new WaitForSeconds(fillTime);
        }

        //倒水过程结束
        GameCtrl.Instance.ReducePouringCount();

        SetBottleColor();
    }

    public void MoveToOtherAnim(BottleCtrl other, int topIndex, int numWater, int useColor = -1)
    {
        Image bottleClickMask = bottle.GetComponent<Image>();

        //获取移动终点
        var (_targetPos, _dir) = GetMoveToPos(transform, other.transform ,other.leftMovePlace);

        isPlayAnim = true;
        var bottleRenderUpdate = bottleAnim.GetComponent<BottleRenderUpdate>();
        //水柱相关
        bottleRenderUpdate.SetMoveBottleRenderState(true, other);

        if (useColor < 1000)
        {
            topIndex += 1;
            //瓶身倾斜动画(BottleIn开始的Z轴大小要以BottleOut的结束为起点)
            //移动时长0.3f = 18帧.在19帧开始二次倾斜瓶子(19帧会播放水柱的动画)
            string bottleAnimName = $"BottleOut{topIndex}_{topIndex - numWater}{_dir}";
            bottleAnim.Play(bottleAnimName);
            //Debug.Log(bottleAnimName);
        }
        else
        {
            bottleAnim.Play($"BottleItemOut{_dir}");
        }

        //移动到目标点位(时长为18帧,19帧处开始二次倾斜)
        bottleClickMask.raycastTarget = false;
        modelGo.transform.DOMove(_targetPos, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetDownWaterSp(useColor);
            if (useColor < 1000) ////非道具动画播放
            {
                PlayWaterDown();
                // 因为先Play了动画片段在做的DoTween
                // 等待动画结束时机 = 动画实际播放时长(总帧/60帧s) * Exit Time - 0.3f(上方移动动画时长)
                // 方案一:修改动画总时长 = 移动时长(0.3f/18帧) + 水流时长(0.384f/23帧) 
                // 方案二:保持原55帧长度、将Exit Time调整为0.74618(以0.384f结束计算得到、缺点是二次倾斜没完全做完)    
                ActionKit.Delay(0.384f, () =>
                {
                    SetNowSpinePos(topIndex - numWater);
                    //回归原点(瓶子摆正动画需同步0.46f 约等于27帧)
                    modelGo.transform.DOLocalMove(Vector3.zero, 0.46f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        isPlayAnim = false;
                        bottleRenderUpdate.SetMoveBottleRenderState(false);
                        bottleClickMask.raycastTarget = true;
                        //回归原点时更新(此处调用则注释CoroutinePlayOutAnim 中的调用)
                        //SetBottleColor();
                    });
                }).Start(this);
            }
            else
            {
                //采用方案1(要调快调动画长度和等待时长)
                ActionKit.Delay(0.617f, () =>
                {
                    //回归原点
                    modelGo.transform.DOLocalMove(Vector3.zero, 0.46f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        isPlayAnim = false;
                        bottleRenderUpdate.SetMoveBottleRenderState(false);
                        bottleClickMask.raycastTarget = true;
                        //SetBottleColor();
                    });
                }).Start(this);
            }

        });
    }

    /// <summary>
    /// 获取移动方向和位置
    /// </summary>
    /// <param name="thisBottle"></param>
    /// <param name="targetBottle"></param>
    /// <param name="moveToTram"></param>
    /// <returns></returns>
    private (Vector3 pos, string dir) GetMoveToPos(Transform thisBottle ,Transform targetBottle ,Transform moveToTram)
    {
        // 要用本地坐标取镜像在转为世界坐标
        Vector3 _targetPos = moveToTram.localPosition;
        string _dir;

        var thisParent = thisBottle.parent;
        var targetParent = targetBottle.parent;
        bool isSameRow = thisParent == targetParent;
        int targetRowActiveCount = GetActiveSiblingCount(targetParent);

        // 同一排直接比较 postion.x
        if (isSameRow)
        {
            if (thisBottle.position.x >= targetBottle.position.x)
            {
                //Debug.Log("向左移动、取镜像");
                _targetPos.x *= -1f;
                _dir = "_Left";
            }
            else
            {
                //Debug.Log("向右移动、取原值");
                _dir = "_Right";
            }
        }
        // 不同排,采用左右各一半区分向左向右
        else
        {
            int activeCount = targetRowActiveCount;
            int targetIndex = targetBottle.GetSiblingIndex();
            int mid = activeCount / 2;

            if (targetIndex < mid)
            {
                //Debug.Log("向左移动，取镜像");
                _targetPos.x *= -1f;
                _dir = "_Left";
            }
            else
            {
                //Debug.Log("向右移动，取原值");
                _dir = "_Right";
            }
        }

        return (moveToTram.parent.TransformPoint(_targetPos), _dir);
    }

    private int GetActiveSiblingCount(Transform parent)
    {
        int count = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject.activeSelf)
                count++;
        }
        return count;
    }

    public void PlayWaterDown()
    {
        fillWaterGoAnim.Play("FillWater");
    }

    /// <summary>
    /// 设置水柱颜色
    /// </summary>
    /// <param name="useColor"></param>
    public void SetDownWaterSp(int useColor = -1)
    {
        var color = GetMoveOutTop();
        if (useColor != -1)
        {
            color = useColor;
        }

        if (color < 1000)
        {
            ImgWaterTop.sprite = LevelManager.Instance.waterTopSp[color - 1];
            ImgWaterDown.sprite = LevelManager.Instance.waterSp[color - 1];
        }
    }

    /// <summary>
    /// 检查本次倒水瓶子中是否有局内道具生效
    /// </summary>
    /// <returns></returns>
    public bool BottleHasItem()
    {
        //作用所有道具
        int itemId = 0;
        int itemPlace = 0;

        for (int i = 0; i < waters.Count; i++)
        {
            int waterColor = waters[i];

            if (waterColor > 1000)
            {
                if (itemId == 0)
                {
                    itemPlace = i;
                    itemId = waterColor;
                }
                else if (waterColor == itemId && i - itemPlace == 1)
                {
                    // 判断是否是符合条件的特殊道具
                    bool isMatch =
                        waterColor == (int)ItemType.ClearItem ||
                        waterColor == (int)ItemType.MakeColorItem ||
                        waterColor == (int)ItemType.MagnetItem ||
                        (waterColor > 2000 && waterColor < 3000) ||
                        waterColor > 3000;

                    if (isMatch)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 检查道具
    /// </summary>
    public void CheckItem()
    {
        int itemId = 0;
        int itemPlace = 0;
        List<int> items = new List<int>();


        for (int i = 0; i < waters.Count; i++)
        {
            var waterColor = waters[i];
            
            if (waterColor > 1000)
            {
                if (itemId == 0)
                {
                    itemPlace = i;
                    itemId = waterColor;
                }
                else
                {
                    if (waters[i] == itemId)
                    {
                        switch (waters[i])
                        {
                            case (int)ItemType.ClearItem:
                                if (itemId == waterColor && i - itemPlace == 1)
                                {
                                    items.Add(waters[i]);
                                    waterImg[i - 1].PlayUseBroom(waterImg[i]);
                                    //waterImg[i].gameObject.SetActive(true);
                                    //StartCoroutine(waterImg[i - 1].ShowBroomAfter());

                                    waters[i] = 0;
                                    waters[itemPlace] = 0;
                                }
                                break;
                            case (int)ItemType.MakeColorItem:
                                if (itemId == waterColor && i - itemPlace == 1)
                                {
                                    items.Add(waters[i]);
                                    waterImg[i - 1].PlayUseCreate(this, waterImg[i]);
                                    waters[i] = 0;
                                    waters[itemPlace] = 0;
                                    waterImg[i].color = new Color(1, 1, 1, 0);
                                    waterImg[i].broomItemGo.SetActive(false);
                                    waterImg[i].createItemGo.SetActive(false);
                                    waterImg[i].changeItemGo.SetActive(false);
                                    waterImg[i].magnetItemGo.SetActive(false);
                                }
                                break;
                            case (int)ItemType.MagnetItem:
                                if (itemId == waterColor && i - itemPlace == 1)
                                {
                                    items.Add(waters[i]);
                                    StringEventSystem.Global.Send("CacheMagnetWater", waterImg[i - 1]);
                                    waterImg[i - 1].PlayUseMagnet(waterImg[i]);
                                    waters[i] = 0;
                                    waters[itemPlace] = 0;
                                }
                                break;
                            default:
                                if (waters[i] > 2000 && waters[i] < 3000)
                                {
                                    if (itemId == waterColor && i - itemPlace == 1)
                                    {
                                        items.Add(waters[i]);
                                        waterImg[i - 1].PlayUseChange(waterImg[i]);
                                        waters[i] = 0;
                                        waters[itemPlace] = 0;
                                    }
                                }
                                else if (waters[i] > 3000)
                                {
                                    if (itemId == waterColor && i - itemPlace == 1)
                                    {
                                        items.Add(waters[i]);
                                        waterImg[i - 1].PlayUseBroom(waterImg[i]);
                                        //StartCoroutine(waterImg[i - 1].ShowBroomAfter());

                                        waters[i] = 0;
                                        waters[itemPlace] = 0;
                                    }
                                }

                                break;
                        }

                        itemId = 0;
                    }
                    else
                    {
                        itemPlace = i;
                        itemId = waterColor;
                    }
                }
            }
            else
            {
                itemId = 0;
            }
        }

        List<int> temp = new List<int>();
        List<WaterItem> tempItem = new List<WaterItem>();

        for (int i = 0; i < waters.Count; i++)
        {
            if (waters[i] != 0)
            {
                temp.Add(waters[i]);
                tempItem.Add(waterItems[i]);
            }
        }
        waters = temp;
        waterItems = tempItem;

        for (int i = 0; i < items.Count; i++)
        {
            int useItem = items[i];
            LevelManager.Instance.UseItem(useItem, waterImg[itemPlace].transform);
        }


        spineGo.gameObject.SetActive(topIdx >= 0);
        PlaySpineWaitAnim();
        SetNowSpinePos(topIdx + 1);

        if (items.Count > 0)
        {
            CheckHide();
            SetHideShow(false);
            SetNowSpinePos(topIdx + 1);
        }
    }

    /// <summary>
    /// 移除单色道具动画
    /// </summary>
    /// <param name="color"></param>
    /// <param name="fromPos"></param>
    public void PlayBroomBullet(int color, Vector3 fromPos)
    {
        List<BottleWaterCtrl> list = new List<BottleWaterCtrl>();
        for (int i = 0; i < waters.Count; i++)
        {
            if (waters[i] == color)
            {
                list.Add(waterImg[i]);
            }
        }

        foreach (var ctrl in list)
        {
            var go = Instantiate(LevelManager.Instance.broomBullet);
            
            var fly = go.GetComponent<FlyCtrl>();
            fly.target = ctrl.transform;
            fly.flyTime = 1.2f;
            go.transform.position = fromPos;
            fly.BeginFly();
        }
    }

    /// <summary>
    /// 移除单色
    /// </summary>
    /// <param name="color"></param>
    /// <param name="sameBottle">是否在一个瓶子</param>
    public void RemoveAllOneColor(int color,bool sameBottle)
    {
        List<int> list = new List<int>();
        List<WaterItem> items = new List<WaterItem>();
        List<bool> hides = new List<bool>();
        for (int i = 0; i < waters.Count; i++)
        {
            if (waters[i] == color)
            {
                StartCoroutine(PlayShine(i,sameBottle));
            }
            else
            {
                list.Add(waters[i]);
                items.Add(waterItems[i]);
                hides.Add(hideWaters[i]);
            }
        }

        waterItems = items;
        waters = list;
        hideWaters = hides;
    }

    /// <summary>
    /// 判断是否有要移除的单色
    /// </summary>
    /// <param name="color"></param>
    public BottleCtrl CheckRemoveOneColor(int color)
    {
        for (int i = 0; i < waters.Count; i++)
        {
            if (waters[i] == color)
                return this;
        }
        return null;
    }

    /// <summary>
    /// 移除时动画特效
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    IEnumerator PlayShine(int i, bool sameBottle)
    {
        isPlayAnim = true;
        var imgcmp = waterImg[i].transform.GetComponent<Image>();
        imgcmp.material = LevelManager.Instance.shineMaterial;
        StartCoroutine(waterImg[i].ShowBroomAfter());


        yield return new WaitForSeconds(2.2f);
        imgcmp.material = null;


        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        SetBottleColor();
        CheckItem();
        if (sameBottle)
        {
            //Debug.Log("在一个瓶子：" + sameBottle);
            isFinish = false;
            finishSpine.Hide();
        }
        //CheckFinish();//移除颜色不会触发完成

        if (topIdx < 0)
        {
            spineGo.gameObject.SetActive(false);
        }
        isPlayAnim = false;
    }

    /// <summary>
    /// 记录上一步
    /// </summary>
    public void RecordLast()
    {
        var record = new BottleRecord();
        record.isFinish = isFinish;
        record.isNearHide = isNearHide;
        record.isClearHide = isClearHide;
        record.isFreeze = isFreeze;
        record.limitColor = limitColor;
        record.waters = new List<int>(waters);
        record.hideWaters = new List<bool>(hideWaters);
        record.waterItems = new List<WaterItem>(waterItems);

        moveRecords.Add(record);
    }

    /// <summary>
    /// 返回上一步
    /// </summary>
    /// <returns></returns>
    public bool ReturnLast()
    {
        if (moveRecords.Count <= 0)
        {
            return false;
        }
        var record = moveRecords[moveRecords.Count - 1];

        var temp = new BottleProperty();

        temp.isFreeze = record.isFreeze;
        temp.isNearHide = record.isNearHide;
        temp.isClearHide = record.isClearHide;
        temp.isFinish = record.isFinish;
        temp.limitColor = record.limitColor;

        temp.waterSet = new List<int>(record.waters);
        temp.isHide = new List<bool>(record.hideWaters);
        temp.waterItem = new List<WaterItem>(record.waterItems);

        temp.numCake = originProperty.numCake;
        //temp.limitColor = originProperty.limitColor;
        temp.lockType = originProperty.lockType;

        Init(temp, bottleIdx);
        //isFinish = record.isFinish;

        moveRecords.Remove(record);
        finishGo.SetActive(isFinish);
        return true;
    }

    /// <summary>
    /// 设置瓶子最大装水数
    /// </summary>
    public void SetMaxBottle()
    {
        ImgBottleOne.gameObject.SetActive(maxNum == 1);
        ImgBottleTwo.gameObject.SetActive(maxNum == 2);
        ImgBottleThree.gameObject.SetActive(maxNum == 3);
        ImgBottleFour.gameObject.SetActive(maxNum == 4);
    }

    /// <summary>
    /// 打乱水块顺序
    /// </summary>
    public void RandomWater()
    {
        var numbers = Enumerable.Range(1, waters.Count).ToList();

        int n = numbers.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            int value = numbers[k];
            numbers[k] = numbers[n];
            numbers[n] = value;
        }

        bool checkSame = true;
        if (numbers[0] == 1)
        {
            for (int i = 0; i < waters.Count - 1; i++)
            {
                if (waters[i] != waters[numbers[i] - 1])
                {
                    if (waters[i] + 1 != waters[i + 1])
                    {
                        checkSame = false;
                        break;
                    }
                }
            }
        }
        else
        {
            checkSame = false;
        }


        if (checkSame)
        {
            RandomWater();
        }
        else
        {
            List<int> temp = new List<int>();
            for (int i = 0; i < waters.Count; i++)
            {
                temp.Add(waters[numbers[i] - 1]);
            }


            waters = temp;

            SetBottleColor();
        }
    }

    /// <summary>
    /// 道具显示材质切换
    /// </summary>
    public void ShowItemSelect()
    {
        ImgBottleOne.material = LevelManager.Instance.selectMaterial;
        ImgBottleTwo.material = LevelManager.Instance.selectMaterial;
        ImgBottleFour.material = LevelManager.Instance.selectMaterial;
        ImgBottleFour.material = LevelManager.Instance.selectMaterial;
    }

    /// <summary>
    /// 道具隐藏材质切换
    /// </summary>
    public void HideItemSelect()
    {
        ImgBottleOne.material = null;
        ImgBottleTwo.material = null;
        ImgBottleFour.material = null;
        ImgBottleFour.material = null;
    }
}
