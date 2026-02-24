using System;

namespace Game.Water
{
    public enum EColorStateSpineType
    {
        None = 0,
        EBroomSpine = 1,
        EMagnetSpine = 2,
        ECreateSpine = 3,
        EChangeSpine = 4,

        // 彩虹水
        ERainBowWater = 5,
        EFlashWater = 6,

        // 炸弹水
        EBombBlackWater = 7,

        // 飞天炸弹
        EFlyBomb = 8,

        // 草地水
        EGrassWaterBomb = 9,
        // 更多机制

        Max = 10
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
        public readonly bool FlashWaterActive;
        public readonly bool GrassWaterBombActive;
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
            FlashWaterActive = spineType == EColorStateSpineType.EFlashWater;
            BombBlackWaterAvtive = spineType == EColorStateSpineType.EBombBlackWater;
            GrassWaterBombActive = spineType == EColorStateSpineType.EGrassWaterBomb;
            // 其他新的水机制
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
        public RainBowWaterState(string spineAnim,EColorStateSpineType EColor = EColorStateSpineType.ERainBowWater) : base(spineAnim, EColor)
        {
           
        }
    }
    
    
    
}
