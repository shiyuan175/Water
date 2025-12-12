using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIRocketActivityEntranceData : UIPanelData
	{
        public bool? IsManagedOpen;
    }
    public partial class UIRocketActivityEntrance : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIRocketActivityEntranceData ?? new UIRocketActivityEntranceData();
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
                var _ra = GameActivityManager.Instance.GetActivity<RocketActivity>();
                _ra.StartActivity();

                mData.IsManagedOpen = false;
                UIKit.OpenPanel<UIRocketActivity>(new UIRocketActivityData()
                {
                    IsManagedOpen = true
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
