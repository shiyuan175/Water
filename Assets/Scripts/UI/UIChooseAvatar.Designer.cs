using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:1018b8ba-0e51-4c6e-b836-a9136f8b45c2
	public partial class UIChooseAvatar
	{
		public const string Name = "UIChooseAvatar";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTitle_Blue;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnSave;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtSave;
		
		private UIChooseAvatarData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtTitle_Blue = null;
			BtnClose = null;
			BtnSave = null;
			TxtSave = null;
			
			mData = null;
		}
		
		public UIChooseAvatarData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIChooseAvatarData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIChooseAvatarData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
