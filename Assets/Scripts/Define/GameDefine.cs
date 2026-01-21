using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using GameAttributes;
using JsonFileData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDefine
{
    public static class GameConst
    {
        //新手关(前五关)
        public const int NEWBIE_LEVEL_COUNT = 5;

        public const int ITEM_COUNT = 8;

        //第八关开启连胜相关功能(主页第一个横幅活动...)
        public const int WIN_STREAK_BEGIN_LEVEL = 8;
        //活动开启目标关卡
        public const int IN_GAME_RANK_BEGIN_LEVEL = 6; //游戏内段位       

        //活动存档标记
        public const string RANKA_ACTIVITY_SIGN = "RankAActivity";

        //存档标记
        public const string FIRST_LAUNCH_SIGN = "FIRST_LAUNCH";
        public const string DOUBLE_COIN_SIGN = "DoubleCoin";

        //事件标记
        public const string MANAGER_OPEN_NEXT_PANEL = "ManagerOpenNextPanel";
        public const string VICTORY_EVENT = "VictoryEvent";
     
        #region Json file info

        public static readonly JsonFileInfo MSADefaultJson = new()
        {
            FileName = "MSADefaultData.json",
            TargetVersion = 1
        };
        public static readonly string MSA_CURRENT_JSON = "MSACurrent.json";

        public static readonly JsonFileInfo TRADefaultJson = new()
        {
            FileName = "TRADefaultData.json",
            TargetVersion = 1
        };
        public static readonly string TRA_CURRENT_JSON = "TRA_Data.json";

        public static readonly JsonFileInfo BPDefaultJson = new()
        {
            FileName = "BPDefaultData.json",
            TargetVersion = 1
        };
        public static readonly string BP_CURRENT_JSON = "BP_Data.json";

        public static readonly JsonFileInfo PGDefaultJson = new()
        {
            FileName = "PGDefaultData.json",
            TargetVersion = 1
        };
        public static readonly string PGCurrentJson = "PG_Data.json";

        public static readonly JsonFileInfo GAME_GLOBAL_DEFAULT_JSON = new()
        {
            FileName = "GameGlobalDefaultData.json",
            TargetVersion = 1
        };
        public static readonly string GAME_GLOBAL_CURRENT_JSON = "GameGlobalData.json";

        public static readonly JsonFileInfo DAILY_REWARD_DEFAULT_JSON = new()
        {
            FileName = "DailyRewardDefaultData.json",
            TargetVersion = 1
        };
        public static readonly string DAILY_REWARD_CURRENT_JSON = "DailyRewardData.json";

        #endregion

        //关卡引导(bottleIndex_1必须大于0，索引为瓶子在levelmanager的bottles所在索引)
        public static readonly Dictionary<int, (int bottleIndex_1, int bottleIndex_2, string guideInfo)>
            GameplayTutorialInfo = new()
            {
                { 3, (0, -1, "Sort the gemstone color to lift the cloth") },
                { 11, (0, -1, "Add one bubble per step until enough are created.") },
                { 21, (-10, -1, "White Cat helps you eliminate one black obstruction per step.") },
                { 31, (0, -1, "The water bottle entangled by the vines cannot be moved") },
                { 41, (2, -1, "The vine water bottle can break the entangled vines after the adjacent water bottles are combined") },
                { 51, (2, -1, "Bottles with gemstone emblems can only be filled with water of the same color as the gemstone") }, 
                { 61, (2, -1, "Get the bomb to the surface before it explodes.") },
                { 71, (0, 1, "Combining two brooms can remove water of the same color") },
                { 81, (0, -1, "Each time you synthesize a water, all curtains drop one grid.") },
                { 91, (0, -1, "When the grass ball is on the top water surface, it spawns grass covering the water tiles.") },
                { 101, (0, -1, "The bomb will explode when the countdown ends. Please synthesize water with bombs as soon as possible.") },
                { 121, (0, -1, "Water with Fire Emblem can thaw ice after being crafted") },
                { 141, (-10, -1, "Black Cat spawns one black obstruction per subsequent step after 5 moves, until enough are created.") },
                { 161, (1, -1, "A bottle is generated after synthesizing sparkling water.") },
                { 181, (1, 3, "Combining two potion bottles can change 4 water of the same color") },
                { 201, (9, 11, "Two question mark balls synthesized spawn some black obstructions.") },
                { 301, (0, 1, "Synthesizing a magic book can remove all negative effects") },
                { 401, (2, -1, "The black obstruction on the bottle never disappears; when the bottle is lifted, it reveals the color of the water tiles.") }, //17
            };
    }

    /// <summary>
    /// 引导关标记
    /// </summary>
    public enum UIGuideLevel
    {
        UIGuideLevel1 = 1,
        UIGuideLevel2 = 2,
        UIGuideLevelRemoveHide = 39,
        UIGuideLevelRemoveAll = 76
    }

    /// <summary>
    ///  全局性机制，不依赖水块或者瓶子
    /// </summary>
    public enum GlobalMechanism
    {
        None = 1,
        WhiteMagicCar = 2,
        BlackMagicCar = 3
    }
    
    public enum LevelHardType
    {
        Hard = 4,
        VeryHand = 9
    }

    public enum GameType
    {
        Normal = 0,
        Bomb = 1,
        Count = 2,
        Step = 3,
        Hide = 4,
    }

    /// <summary>
    /// 带有底部水的机制
    /// </summary>
    public enum WaterItem
    {
        // 更新分为两类，一类是每一步都需要维护状态的如炸弹，一类是只维护消失结束的，如泡沐;
        // 炸弹采用同黑水等机制的类update去维护，泡沐采用在边界情况维护，如removeall，退出游戏等
        None = 0,
        Ice = 1,
        BreakIce = 2,
        Bomb = 3,
        FlyBomb = 4,
        Bubble = 5,
        Bubble_Origin = 6,
        GrassBomb = 7
    }

    public enum BottleType
    {
        None = 0,
        ClearShow = 1,
        NearShow = 2
    }

    /// <summary>
    /// 不带底部水的机制(或特殊水块)
    /// </summary>
    public enum ItemType
    {
        [WaterColorState("", EColorStateSpineType.None)]
        UseColor = 1,

        [WaterColorState("idle_cl", EColorStateSpineType.EBroomSpine)]
        ClearRandomWaterItem = 1001,

        [WaterColorState("idle", EColorStateSpineType.EMagnetSpine)]
        MagnetItem = 1002,

        [WaterColorState("idle", EColorStateSpineType.ECreateSpine)]
        MakeColorItem = 1003,

        [ChangeColorItemState(1, "idle_cl")]
        ChangeGreen = 2001,

        [ChangeColorItemState(7, "idle_jh")]
        ChangeOrange = 2002,

        [ChangeColorItemState(3, "idle_fs")]
        ChangePink = 2003,

        [ChangeColorItemState(10, "idle_zs")]
        ChangePurple = 2004,

        [ChangeColorItemState(6, "idle_hs")]
        ChangeYellow = 2005,

        [ChangeColorItemState(9, "idle_sl")] ChangeDarkGreen = 2006,

        [ChangeColorItemState(4, "idle_gl")] ChangeBlue = 2007,
        
        [ClearItemState(3, "idle_fh")]
        ClearPink = 3001,

        [ClearItemState(7, "idle_jh")]
        ClearOrange = 3002,

        [ClearItemState(4, "idle_gl")]
        ClearBlue = 3003,

        [ClearItemState(6, "idle_hs")]
        ClearYellow = 3004,

        [ClearItemState(9, "idle_sl")]
        ClearDarkGreen = 3005,

        [ClearItemState(2, "idle_dh")]
        ClearRed = 3006,

        [ClearItemState(1, "idle_cl")]
        ClearGreen = 3007,

        [RainBowWaterState("")]
        RainBowWater = 4001,

        [RainBowWaterState("", EColorStateSpineType.EFlashWater)]
        FlashWater = 4002,

        [WaterColorState("Combile", EColorStateSpineType.EBombBlackWater)]
        BombBlackWater = 5001,

        [WaterColorState("idle", EColorStateSpineType.EFlyBomb)]
        FlyBomb = 5002,
        
    }

    public enum LanguageType
    {
        zh = 0,
        ja = 1,
        en = 2,
        ko = 3,
    }

    public enum EIdleAnim
    {
        [Description("idle_cl")]
        IDLE_CL = 1,

        [Description("idle_dh")]
        IDLE_DH = 2,

        [Description("idle_fh")]
        IDLE_FH = 3,

        [Description("idle_gl")]
        IDLE_GL = 4,

        [Description("idle_hl")]
        IDLE_HL = 5,

        [Description("idle_hs")]
        IDLE_HS = 6,

        [Description("idle_jh")]
        IDLE_JH = 7,

        [Description("idle_lh")]
        IDLE_LH = 8,

        [Description("idle_sl")]
        IDLE_SL = 9,

        [Description("idle_ze")]
        IDLE_ZE = 10,

        [Description("idle_zs")]
        IDLE_ZS = 11,

        [Description("idle_mh")]
        IDLE_MH = 12,

        IDLE_MAX = 13
    }

    public enum ECombimeAnim
    {
        [Description("combine_cl")]
        COMBINE_CL = 1,

        [Description("combine_dh")]
        COMBINE_DH = 2,

        [Description("combine_fh")]
        COMBINE_FH = 3,

        [Description("combine_gl")]
        COMBINE_GL = 4,

        [Description("combine_hl")]
        COMBINE_HL = 5,

        [Description("combine_hs")]
        COMBINE_HS = 6,

        [Description("combine_jh")]
        COMBINE_JH = 7,

        [Description("combine_lh")]
        COMBINE_LH = 8,

        [Description("combine_sl")]
        COMBINE_SL = 9,

        [Description("combine_ze")]
        COMBINE_ZE = 10,

        [Description("combine_zs")]
        COMBINE_ZS = 11,

        [Description("combine_mh")]
        COMBINE_MH = 12,

        IDLE_MAX = 13
    }

    public enum EDisapearAnim
    {
        [Description("disapear_cl")]
        DISAPEAR_CL = 1,

        [Description("disapear_dh")]
        DISAPEAR_DH = 2,

        [Description("disapear_fh")]
        DISAPEAR_FH = 3,

        [Description("disapear_gl")]
        DISAPEAR_GL = 4,

        [Description("disapear_hl")]
        DISAPEAR_HL = 5,

        [Description("disapear_hs")]
        DISAPEAR_HS = 6,

        [Description("disapear_jh")]
        DISAPEAR_JH = 7,

        [Description("disapear_lh")]
        DISAPEAR_LH = 8,

        [Description("disapear_sl")]
        DISAPEAR_SL = 9,

        [Description("disapear_ze")]
        DISAPEAR_ZE = 10,

        [Description("disapear_zs")]
        DISAPEAR_ZS = 11,

        [Description("disapear_mh")]
        DISAPEAR_MH = 12,

        IDLE_MAX = 13
    }

    /// <summary>
    /// 水花动画名
    /// </summary>
    public enum EDaoShuiAnim
    {
        [Description("daoshui_cl")]
        DAOSHUI_CL = 1,

        [Description("daoshui_dh")]
        DAOSHUI_DH = 2,

        [Description("daoshui_fh")]
        DAOSHUI_FH = 3,

        [Description("daoshui_gl")]
        DAOSHUI_GL = 4,

        [Description("daoshui_hl")]
        DAOSHUI_HL = 5,

        [Description("daoshui_hs")]
        DAOSHUI_HS = 6,

        [Description("daoshui_jh")]
        DAOSHUI_JH = 7,

        [Description("daoshui_lh")]
        DAOSHUI_LH = 8,

        [Description("daoshui_sl")]
        DAOSHUI_SL = 9,

        [Description("daoshui_ze")]
        DAOSHUI_ZE = 10,

        [Description("daoshui_zs")]
        DAOSHUI_ZS = 11,

        [Description("daoshui_mh")]
        DAOSHUI_MH = 12,

        IDLE_MAX = 13,

        [Description("daoshui_cs")]
        RainBowWater = 4001,

        [Description("daoshui_js")]
        FlashWater = 4002,
    }

    /// <summary>
    /// 水面动画名
    /// </summary>
    public enum ERuChangAnim
    {
        [Description("ruchanghuangdong_cl")]
        RUCHANGANIM_CL = 1,

        [Description("ruchanghuangdong_dh")]
        RUCHANGANIM_DH = 2,

        [Description("ruchanghuangdong_fh")]
        RUCHANGANIM_FH = 3,

        [Description("ruchanghuangdong_gl")]
        RUCHANGANIM_GL = 4,

        [Description("ruchanghuangdong_hl")]
        RUCHANGANIM_HL = 5,

        [Description("ruchanghuangdong_hs")]
        RUCHANGANIM_HS = 6,

        [Description("ruchanghuangdong_jh")]
        RUCHANGANIM_JH = 7,

        [Description("ruchanghuangdong_lh")]
        RUCHANGANIM_LH = 8,

        [Description("ruchanghuangdong_sl")]
        RUCHANGANIM_SL = 9,

        [Description("ruchanghuangdong_ze")]
        RUCHANGANIM_ZE = 10,

        [Description("ruchanghuangdong_zs")]
        RUCHANGANIM_ZS = 11,

        [Description("ruchanghuangdong_mh")]
        RUCHANGANIM_MH = 12,

        IDLE_MAX = 13,

        [Description("ruchanghuangdong_cs")]
        RainBowWater = 4001,

        [Description("ruchanghuangdong_js")]
        FlashWater = 4002,
    }

    public enum HideWaterType
    {
        None = 0,
        HideWater = 1,
        GrassWater = 2,
        Max = 3
    }

    public class GameEnum
    {
        public static string GetDescription<T>(T value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
    
    public static partial class GameUtils
    {
        /// <summary>
        /// false is does not exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DoesCountDownKeyExist(string id) =>
            PlayerPrefs.HasKey(CountDownTimerManager.COUNTDOWN_TIMER_SIGN + id);

        public static void SotrArray<T>(T[] array) where T : Object
        {
            Array.Sort(array, (a, b) =>
            {
                int aIndex = ExtractNumber(a.name);
                int bIndex = ExtractNumber(b.name);
                return aIndex.CompareTo(bIndex);
            });
        }

        private static int ExtractNumber(string name)
        {
            Match match = Regex.Match(name, @"(\d+)$");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return 0;
        }
    }

    public static class WaterAttrCache
    {
        public static readonly Dictionary<ItemType, WaterColorState> Dict = new();

        static WaterAttrCache()
        {
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                var field = typeof(ItemType).GetField(type.ToString());
                var attr = field?.GetCustomAttribute<WaterColorState>();
                if (attr != null)
                    Dict[type] = attr;
            }
        }
    }
}
