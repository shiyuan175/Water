using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIVolcanicActivityData : UIPanelData
	{
	}
	public partial class UIVolcanicActivity : UIPanel
	{
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIVolcanicActivityData ?? new UIVolcanicActivityData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			TxtLevels.font = LevelManager.Instance.redFont;
			TxtPlayers.font = LevelManager.Instance.redFont;
			Txt_Levels.font = LevelManager.Instance.redFont;
			Txt_Players.font = LevelManager.Instance.redFont;
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
