using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:423a54f7-c84e-443d-9cc1-a4d293f95fb4
	public partial class UIPopUpWindow
	{
		public const string Name = "UIPopUpWindow";
		
		
		private UIPopUpWindowData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIPopUpWindowData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPopUpWindowData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPopUpWindowData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
