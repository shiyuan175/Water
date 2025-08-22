using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIVolcanicActivityEntranceData : UIPanelData
	{
	}
	public partial class UIVolcanicActivityEntrance : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIVolcanicActivityEntranceData ?? new UIVolcanicActivityEntranceData();
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

			BtnStart.onClick.AddListener(() =>
            {
				var _va = GameActivityManager.Instance.GetActivity<VolcanicActivity>();
				_va.StartActivity();

                UIKit.OpenPanel<UIVolcanicActivity>();
                CloseSelf();
            });
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			BtnClose.onClick.RemoveAllListeners();
			BtnStart.onClick.RemoveAllListeners();
		}
	}
}
