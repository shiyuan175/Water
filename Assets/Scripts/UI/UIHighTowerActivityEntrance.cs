using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIHighTowerActivityEntranceData : UIPanelData
	{
        public bool? IsManagedOpen;
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

                mData.IsManagedOpen = false;
                UIKit.OpenPanel<UIHighTowerActivity>(new UIHighTowerActivityData()
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
			BtnStart.onClick.RemoveAllListeners();
			BtnClose.onClick.RemoveAllListeners();

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }
    }
}
