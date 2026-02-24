using QFramework;

namespace Game.Water
{
	public class UIPropMaskData : UIPanelData
	{
	}
	public partial class UIPropMask : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIPropMaskData ?? new UIPropMaskData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
