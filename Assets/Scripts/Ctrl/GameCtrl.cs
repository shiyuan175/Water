using GameDefine;
using QFramework;
using QFramework.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCtrl : MonoBehaviour, ICanSendEvent
{
    public static GameCtrl Instance;
    public BottleCtrl FirstBottle, SecondBottle;

    public bool control = false;
    //倒水量计数器，初始为0，用于控制倒水动画
    private int pouringCount = 0;
    public bool IsPouring => pouringCount == 0;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //Application.targetFrameRate = 5;
    }

    /// <summary>
    /// 选中瓶子的处理逻辑
    /// </summary>
    /// <param name="bottle"></param>
    public void OnSelect(BottleCtrl bottle)
    {
        if (!control)
        {
            if (FirstBottle == null)
            {
                if (bottle.OnSelect(true))
                {
                    AudioKit.PlaySound("resources://Audio/SelectBottle");
                    FirstBottle = bottle;
                    
                }

            }
            else if (SecondBottle == null)
            {
                if (bottle != FirstBottle)// && bottle.OnSelect(false)
                {
                    SecondBottle = bottle;
                }
                else
                {
                    FirstBottle.OnCancelSelect();
                    FirstBottle = null;
                }
            }
            // 检查两个瓶子是否都已选中
            if (FirstBottle != null && SecondBottle != null)
            {
                control = true;
                if (FirstBottle.CheckMoveOut() && SecondBottle.CheckMoveIn(FirstBottle.GetMoveOutTop())
                    && !FirstBottle.isPlayAnim && !SecondBottle.isPlayAnim)
                {
                    //Debug.Log("??? " + FirstCake.gameObject.name + "->" + SecondCake.gameObject.name);

                    ++pouringCount;
                    // 记录步数

                    LevelManager.Instance.AddMoveNum();              
                    
                    #region 炸弹检测和相关UI显示
                    #region 炸弹检测--是否触发爆炸
                    // 检查是否触发炸弹爆炸
                    bool flag = LevelManager.Instance.CheckBomb();
                    // 如果触发炸弹爆炸则取消本次操作
                    if (flag == true)
                    {
                        control = false;
                        FirstBottle.OnCancelSelect();
                        //???flag?????????UI???
                        InitPouringCount();
                        FirstBottle = null;
                        SecondBottle = null;
                        LevelManager.Instance.AddMoveNum(false);
                        return;
                    }
                    #endregion
                    #endregion

                    LevelManager.Instance.RecordLast();
                    FirstBottle.MoveTo(SecondBottle);
                    FirstBottle = null;
                    SecondBottle = null;
                    AudioKit.PlaySound("resources://Audio/PourWaterSound");
                }
                else
                {
                    control = false;
                    FirstBottle.OnCancelSelect();
                    FirstBottle = null;
                    SecondBottle = null;
                }
            }
        }
    }

    /// <summary>
    /// 初始化游戏控制器
    /// </summary>
    public void InitGameCtrl()
    {
        FirstBottle = null;
        SecondBottle = null;
        control = false;
    }

    /// <summary>
    /// 减少倒水量计数器
    /// </summary>
    public void ReducePouringCount()
    {
        --pouringCount;
        if (pouringCount < 0)
            pouringCount = 0;
    }

    /// <summary>
    /// 重置倒水量计数器
    /// </summary>
    public void InitPouringCount()
    {
        pouringCount = 0;
    }
}