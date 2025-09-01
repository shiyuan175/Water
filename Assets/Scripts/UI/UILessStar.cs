using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UILessStarData : UIPanelData
	{
		public UIPanel CurPanel;
	}

	public partial class UILessStar : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILessStarData ?? new UILessStarData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			BtnClose.onClick.RemoveAllListeners();
			BtnClose.onClick.AddListener(() =>
			{
				CloseSelf();
			});

            BtnContinue.onClick.RemoveAllListeners();
            BtnContinue.onClick.AddListener(() =>
			{
                UIKit.ClosePanel(mData.CurPanel);
				UIKit.ShowPanel<UIBegin>();
                UIKit.OpenPanel<UIBeginSelect>();
				
				CloseSelf();
			});
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
