using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:b31a2384-a2d2-44ce-a985-1ed023636953
	public partial class UIRocketActivity
	{
		public const string Name = "UIRocketActivity";
		
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
