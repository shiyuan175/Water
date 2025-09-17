using UnityEngine;
using UnityEngine.UI;
using QFramework;
using GameDefine;

namespace QFramework.Example
{
	public class UIStreakWinGuideData : UIPanelData
	{
	}
	public partial class UIStreakWinGuide : UIPanel, ICanGetModel
	{
        private StageModel stageModel;
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIStreakWinGuideData ?? new UIStreakWinGuideData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            stageModel = this.GetModel<StageModel>();

            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            BtnPlay.onClick.AddListener(() =>
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
            int WinNum = stageModel.CountinueWinNum > GameConst.MAX_GIFT_STREAK_WIN ? GameConst.MAX_GIFT_STREAK_WIN : stageModel.CountinueWinNum;
            StringEventSystem.Global.Send(GameConst.STREAK_WIN_REMOVE_HIDE, WinNum);

			stageModel = null;
			BtnPlay.onClick.RemoveAllListeners();
            BtnClose.onClick.RemoveAllListeners();
        }

        public IArchitecture GetArchitecture()
        {
			return GameMainArc.Interface;
        }
    }
}
