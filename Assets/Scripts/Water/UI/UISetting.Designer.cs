using UnityEngine;

namespace Game.Water
{
	// Generate Id:7837cf9b-98b3-4356-abe9-eb9b17bb3f68
	public partial class UISetting
	{
		public const string Name = "UISetting";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnSelect;
		[SerializeField]
		public UnityEngine.UI.Image ImgSelected;
		
		private UISettingData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			BtnSelect = null;
			ImgSelected = null;
			
			mData = null;
		}
		
		public UISettingData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISettingData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISettingData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
