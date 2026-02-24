using System.Collections.Generic;
using Game.Water;
using UnityEngine;

namespace Game.Water
{
    public class HideWaterCtrl : MonoBehaviour
    {
        public List<GameObject> blackWaterGos;
        private int _hideType = 0;

        public void SetHideShow(HideWaterType hideType)
        {
            if (blackWaterGos[_hideType])
                blackWaterGos[_hideType]?.SetActive(false);
            _hideType = (int)hideType;
            if (blackWaterGos[_hideType])
                blackWaterGos[_hideType]?.SetActive(true);
        }
    }
}