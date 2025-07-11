using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:222a7e38-669b-4a54-b230-9d8d101c4198
	public partial class UIVolcanicActivityEntrance
	{
		public const string Name = "UIVolcanicActivityEntrance";
		
		
		private UIVolcanicActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
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
