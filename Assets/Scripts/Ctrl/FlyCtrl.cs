using DG.Tweening;
using Game.Water;
using UnityEngine;

namespace Game.Water
{
    public class FlyCtrl : MonoBehaviour
    {
        public Transform target;
        public float flyTime;

        public void BeginFly()
        {

            var tween = transform.DOMove(target.position, flyTime)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    LevelManager.Instance.isPlayFxAnim = false;
                    Destroy(gameObject);
                });
        }
    }
}
