using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIDeleteLifeData : UIPanelData
	{
	}
	public partial class UIDeleteLife : UIPanel, ICanSendEvent, IController
    {
		private SaveDataUtility saveData;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIDeleteLifeData ?? new UIDeleteLifeData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
            saveData = this.GetUtility<SaveDataUtility>();
            BtnClose.onClick.AddListener(() =>
			{
                CloseSelf();
            });
			BtnQuit.onClick.AddListener(() =>
			{
                string _del = $"用户退出关卡:{saveData.GetCurrentLevel()}," +
                $"当前关卡进度:{saveData.GetCurrentLevel()}";
                AnalyticsManager.Instance.SendLevelEvent(_del);

                HealthManager.Instance.UseHp();
				//避免引导关退出的UI残留
				UIKit.ClosePanel<UIGuideAnimPop>();
				if (saveData.GetCurrentLevel() == 1 || saveData.GetCurrentLevel() == 2)
				{
					UIKit.ClosePanel<UIGuideLevel1>();
                    UIKit.ClosePanel<UIGuideLevel2>();
                }
                UIKit.ClosePanel<UIGameNode>();
                this.GetModel<StageModel>().ResetCountinueWinNum();
                this.SendEvent<ReturnMainEvent>(new ReturnMainEvent());

                if (GameActivityManager.Instance.GetActivity<VolcanicActivity>() is VolcanicActivity volcanicActivity
                && volcanicActivity.ActivityStatus == GameActivityStatus.Active)
				{
                    UIKit.OpenPanel<UIVolcanicActivity>(new UIVolcanicActivityData()
                    {
                        isSuceed = false
                    });
                }
                    
                CloseSelf();
            });
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			BtnClose.onClick.RemoveAllListeners();
            BtnQuit.onClick.RemoveAllListeners();
            saveData = null;
        }
    }
}
