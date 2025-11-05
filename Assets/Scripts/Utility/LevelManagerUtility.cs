using GameDefine;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 该脚本的本意是将LevelManager中的一些计算逻辑移动出来，是的LevelManager的体量降低，变为较可维护
/// </summary>
public class LevelManagerUtility : IUtility
{
    private const int NORMALWATER_LIMITMAX = 1000;

    #region 随机生成黑水
    /// <summary>
    /// 随机选取一个水块进行黑水的变化 hidebottle 没有更新
    /// </summary>
    public BottleCtrl RandomBarkWaterBottle(List<BottleCtrl> bottles)
    {

        int i = UnityEngine.Random.Range(0, bottles.Count);
        int old = i;
        while (true)
        {

            BottleCtrl newBottleCtrl = bottles[i % bottles.Count];
            if (!BottleStateCheck(newBottleCtrl))
            {
                i++;
                continue;
            }
            BottleCtrl result = RandomBarkWater(newBottleCtrl);
            if (result != null)
                return result;
            else
                i++;
            if (old == i % bottles.Count)
                return null;
        }
    }
    private BottleCtrl RandomBarkWater(BottleCtrl bottleCtrl)
    {
        // 去掉最顶上
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {

            if (!WaterStateCheckForHide(bottleCtrl, i))
                continue;
            Debug.Log("adas");
            bottleCtrl.hideWaters[i] = true;
            return bottleCtrl;
        }
        for (int i = 0; i < random; i++)
        {

            if (!WaterStateCheckForHide(bottleCtrl, i))
                continue;
            Debug.Log("adas");
            bottleCtrl.hideWaters[i] = true;
            return bottleCtrl;

        }
        return null;
    }

    private bool WaterStateCheckForHide(BottleCtrl bottleCtrl, int index)
    {
        if (bottleCtrl.waterItems[index] != WaterItem.None)
            return false;
        if (bottleCtrl.hideWaters[index] != false)
            return false;
        if (bottleCtrl.waters[index] > NORMALWATER_LIMITMAX)
            return false;
        if (bottleCtrl.waters[index] == bottleCtrl.GetMoveOutTop())
        {
            bool _flag = false;
            for (int i = index+1; i < bottleCtrl.topIdx; i++)
            {
                if (bottleCtrl.waters[i] != bottleCtrl.GetMoveOutTop())
                    _flag = true;
            }
            return _flag;
        }

      
        return true;
    }

    #endregion
    #region 随机删除黑水--单格
    /// <summary>
    /// 随机选取一个水块进行黑水的变化 hidebottle 没有更新
    /// </summary>
    public BottleCtrl RandomRomveBarkWaterBottle(List<BottleCtrl> bottles)
    {

        int i = UnityEngine.Random.Range(0, bottles.Count);
        int old = i;
        while (true)
        {

            BottleCtrl newBottleCtrl = bottles[i % bottles.Count];
            if (!BottleStateCheck(newBottleCtrl))
            {
                i++;
                continue;
            }
            BottleCtrl result = RandomRomveBarkWater(newBottleCtrl);
            if (result != null)
                return result;
            else
                i++;
            if (old == i % bottles.Count)
                return null;
        }
    }
    private BottleCtrl RandomRomveBarkWater(BottleCtrl bottleCtrl)
    {
        // 去掉最顶上
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {
            if (!WaterStateCheckForClear(bottleCtrl, i))
                continue;

            bottleCtrl.hideWaters[i] = false;
            return bottleCtrl;
        }
        for (int i = 0; i < random; i++)
        {
            if (!WaterStateCheckForClear(bottleCtrl, i))
                continue;
            bottleCtrl.hideWaters[i] = false;
            return bottleCtrl;

        }
        return null;
    }
    private bool WaterStateCheckForClear(BottleCtrl bottleCtrl, int index)
    {
        if (bottleCtrl.hideWaters[index] != true)
            return false;
        return true;
    }
    #endregion
    #region 随机生成泡沐

    public bool RandomBubleWaterBottle(List<BottleCtrl> bottles)
    {
        int i = UnityEngine.Random.Range(0, bottles.Count);
        int old = i;
        while (true)
        {

            BottleCtrl newBottleCtrl = bottles[i % bottles.Count];

            if (!BottleStateCheck(newBottleCtrl))
            {
                i++;
                continue;
            }

            if (RandomBubbleWater(newBottleCtrl))
                return true;
            else
                i++;
            if (old == i % bottles.Count)
                return false;
        }
    }
    private bool RandomBubbleWater(BottleCtrl bottleCtrl)
    {
        // 去掉最顶上
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheckForBubble(bottleCtrl, i))
                continue;

            bottleCtrl.waterItems[i] = WaterItem.Bubble;
            bottleCtrl.waterImg[i].bubbleCtrl.BubbleAppend();
            return true;
        }
        for (int i = 0; i < random; i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheckForBubble(bottleCtrl, i))
                continue;

            bottleCtrl.waterItems[i] = WaterItem.Bubble;
            bottleCtrl.waterImg[i].bubbleCtrl.BubbleAppend();
            return true;
        }

        return false;
    }
    private bool WaterStateCheckForBubble(BottleCtrl bottleCtrl, int index)
    {
        return WaterStateCheckForHide(bottleCtrl, index);
    }

    #endregion

    private bool BottleStateCheck(BottleCtrl bottleCtrl)
    {
        return true;
    }

}
