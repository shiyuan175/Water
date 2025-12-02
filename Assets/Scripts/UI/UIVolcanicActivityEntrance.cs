using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIVolcanicActivityEntranceData : UIPanelData
	{
        public bool? IsManagedOpen;
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

                mData.IsManagedOpen = false;
                UIKit.OpenPanel<UIVolcanicActivity>(new UIVolcanicActivityData()
                {
                    IsManagedOpen = true,
                });
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

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }
	}
}
