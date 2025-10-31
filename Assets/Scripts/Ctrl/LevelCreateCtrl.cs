using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using System;

//1：浅绿色
//2：深红色
//3：浅粉色
//4：深蓝色
//5：浅蓝色
//6：黄色
//7：橙色
//8：浅紫色
//9：深绿色
//10：深紫色
//11：浅红(棕)色
//12：深粉色
[CreateAssetMenu(fileName = "Level", menuName = "Levels")]
public class LevelCreateCtrl : ScriptableObject
{
    // 瓶子配置(总数为topNum + bottomNum)
    public List<BottleProperty> bottles;
    // 当前关卡的游戏类型
    public GameType gameType;

    /// <summary>
    /// 瓶子的属性定义
    /// </summary>
    [System.Serializable]
    public class BottleProperty
    {
        // 瓶子中每层水的颜色编号（1-12 表示颜色，>1000 表示特殊道具）
        // 配置修改颜色道具需同时配置changeList
        public List<int> waterSet = new List<int>();
        // 是否隐藏水颜色（黑色问号）
        public List<bool> isHide = new List<bool>();
        // 每层水的附加状态（如冰块、炸弹等）
        public List<WaterItem> waterItem = new List<WaterItem>();
        // 炸弹计数器
        public List<int> bombCounts = new List<int>();
        // 最大水层数
        public int numCake = 4;
        // 限制往瓶子倒水的颜色-同水颜色编号（0 表示无限制）
        public int limitColor;
        // 不确定（可能用于控制瓶子的解锁逻辑。需要特定颜色编号才能解锁隐藏内容）
        public int lockType;
        // 三种障碍(遮挡布，大型藤曼，底部藤曼)
        public bool isClearHide, isNearHide, isFreeze;
        public bool isFinish;

        // 弃用
        public bool hidePriority = false;
        // 是否是飞翔炸弹，注意，最好全设置飞翔炸弹存在,且飞翔炸弹最好不要设置在水面
        public bool isFlyBomb = false;


    }
    // 需要清除的颜色列表（关卡目标）
    public List<int> clearList;
    // 隐藏的颜色列表（初始隐藏的颜色，通过道具1003触发然后显示）
    public List<int> hideList;
    // 当前关卡中炸弹的数量_配置都为0不确定用途 弃用
    public int bombNum;
    // 当前关卡的倒计时步数（用于步数限制模式）_配置都为0不确定用途
    public int countDownNum;
    // 当前关卡的倒计时时间（用于时间限制模式）_配置都为0不确定用途
    public float timeCountDown;
    // 顶部瓶子的数量（用于瓶子布局）
    public int topNum;
    // 底部瓶子的数量（用于瓶子布局）
    public int bottomNum;
    // 泡沫生成次数
    public List<BubbleData> bubbleData;
    // 该关卡存在的颜色变换列表（用于某些特殊道具逻辑,2001-2006的道具需配置）
    public List<ChangePair> changeList;
}

[Serializable]
public class ChangePair
{
    // 触发变换的道具类型(变换的目标颜色)
    public ItemType item;
    // 需要变换的颜色编号
    public int NeedChangeColor;
}
[Serializable]
public class BubbleData
{
    public BottleCtrl bottleCtrl;
    public int bubbleCount;
}
