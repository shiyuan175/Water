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
        Max = 5,
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class WaterColorState : Attribute
    {
        public readonly bool BroomItemActive;
        public readonly bool CreateItemActive;
        public readonly bool ChangeItemActive;
        public readonly bool MagnetItemActive;
        public readonly string SpineAnim;
        public readonly EColorStateSpineType SpineType;

        public WaterColorState(bool broomItemActive, bool createItemActive, bool changeItemActive, 
            bool magnetItemActive, string spineAnim, EColorStateSpineType spineType = EColorStateSpineType.None)
        {
            BroomItemActive = broomItemActive;
            CreateItemActive = createItemActive;
            ChangeItemActive = changeItemActive;
            MagnetItemActive = magnetItemActive;
            SpineAnim = spineAnim;
            SpineType = spineType;
        }
    }

    public class ClearItemState : WaterColorState
    {
        public readonly int TargetIndex;

        public ClearItemState(int targetIndex, string spineAnim)
            : base(true, false, false, false, spineAnim, EColorStateSpineType.EBroomSpine)
        {
            TargetIndex = targetIndex;
        }
    }

    public class ChangeColorItemState : WaterColorState
    {
        public readonly int TargetIndex;

        public ChangeColorItemState(int targetIndex, string spineAnim)
            : base(false, false, true, false, spineAnim, EColorStateSpineType.EChangeSpine)
        {
            TargetIndex = targetIndex;
        }
    }
}