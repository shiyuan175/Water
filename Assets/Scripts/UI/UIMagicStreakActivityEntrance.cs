using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIMagicStreakActivityEntranceData : UIPanelData
	{
	}
	public partial class UIMagicStreakActivityEntrance : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMagicStreakActivityEntranceData ?? new UIMagicStreakActivityEntranceData();
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
				var _msa = GameActivityManager.Instance.GetActivity<MagicStreakActivity>();
				_msa.StartActivity();

				UIKit.OpenPanel<UIMagicStreakActivity>();
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
