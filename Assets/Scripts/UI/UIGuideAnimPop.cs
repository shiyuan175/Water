using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIGuideAnimPopData : UIPanelData
	{
		public string GuideText;
		public string GuideAnimName;
    }
	public partial class UIGuideAnimPop : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGuideAnimPopData ?? new UIGuideAnimPopData();
			// please add init code here
		}

		protected override void OnOpen(IUIData uiData = null)
		{
            TxtGuide.text = mData.GuideText;
			GuideAni.Play(mData.GuideAnimName);
        }
		
		protected override void OnShow()
		{
            ActionKit.Delay(5f, () =>
            {
                CloseSelf();
            }).Start(this);
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
        }
    }
}
