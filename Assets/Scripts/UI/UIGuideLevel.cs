using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Events;
using System.Reflection;
using Spine.Unity;
using TMPro;
using Spine.Unity.Editor;

namespace QFramework.Example
{
    public class UIGuideLevelData : UIPanelData
    {
    }
    public abstract partial class UIGuideLevel : UIPanel,ICanSendEvent
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        protected abstract override void OnClose();
        protected abstract override void OnInit(IUIData uiData = null);
        protected abstract override void OnOpen(IUIData uiData = null);
        protected abstract override void OnShow();
        protected abstract override void OnHide();
        
        /// <summary>
        /// 注意，在启用的时候记得修改基类，这是留作以防后期需要，自动计算比例去移动UI
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="targetPositon"></param>
        protected void SetLocalPosition(Transform transform,Vector3 targetPositon)
        {
       /*     transform.localPosition = new Vector3(targetPositon.x / 1080 * screenWidth, targetPositon.y / 1920 * screenHeight, 0);*/
            transform.localPosition = targetPositon;
        }

        

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
