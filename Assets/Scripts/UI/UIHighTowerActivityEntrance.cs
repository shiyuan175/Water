using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIHighTowerActivityEntranceData : UIPanelData
	{
	}
	public partial class UIHighTowerActivityEntrance : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIHighTowerActivityEntranceData ?? new UIHighTowerActivityEntranceData();
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
				var _hta = GameActivityManager.Instance.GetActivity<HighTowerActivity>();
				_hta.StartActivity();

                UIKit.OpenPanel<UIHighTowerActivity>();
                CloseSelf();
            });
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			BtnStart.onClick.RemoveAllListeners();
			BtnClose.onClick.RemoveAllListeners();
        }
    }
}
