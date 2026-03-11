using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
    public class UIPopUpWindowData : UIPanelData
    {
    }

    public partial class UIPopUpWindow : UIPanel
    {
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIPopUpWindowData ?? new UIPopUpWindowData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
            ActionKit.Delay(2, () => { CloseSelf(); }).Start(this);
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }
    }
}