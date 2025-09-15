using MoonSharp.Interpreter;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
/// <summary>
/// 该脚本的本意是将LevelManager中的一些计算逻辑移动出来，是的LevelManager的体量降低，变为较可维护
/// </summary>
public class LevelManagerUtility : IUtility
{
    /// <summary>
    /// 随机选取一个水块进行黑水的变化
    /// </summary>
    /// <param name="expect">期望的黑水结果</param>
    public GameObject RandomBarkWater(List<BottleCtrl> bottles ,bool expect)
    {
        int i= Random.Range(0, bottles.Count);
        while( true)
        {
            
            int old = i;
            BottleCtrl newBottleCtrl = bottles[i];
            if (!BottleStateCheck(newBottleCtrl))
            {
                i++;
                continue;
            }
               

            if (WaterComplete(newBottleCtrl,expect) != null)
                return WaterComplete(newBottleCtrl, expect);
            else
                i++;
            if (old == i % bottles.Count)
                return null;
        }
        
    }
    public  GameObject WaterComplete(BottleCtrl bottleCtrl, bool expect)
    {
        // 去掉最顶上
        int random = Random.Range(0, bottleCtrl.hideWaters.Count - 1);
        for (int i = random; i < bottleCtrl.hideWaters.Count - 1; i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheck(bottleCtrl, i))
                continue;
            if (bottleCtrl.hideWaters[i] != expect)
            { 
                bottleCtrl.hideWaters[i] = expect;
                return bottleCtrl.waterImg[i].gameObject;
            }
        }
        for(int i=0;i<random;i++)
        {
            // 暂时留的一个判断函数，不确定是否需要
            if (!WaterStateCheck(bottleCtrl, i))
                continue;


            if (bottleCtrl.hideWaters[i] != expect)
            {
                bottleCtrl.hideWaters[i] = expect;
                return bottleCtrl.waterImg[i].gameObject;
            }
        }
        
        return null;
    }
    public bool WaterStateCheck(BottleCtrl bottleCtrl,int index)
    {
        return true;
    }
    public bool BottleStateCheck(BottleCtrl bottleCtrl)
    {
        return true;
    }
}
