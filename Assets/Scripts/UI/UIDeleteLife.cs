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
                string _del = $"用户退出关卡:{saveData.GetLevelClear()}," +
                $"当前关卡进度:{saveData.GetLevelClear()}";
                AnalyticsManager.Instance.SendLevelEvent(_del);

                HealthManager.Instance.UseHp();
				//避免引导关退出的UI残留
				UIKit.ClosePanel<UIGuideAnimPop>();
				if (saveData.GetLevelClear() == 1 || saveData.GetLevelClear() == 2)
				{
					UIKit.ClosePanel<UIGuideLevel1>();
                    UIKit.ClosePanel<UIGuideLevel2>();
                }
                UIKit.ClosePanel<UIGameNode>();
                this.GetModel<StageModel>().ResetCountinueWinNum();
                this.SendEvent<ReturnMainEvent>(new ReturnMainEvent());
                CloseSelf();
            });
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			BtnQuit.onClick.RemoveAllListeners();
            saveData = null;
        }
    }
}
