using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class PlotUnlockGuideData : UIPanelData
	{
	}
	public partial class PlotUnlockGuide : UIPanel
	{
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as PlotUnlockGuideData ?? new PlotUnlockGuideData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            SpineHandle.transform.position = BtnStep1.transform.position;
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);
			BtnStep2.interactable = false;

            BtnStep1.onClick.AddListener(() =>
            {
                BtnStep1.interactable = false;
                StringEventSystem.Global.Send(GameDefine.GameConst.SCENE_UNLOCK_GUIDE_STEP1);
                BtnStep2.interactable = true;
                SpineHandle.transform.position = BtnStep2.transform.position;
            });

            BtnStep2.onClick.AddListener(() =>
			{
				BtnStep2.interactable = false;
                StringEventSystem.Global.Send(GameDefine.GameConst.SCENE_UNLOCK_GUIDE_STEP2);
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
		}
	}
}
