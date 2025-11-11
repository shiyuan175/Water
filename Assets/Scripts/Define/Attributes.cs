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

        //炸弹黑水
        EBombBlackWater = 6,
        //其余特殊水块...

        Max = 7
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class WaterColorState : Attribute
    {
        public readonly bool BroomItemActive;
        public readonly bool CreateItemActive;
        public readonly bool ChangeItemActive;
        public readonly bool MagnetItemActive;
        public readonly bool RainBowWaterActive;
        public readonly bool BombBlackWaterAvtive;

        public readonly string SpineAnim;
        public readonly EColorStateSpineType SpineType;


        public WaterColorState(string spineAnim,EColorStateSpineType spineType = EColorStateSpineType.None)
        {
            SpineAnim = spineAnim;
            SpineType = spineType;

           
            BroomItemActive = spineType == EColorStateSpineType.EBroomSpine;
            CreateItemActive = spineType == EColorStateSpineType.ECreateSpine;
            ChangeItemActive = spineType == EColorStateSpineType.EChangeSpine;
            MagnetItemActive = spineType == EColorStateSpineType.EMagnetSpine;
            RainBowWaterActive = spineType == EColorStateSpineType.ERainBowWater;
            BombBlackWaterAvtive = spineType == EColorStateSpineType.EBombBlackWater;
                
            //局内机制道具补充...
        }
    }

    public class ClearItemState : WaterColorState
    {
        public readonly int TargetIndex;

        public ClearItemState(int targetIndex, string spineAnim) : base(spineAnim, EColorStateSpineType.EBroomSpine)
        {
            TargetIndex = targetIndex;
        }
    }

    public class ChangeColorItemState : WaterColorState
    {
        public readonly int TargetIndex;

        public ChangeColorItemState(int targetIndex, string spineAnim) : base(spineAnim, EColorStateSpineType.EChangeSpine)
        {
            TargetIndex = targetIndex;
        }
    }

    public class RainBowWaterState : WaterColorState
    {
        public RainBowWaterState(string spineAnim) : base(spineAnim, EColorStateSpineType.ERainBowWater)
        {

        }
    }


}