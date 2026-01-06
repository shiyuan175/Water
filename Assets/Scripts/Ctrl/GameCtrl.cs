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

    //倒水计数，处于0表示当前不处于倒水过程
    [SerializeField] private int pouringCount = 0;
    public bool IsPouring => pouringCount == 0;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    private void Awake()
    {
        Debug.Log("GameCtrl");
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

            // 倒水
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

                    #region 倒水前触发、玩家走一步触发、倒水前全局游戏机制，

                    #region 会中止逻辑，需要重新刷新UI

                    #region 炸弹机制--失败检查

                    // 炸弹更新并进行失败检测
                    bool flag = LevelManager.Instance.CheckBomb();
                    // 炸弹爆炸要中断去执行瓶子的相关事件和动画
                    if (flag == true)
                    {
                        control = false;
                        FirstBottle.OnCancelSelect();
                        //是否将flag作为需要刷新UI标记
                        InitPouringCount();
                        FirstBottle = null;
                        SecondBottle = null;
                        LevelManager.Instance.AddMoveNum(false);
                        return;
                    }
                    #endregion

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
