using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIMaskData : UIPanelData
	{
	}
	public partial class UIMask : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMaskData ?? new UIMaskData();
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
