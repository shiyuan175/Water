using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Events;
using System.Reflection;
using Spine.Unity;
using TMPro;

namespace QFramework.Example
{
    public class UIGuideLevel1Data : UIPanelData
    {
    }
    public partial class UIGuideLevel1 : UIPanel
    {
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGuideLevel1Data ?? new UIGuideLevel1Data();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            TxtGuide.font = LevelManager.Instance.blueFont;
        }

        protected override void OnShow()
        {
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);
            //模拟点击左侧瓶子
            BtnBottle1.onClick.AddListener(() =>
            {
                LevelManager.Instance.nowBottles[0].bottle.onClick.Invoke();
                SpineHandle.transform.localPosition = new Vector3(115, 0, 0);
            });
           
            BtnBottle2.onClick.AddListener(() =>
            {
                //模拟点击右侧瓶子
                LevelManager.Instance.nowBottles[1].bottle.onClick.Invoke();
                CloseSelf();
            });
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnBottle1.onClick.RemoveAllListeners();
            BtnBottle2.onClick.RemoveAllListeners();
        }
    }
}
