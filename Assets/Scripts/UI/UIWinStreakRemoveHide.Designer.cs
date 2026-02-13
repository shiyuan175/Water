using UnityEngine;

namespace Game.Water
{
	// Generate Id:664a1cc1-9956-49fe-b228-08f77de747f1
	public partial class UIWinStreakRemoveHide
	{
		public const string Name = "UIWinStreakRemoveHide";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		
		private UIWinStreakRemoveHideData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			
			mData = null;
		}
		
		public UIWinStreakRemoveHideData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIWinStreakRemoveHideData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIWinStreakRemoveHideData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
