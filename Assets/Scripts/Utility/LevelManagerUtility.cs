using GameDefine;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ????????????LevelManager?е??Щ???????????????????LevelManager????????????????????
/// </summary>
public class LevelManagerUtility : IUtility
{
    private const int NORMALWATER_LIMITMAX = 1000;

    #region 黑水
    /// <summary>
    /// 随机生存黑水
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
        // ???????
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {

            if (!WaterStateCheckForHide(bottleCtrl, i))
                continue;
            bottleCtrl.hideWaters[i] = true;
            if(LevelManager.Instance.hideBottleList.Find(bottle => bottle == bottleCtrl) == null)
            {
                // ???????
                LevelManager.Instance.hideBottleList.Add(bottleCtrl);               
            }

            // ????????
                bottleCtrl.SetHideShow(true,i);    
            return bottleCtrl;
        }
        for (int i = 0; i < random; i++)
        {

            if (!WaterStateCheckForHide(bottleCtrl, i))
                continue;
            bottleCtrl.hideWaters[i] = true;
            if (LevelManager.Instance.hideBottleList.Find(bottle => bottle == bottleCtrl) == null)
            {
                // ???????
                LevelManager.Instance.hideBottleList.Add(bottleCtrl);              
            }

            // ????????
                bottleCtrl.SetHideShow(true, i);
            return bottleCtrl;

        }
        return null;
    }

    private bool WaterStateCheckForHide(BottleCtrl bottleCtrl, int index)
    {

        if (bottleCtrl.waterItems[index] != WaterItem.None)
            return false;
        if (bottleCtrl.IsBlackBottles[index])
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

    #region 随机删除黑水
    /// <summary>
    /// 删除黑水 
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
        // ???????
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {
            if (!WaterStateCheckForClear(bottleCtrl, i))
                continue;

            bottleCtrl.hideWaters[i] = false;
            // ?ж??????????????????
            bool flag = true;
            foreach (var hide in bottleCtrl.hideWaters)
            {
                if (hide == true)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                LevelManager.Instance.hideBottleList.Remove(bottleCtrl);
            bottleCtrl.SetHideShow(true, i);
            return bottleCtrl;
        }
        for (int i = 0; i < random; i++)
        {
            if (!WaterStateCheckForClear(bottleCtrl, i))
                continue;
            bottleCtrl.hideWaters[i] = false;
            // ?ж??????????????????
            bool flag = true;
            foreach(var hide in bottleCtrl.hideWaters)
            {
                if(hide == true)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                LevelManager.Instance.hideBottleList.Remove(bottleCtrl);
            bottleCtrl.SetHideShow(true, i);
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

    #region 随机生成泡沫

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
        
        int random = UnityEngine.Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {
            // 检查是否可以生成泡沐
            if (!WaterStateCheckForBubble(bottleCtrl, i))
                continue;

            bottleCtrl.waterItems[i] = WaterItem.Bubble;
            bottleCtrl.waterImg[i].bubbleCtrl.BubbleAppend();
            return true;
        }
        for (int i = 0; i < random; i++)
        {
            // ???????????ж?????????????????
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
