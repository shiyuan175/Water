using Spine.Unity;
using UnityEngine;

namespace Game.Water
{
    public class GrassBombCtrl : MonoBehaviour
    {
        [SerializeField] SkeletonGraphic spine;

        private const string DISABLE_APPEND = "animation";

        private void OnDisable()
        {
            spine.enabled = false;
        }

        public void BombApeend()
        {
            spine.AnimationState.ClearTracks(); // 清除所有轨道
            spine.Skeleton.SetToSetupPose();
            spine.enabled = true;
        }

        public void BombDis()
        {
            spine.enabled = false;
        }
        public void Bombing()
        {
            var track = spine.AnimationState.SetAnimation(0, DISABLE_APPEND, false);
            track.Complete += track => { spine.enabled = false; };
            /*track.TimeScale = 0.7f;*/
        }
    }
}