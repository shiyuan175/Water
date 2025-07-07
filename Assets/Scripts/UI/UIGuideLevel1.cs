using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Events;
using System.Reflection;

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

        }

        protected override void OnShow()
        {
            //模拟点击左侧瓶子
            ActionKit.DelayFrame(1, () =>
            {
                LevelManager.Instance.nowBottles[0].bottle.onClick.Invoke();

            }).Start(this);
            BtnBottle.onClick.AddListener(() =>
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
            BtnBottle.onClick.RemoveAllListeners();

        }
    }
}
