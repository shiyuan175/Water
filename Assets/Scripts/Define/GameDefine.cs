using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using GameAttributes;

namespace GameDefine
{
    public static class GameConst
    {
        public const int ITEM_COUNT = 8;
        //十连胜1.5倍金币buff
        public const int CONTINUE_WIN_NUM_COIN = 10;
        //过关基础金币
        public const int WIN_COINS = 20;
        public const int ADD_BOTTLE_COST = 900;

        public const int SCENE_PART_COUNT = 5;
        //第八关开启连胜相关功能(连胜段位\连胜去黑\主页连胜活动...)
        public const int WIN_STREAK_BEGIN_LEVEL = 8;

        //连胜去黑(最高三档)
        public const int MAX_GIFT_STREAK_WIN = 3;

        public const string DOUBLE_COIN_SIGN = "DoubleCoin";
        public const string POTION_ACTIVITY_SIGN = "PotionActivity";
        public const string UNLIMIT_ITEM_SIGN = "UnLimitItenEndTime";
        public const string RANKA_ACTIVITY_SIGN = "RankAActivity";

        //活动开启目标关卡
        public const int VA_BEGIN_LEVEL = 0;//开启火山活动的关卡(未定)

        //关卡引导
        public static readonly Dictionary<int, (string, string)> GuideLevelInfo = new Dictionary<int, (string, string)>
        {
            { 3, ("Sort the gemstone color to lift the cloth", "GuideAnim_3") },
            { 11, ("The bomb will explode when the countdown ends. Please synthesize water with bombs as soon as possible.", "GuideAnim_11") },
            { 21, ("Water with Fire Emblem can thaw ice after being crafted", "GuideAnim_21") },
            { 31, ("The water bottle entangled by the vines cannot be moved", "GuideAnim_31") },
            { 41, ("The vine water bottle can break the entangled vines after the adjacent water bottles are combined", "GuideAnim_41") },
            { 51, ("Combining two brooms can remove water of the same color", "GuideAnim_51") },
            { 61, ("Bottles with gemstone emblems can only be filled with water of the same color as the gemstone", "GuideAnim_61") },
            { 71, ("Combining two potion bottles can change 4 water of the same color", "GuideAnim_71") },
            { 81, ("Synthesizing a magic book can remove all negative effects", "GuideAnim_81")},
        };

        //场景解锁界面(索引对应AB包名)
        public static readonly Dictionary<int, string> SceneUnlock = new Dictionary<int, string>
        {
            {0, "SceneUnlock1"}
        };
    }

    public enum GameType
    {
        Normal = 0,
        Bomb = 1,
        Count = 2,
        Step = 3,
        Hide = 4,
    }

    public enum WaterItem
    {
        None = 0,
        Ice = 1,
        BreakIce = 2,
        Bomb = 3,
    }

    public enum BottleType
    {
        None = 0,
        ClearShow = 1,
        NearShow = 2
    }

    public enum ItemType
    {
        [WaterColorState(false, false, false, false, "", EColorStateSpineType.None)]
        UseColor = 1,
        
        [WaterColorState(true, false, false, false, "idle_cl", EColorStateSpineType.EBroomSpine)]
        ClearItem = 1001,
        
        [WaterColorState(false, false, false, true, "idle", EColorStateSpineType.EMagnetSpine)]
        MagnetItem = 1002, 
        
        [WaterColorState(false, true, false, false, "idle", EColorStateSpineType.ECreateSpine)]
        MakeColorItem = 1003,
        
        [WaterColorState(false, false, true, false, "idle_cl", EColorStateSpineType.EChangeSpine)]
        ChangeGreen = 2001,
        
        [WaterColorState(false, false, true, false, "idle_jh", EColorStateSpineType.EChangeSpine)]
        ChangeOrange = 2002,
        
        [WaterColorState(false, false, true, false, "idle_fs", EColorStateSpineType.EChangeSpine)]
        ChangePink = 2003,
        
        [WaterColorState(false, false, true, false, "idle_zs", EColorStateSpineType.EChangeSpine)]
        ChangePurple = 2004,
        
        [WaterColorState(false, false, true, false, "idle_hs", EColorStateSpineType.EChangeSpine)]
        ChangeYellow = 2005,
        
        [WaterColorState(false, false, true, false, "idle_sl", EColorStateSpineType.EChangeSpine)]
        ChangeDarkBlue = 2006,
        
        [WaterColorState(true, false, false, false, "idle_fh", EColorStateSpineType.EBroomSpine)]
        ClearPink = 3001,
        
        [WaterColorState(true, false, false, false, "idle_jh", EColorStateSpineType.EBroomSpine)]
        ClearOrange = 3002,
        
        [WaterColorState(true, false, false, false, "idle_gl", EColorStateSpineType.EBroomSpine)]
        ClearBlue = 3003,
        
        [WaterColorState(true, false, false, false, "idle_hs", EColorStateSpineType.EBroomSpine)]
        ClearYellow = 3004,
        
        [WaterColorState(true, false, false, false, "idle_sl", EColorStateSpineType.EBroomSpine)]
        ClearDarkGreen = 3005,
        
        [WaterColorState(true, false, false, false, "idle_dh", EColorStateSpineType.EBroomSpine)]
        ClearRed = 3006,
        
        [WaterColorState(true, false, false, false, "idle_cl", EColorStateSpineType.EBroomSpine)]
        ClearGreen = 3007,

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

        IDLE_MAX = 13
    }

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

        IDLE_MAX = 13
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

    public static class GameUtils
    {
        public static void SotrArray<T>(T[] array) where T : UnityEngine.Component
        {
            System.Array.Sort(array, (a, b) =>
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
}
