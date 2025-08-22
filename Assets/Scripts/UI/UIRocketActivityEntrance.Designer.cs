using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:b9809861-a630-4dca-b01d-46c727454c74
	public partial class UIRocketActivityEntrance
	{
		public const string Name = "UIRocketActivityEntrance";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		
		private UIRocketActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			BtnStart = null;
			
			mData = null;
		}
		
		public UIRocketActivityEntranceData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIRocketActivityEntranceData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIRocketActivityEntranceData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
