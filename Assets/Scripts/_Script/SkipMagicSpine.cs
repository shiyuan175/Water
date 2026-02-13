using Game.Water;
using QFramework;
using UnityEngine;

namespace Game.Water
{
    public class SkipMagicSpine : MonoBehaviour
    {
        private BottleWaterCtrl mCacheWater;

        private void Awake()
        {
            StringEventSystem.Global.Register<BottleWaterCtrl>("CacheMagnetWater", (water) =>
            {
                mCacheWater = water;

            }).UnRegisterWhenGameObjectDestroyed(this);
            this.Hide();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SkipSpine();
            }
        }

        private void OnDisable()
        {
            mCacheWater = null;
        }

        private void SkipSpine()
        {
            AudioKit.StopAllSound();
            StopCoroutine(LevelManager.Instance.ShowMahoujinCoroutine());
            if(mCacheWater != null) mCacheWater.StopPlayUseMagnet();
            LevelManager.Instance.isPlayFxAnim = false;
            UIKit.ClosePanel<UIMask>();
            LevelManager.Instance.RemoveAll();
            this.Hide();
        }
    }
}
