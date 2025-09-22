using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIDailyTaskADActivitiesData : UIPanelData
	{
	}
	public partial class UIDailyTaskADActivities : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIDailyTaskADActivitiesData ?? new UIDailyTaskADActivitiesData();
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
