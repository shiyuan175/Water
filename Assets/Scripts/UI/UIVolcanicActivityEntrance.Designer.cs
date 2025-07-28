using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:7e98fbf6-fba3-4588-a3b9-53384edbfc59
	public partial class UIVolcanicActivityEntrance
	{
		public const string Name = "UIVolcanicActivityEntrance";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		
		private UIVolcanicActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnStart = null;
			
			mData = null;
		}
		
		public UIVolcanicActivityEntranceData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIVolcanicActivityEntranceData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIVolcanicActivityEntranceData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
