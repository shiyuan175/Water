using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:a2d84433-6172-453a-a40a-f66b35837ea6
	public partial class UIDoubleGiftADActivity
	{
		public const string Name = "UIDoubleGiftADActivity";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnBuy;
		[SerializeField]
		public UnityEngine.UI.Button BtnFree;
		
		private UIDoubleGiftADActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			BtnBuy = null;
			BtnFree = null;
			
			mData = null;
		}
		
		public UIDoubleGiftADActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIDoubleGiftADActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIDoubleGiftADActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
