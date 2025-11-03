using GameDefine;
using QFramework;
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

    //倒水计数，处于0表示当前不处于倒水过程
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
    /// 选中瓶子
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

            if (FirstBottle != null && SecondBottle != null)
            {
                control = true;
                if (FirstBottle.CheckMoveOut() && SecondBottle.CheckMoveIn(FirstBottle.GetMoveOutTop())
                    && !FirstBottle.isPlayAnim && !SecondBottle.isPlayAnim)
                {
                    //Debug.Log("移动 " + FirstCake.gameObject.name + "->" + SecondCake.gameObject.name);

                    ++pouringCount;
                    // 炸弹的判断优先于水瓶的内容，固将计数移动到前面

                    LevelManager.Instance.AddMoveNum();

                    // 炸弹更新并进行失败检测
                    bool flag = LevelManager.Instance.BombUpdate(bottle);
                    // 炸弹爆炸要中断去执行瓶子的相关事件和动画
                    if (flag == true)
                    {
                        control = false;
                        FirstBottle.OnCancelSelect();
                        InitPouringCount();
                        FirstBottle = null;
                        SecondBottle = null;
                        LevelManager.Instance.AddMoveNum(false);
                        return;
                    }
                    LevelManager.Instance.RecordLast();
                    FirstBottle.MoveTo(SecondBottle);
                    FirstBottle = null;
                    SecondBottle = null;
                    AudioKit.PlaySound("resources://Audio/PourWaterSound");
                    //LevelManager.Instance.AddMoveNum();//步数统计.暂时无用                
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
    /// 重置状态
    /// </summary>
    public void InitGameCtrl()
    {
        FirstBottle = null;
        SecondBottle = null;
        control = false;
    }

    /// <summary>
    /// 倒水状态完成
    /// </summary>
    public void ReducePouringCount()
    {
        --pouringCount;
        if (pouringCount < 0)
            pouringCount = 0;
    }

    /// <summary>
    /// 重置倒水状态
    /// </summary>
    public void InitPouringCount()
    {
        pouringCount = 0;
    }
}
