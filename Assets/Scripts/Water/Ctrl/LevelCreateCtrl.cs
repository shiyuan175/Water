using System;
using System.Collections.Generic;
using Game.Water;
using UnityEngine;

namespace Game.Water
{
    /// <summary>
    ///     关卡创建控制器 - 用于定义游戏关卡的配置数据
    /// </summary>
    [CreateAssetMenu(fileName = "Level", menuName = "Levels")]
    public class LevelCreateCtrl : ScriptableObject
    {
        /// <summary>
        ///     所有瓶子的属性列表 (总数为topNum + bottomNum)
        /// </summary>
        public List<BottleProperty> bottles;

        /// <summary>
        ///     当前关卡的游戏模式
        /// </summary>
        public GameType gameType;

        /// <summary>
        ///     需要清空的颜色列表 (关卡目标)
        /// </summary>
        public List<int> clearList;

        /// <summary>
        ///     隐藏的颜色列表 (初始隐藏的颜色，通过道具1003等显示)
        /// </summary>
        public List<int> hideList;

        /// <summary>
        ///     隐藏的水类型列表 (与hideList对应，0为普通水，1为草丛水)
        /// </summary>
        public List<HideWaterType> hideTypes;

        /// <summary>
        ///     全局机制配置
        /// </summary>
        public GlobalMechanism globalMechanism;

        /// <summary>
        ///     全局机制开始步数
        /// </summary>
        public int GlobalMechanismBeginSetp;

        /// <summary>
        ///     全局机制持续步数
        /// </summary>
        public int GlobalMechanismContinueSetps;

        /// <summary>
        ///     当前关卡的倒计时数字 (在关卡模式中，0表示不启用)
        /// </summary>
        public int countDownNum;

        /// <summary>
        ///     当前关卡的倒计时时间 (在关卡模式中，0表示不启用)
        /// </summary>
        public float timeCountDown;

        /// <summary>
        ///     顶部瓶子的数量 (游戏界面上方瓶子数量)
        /// </summary>
        public int topNum;

        /// <summary>
        ///     底部瓶子的数量 (游戏界面下方瓶子数量)
        /// </summary>
        public int bottomNum;

        /// <summary>
        ///     默认气泡数量
        /// </summary>
        public List<int> bubbleCount;

        /// <summary>
        ///     当前关卡中的颜色转换列表 (控制某些特殊效果, 2001-2006为特殊效果)
        /// </summary>
        public List<ChangePair> changeList;

        /// <summary>
        ///     瓶子的属性数据类
        /// </summary>
        [Serializable]
        public class BottleProperty
        {
            /// <summary>
            ///     瓶子中每层水的颜色数组 (1-12 表示颜色, >1000 表示特殊效果)
            /// </summary>
            public List<int> waterSet = new();

            /// <summary>
            ///     每层水的隐藏类型 (0为普通水，1为草丛水)
            /// </summary>
            public List<HideWaterType> hideTypes = new();

            /// <summary>
            ///     每层水的详细状态 (炸弹、冰冻等)
            /// </summary>
            public List<WaterItem> waterItem = new();

            /// <summary>
            ///     炸弹倒计时列表
            /// </summary>
            public List<int> bombCounts = new();

            /// <summary>
            ///     蛋糕数量
            /// </summary>
            public int numCake = 4;

            /// <summary>
            ///     限制颜色 - 相同颜色数量 (0 表示无限制)
            /// </summary>
            public int limitColor;

            /// <summary>
            ///     是否清除隐藏状态
            /// </summary>
            public bool isClearHide;

            /// <summary>
            ///     锁定类型 - 确定瓶子中液体需要特定颜色才能混合或清空
            /// </summary>
            public int lockType;

            /// <summary>
            ///     是否为黑色瓶子
            /// </summary>
            public List<bool> BlackBottleList = new();

            public bool isBlackBottle = false;

            /// <summary>
            ///     窗帘高度
            /// </summary>
            public int CurtainHight;

            /// <summary>
            ///     是否靠近隐藏、是否冻结 (在关卡中控制显示/隐藏)
            /// </summary>
            public bool isNearHide, isFreeze;

            /// <summary>
            ///     是否已完成
            /// </summary>
            public bool isFinish;
        }
    }

    /// <summary>
    ///     颜色转换对数据类
    /// </summary>
    [Serializable]
    public class ChangePair
    {
        /// <summary>
        ///     转换目标的道具类型 (转换后目标颜色)
        /// </summary>
        public ItemType item;

        /// <summary>
        ///     需要转换的颜色数值
        /// </summary>
        public int NeedChangeColor;
    }
}