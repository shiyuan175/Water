using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:cab85169-7079-4e70-aab6-fb0c172db8c9
	public partial class UIVolcanicActivityEntrance
	{
		public const string Name = "UIVolcanicActivityEntrance";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		
		private UIVolcanicActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
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
