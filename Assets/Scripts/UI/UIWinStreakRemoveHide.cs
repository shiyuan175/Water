using QFramework;

namespace Game.Water
{
	public class UIWinStreakRemoveHideData : UIPanelData
	{
	}
	public partial class UIWinStreakRemoveHide : UIPanel
	{
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIWinStreakRemoveHideData ?? new UIWinStreakRemoveHideData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });
        }
		
		protected override void OnShow()
		{
			
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
