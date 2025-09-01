using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:12ae10f1-4b62-41ec-a51d-714183b92340
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
