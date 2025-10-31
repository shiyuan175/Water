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
    #region 生成黑水
    /// <summary>
    /// 随机选取一个水块进行黑水的变化
    /// </summary>
    /// <param name="expect">期望的黑水结果</param>
    public GameObject RandomBarkWaterBottle(List<BottleCtrl> bottles, bool expect)
    {
        int i = UnityEngine.Random.Range(0, bottles.Count);
        while (true)
        {

            int old = i;
            BottleCtrl newBottleCtrl = bottles[i % bottles.Count];
            if (!BottleStateCheck(newBottleCtrl))
            {
                i++;
                continue;
            }


            if (RandomBarkWater(newBottleCtrl, expect) != null)
                return RandomBarkWater(newBottleCtrl, expect);
            else
                i++;
            if (old == i % bottles.Count)
                return null;
        }

    }
    public GameObject RandomBarkWater(BottleCtrl bottleCtrl, bool expect)
    {
        // 去掉最顶上
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheckForHide(bottleCtrl, i))
                continue;
            if (bottleCtrl.hideWaters[i] != expect)
            {
                bottleCtrl.hideWaters[i] = expect;
                return bottleCtrl.gameObject;
            }
        }
        for (int i = 0; i < random; i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheckForHide(bottleCtrl, i))
                continue;
            bottleCtrl.hideWaters[i] = expect;
            return bottleCtrl.gameObject;

        }

        return null;
    }
    public bool WaterStateCheckForHide(BottleCtrl bottleCtrl, int index)
    {
        if (bottleCtrl.waterItems[index] != WaterItem.None)
            return false;
        if (bottleCtrl.hideWaters[index] != true)
            return false;
        if (bottleCtrl.waters[index] <= NORMALWATER_LIMITMAX)
            return false;
        return true;
    }

    #endregion

    #region 生成泡沐

    public GameObject RandomBubleWaterBottle(List<BottleCtrl> bottles, bool expect)
    {
        int i = UnityEngine.Random.Range(0, bottles.Count);
        while (true)
        {

            int old = i;
            BottleCtrl newBottleCtrl = bottles[i % bottles.Count];
            if (!BottleStateCheck(newBottleCtrl))
            {
                i++;
                continue;
            }

            if (RandomBubbleWater(newBottleCtrl, expect) != null)
                return RandomBubbleWater(newBottleCtrl, expect);
            else
                i++;
            if (old == i % bottles.Count)
                return null;
        }
    }
    public GameObject RandomBubbleWater(BottleCtrl bottleCtrl, bool expect)
    {
        // 去掉最顶上
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheckForBubble(bottleCtrl, i))
                continue;
            if (bottleCtrl.hideWaters[i] != expect)
            {
                bottleCtrl.hideWaters[i] = expect;
                return bottleCtrl.gameObject;
            }
        }
        for (int i = 0; i < random; i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheckForHide(bottleCtrl, i))
                continue;



            bottleCtrl.hideWaters[i] = expect;
            bottleCtrl.waterItems[i] = WaterItem.Bubble;
            return bottleCtrl.gameObject;

        }

        return null;
    }
    public bool WaterStateCheckForBubble(BottleCtrl bottleCtrl, int index)
    {
        return WaterStateCheckForHide(bottleCtrl, index);
    }

    #endregion

    public bool BottleStateCheck(BottleCtrl bottleCtrl)
    {
        return true;
    }
}
