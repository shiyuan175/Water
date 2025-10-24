using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIShopData : UIPanelData
	{
	}
	public partial class UIShop : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIShopData ?? new UIShopData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			BtnClose.onClick.AddListener(() =>
			{
				CloseSelf();
			});
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			BtnClose.onClick.RemoveAllListeners();
		}
	}
}
