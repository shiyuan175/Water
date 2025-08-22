using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f3d464d8-c997-4c1b-a493-8fe6e13c4523
	public partial class UIRocketActivity
	{
		public const string Name = "UIRocketActivity";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRefreshCountDown;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtDailyRefresh;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Prompt;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_PlayerWin;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Robot1Win;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Robot2Win;
		
		private UIRocketActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtRefreshCountDown = null;
			TxtDailyRefresh = null;
			Txt_Prompt = null;
			BtnClose = null;
			Txt_PlayerWin = null;
			Txt_Robot1Win = null;
			Txt_Robot2Win = null;
			
			mData = null;
		}
		
		public UIRocketActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIRocketActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIRocketActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
