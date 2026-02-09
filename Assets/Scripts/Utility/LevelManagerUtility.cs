using GameDefine;
using QFramework;
using System.Collections.Generic;
using UnityEngine;
using Water;
public class LevelManagerUtility : IUtility
{
    private const int NORMALWATER_LIMITMAX = 1000;

    #region 黑水
    /// <summary>
    /// 随机生存黑水
    /// </summary>
    public BottletempCtrl RandomBarkWaterBottle(List<BottletempCtrl> bottles, HideWaterType hideType)
    {
        int i = UnityEngine.Random.Range(0, bottles.Count);
        int old = i;
        while (true)
        {
            var newBottletempCtrl = bottles[i % bottles.Count];
            if (!BottleStateCheck(newBottletempCtrl))
            {
                i++;
                continue;
            }

            var result = RandomBarkWater(newBottletempCtrl, hideType);
            if (result != null)
                return result;
            else
                i++;
            if (old == i % bottles.Count)
                return null;
        }
    }

    private BottletempCtrl RandomBarkWater(BottletempCtrl BottletempCtrl, HideWaterType hideType)
    {
        var random = Random.Range(0, BottletempCtrl.hideTypes.Count - 1);
        for (var i = random; i < BottletempCtrl.hideTypes.Count - 1; i++)
        {
            if (!WaterStateCheckForHide(BottletempCtrl, i))
                continue;
            BottletempCtrl.hideTypes[i] = hideType;
            if (LevelManager.Instance.hideBottleList.Find(bottle => bottle == BottletempCtrl) == null)
            {
                LevelManager.Instance.hideBottleList.Add(BottletempCtrl);
            }

            BottletempCtrl.SetHideShow(true, i);
            return BottletempCtrl;
        }
        for (int i = 0; i < random; i++)
        {
            if (!WaterStateCheckForHide(BottletempCtrl, i))
                continue;
            BottletempCtrl.hideTypes[i] = hideType;
            if (LevelManager.Instance.hideBottleList.Find(bottle => bottle == BottletempCtrl) == null)
            {
                // ???????
                LevelManager.Instance.hideBottleList.Add(BottletempCtrl);
            }

            // ????????
            BottletempCtrl.SetHideShow(true, i);
            return BottletempCtrl;

        }
        return null;
    }

    private bool WaterStateCheckForHide(BottletempCtrl BottletempCtrl, int index)
    {
        if (BottletempCtrl.waterItems[index] != WaterItem.None)
            return false;
        if (BottletempCtrl.IsBlackBottles.Count > index && BottletempCtrl.IsBlackBottles[index])
            return false;
        if (BottletempCtrl.hideTypes[index] != HideWaterType.None)
            return false;
        if (BottletempCtrl.waters[index] > NORMALWATER_LIMITMAX)
            return false;
        if (BottletempCtrl.waters[index] == BottletempCtrl.GetMoveOutTop())
        {
            bool _flag = false;
            for (var i = index + 1; i < BottletempCtrl.topIdx; i++)
            {
                if (BottletempCtrl.waters[i] != BottletempCtrl.GetMoveOutTop())
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
    public BottletempCtrl RandomRomveBarkWaterBottle(List<BottletempCtrl> bottles)
    {

        int i = UnityEngine.Random.Range(0, bottles.Count);
        int old = i;
        while (true)
        {
            var newBottletempCtrl = bottles[i % bottles.Count];
            if (!BottleStateCheck(newBottletempCtrl))
            {
                i++;
                continue;
            }

            var result = RandomRomveBarkWater(newBottletempCtrl);
            if (result != null)
                return result;
            else
                i++;
            if (old == i % bottles.Count)
                return null;
        }
    }

    private BottletempCtrl RandomRomveBarkWater(BottletempCtrl BottletempCtrl)
    {
        // ???????
        var random = Random.Range(0, BottletempCtrl.hideTypes.Count - 1);
        for (var i = random; i < BottletempCtrl.hideTypes.Count - 1; i++)
        {
            if (!WaterStateCheckForClear(BottletempCtrl, i))
                continue;

            BottletempCtrl.hideTypes[i] = HideWaterType.None;
            // ?ж??????????????????
            bool flag = true;
            foreach (var hide in BottletempCtrl.hideTypes)
            {
                if (hide == HideWaterType.HideWater)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                LevelManager.Instance.hideBottleList.Remove(BottletempCtrl);
            BottletempCtrl.SetHideShow(true, i);
            return BottletempCtrl;
        }
        for (int i = 0; i < random; i++)
        {
            if (!WaterStateCheckForClear(BottletempCtrl, i))
                continue;
            BottletempCtrl.hideTypes[i] = HideWaterType.None;
            bool flag = true;
            foreach (var hide in BottletempCtrl.hideTypes)
            {
                if (hide != HideWaterType.None)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                LevelManager.Instance.hideBottleList.Remove(BottletempCtrl);
            BottletempCtrl.SetHideShow(true, i);
            return BottletempCtrl;

        }
        return null;
    }

    private bool WaterStateCheckForClear(BottletempCtrl BottletempCtrl, int index)
    {
        if (BottletempCtrl.hideTypes[index] == HideWaterType.None)
            return false;
        return true;
    }
    #endregion

    #region 随机生成泡沫

    public bool RandomBubleWaterBottle(List<BottletempCtrl> bottles)
    {
        int i = UnityEngine.Random.Range(0, bottles.Count);
        int old = i;
        while (true)
        {
            var newBottletempCtrl = bottles[i % bottles.Count];

            if (!BottleStateCheck(newBottletempCtrl))
            {
                i++;
                continue;
            }

            if (RandomBubbleWater(newBottletempCtrl))
                return true;
            else
                i++;
            if (old == i % bottles.Count)
                return false;
        }
    }

    private bool RandomBubbleWater(BottletempCtrl BottletempCtrl)
    {
        var random = Random.Range(0, BottletempCtrl.hideTypes.Count - 1);
        for (var i = random; i < BottletempCtrl.hideTypes.Count - 1; i++)
        {
            // 检查是否可以生成泡沐
            if (!WaterStateCheckForBubble(BottletempCtrl, i))
                continue;

            BottletempCtrl.waterItems[i] = WaterItem.Bubble;
            BottletempCtrl.waterImg[i].bubbleCtrl.BubbleAppend();
            return true;
        }

        for (int i = 0; i < random; i++)
        {
            if (!WaterStateCheckForBubble(BottletempCtrl, i))
                continue;

            BottletempCtrl.waterItems[i] = WaterItem.Bubble;
            BottletempCtrl.waterImg[i].bubbleCtrl.BubbleAppend();
            return true;
        }

        return false;
    }


    private bool WaterStateCheckForBubble(BottletempCtrl BottletempCtrl, int index)
    {
        return WaterStateCheckForHide(BottletempCtrl, index);
    }

    #endregion


    private bool BottleStateCheck(BottletempCtrl BottletempCtrl)
    {
        return true;
    }

}
