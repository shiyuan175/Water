using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameAttributes
{
    public enum EColorStateSpineType
    {
        None = 0,
        EBroomSpine = 1,
        EMagnetSpine = 2,
        ECreateSpine = 3,
        EChangeSpine = 4,

        //彩色水块
        ERainBowWater = 5,
        //其余特殊水块...
        
        Max = 6,
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class WaterColorState : Attribute
    {
        public readonly bool BroomItemActive;
        public readonly bool CreateItemActive;
        public readonly bool ChangeItemActive;
        public readonly bool MagnetItemActive;
        public readonly bool RainBowWaterActive;
        
        // 是否是两两合成道具
        public readonly bool PairCombine; 
        public readonly string SpineAnim;
        public readonly EColorStateSpineType SpineType;

        public WaterColorState(string spineAnim,EColorStateSpineType spineType = EColorStateSpineType.None, bool pairCombine = true)
        {
            SpineAnim = spineAnim;
            SpineType = spineType;
            PairCombine = pairCombine;

            BroomItemActive = spineType == EColorStateSpineType.EBroomSpine;
            CreateItemActive = spineType == EColorStateSpineType.ECreateSpine;
            ChangeItemActive = spineType == EColorStateSpineType.EChangeSpine;
            MagnetItemActive = spineType == EColorStateSpineType.EMagnetSpine;
            RainBowWaterActive = spineType == EColorStateSpineType.ERainBowWater;
            //局内机制道具补充...
        }
    }

    public class ClearItemState : WaterColorState
    {
        public readonly int TargetIndex;

        public ClearItemState(int targetIndex, string spineAnim, bool pairCombine = true) : base(spineAnim, EColorStateSpineType.EBroomSpine, pairCombine)
        {
            TargetIndex = targetIndex;
        }
    }

    public class ChangeColorItemState : WaterColorState
    {
        public readonly int TargetIndex;

        public ChangeColorItemState(int targetIndex, string spineAnim, bool pairCombine = true) : base(spineAnim, EColorStateSpineType.EChangeSpine, pairCombine)
        {
            TargetIndex = targetIndex;
        }
    }
}