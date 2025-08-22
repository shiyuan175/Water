using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:8cba7257-f7de-4ff1-9425-2f6dca6f5baf
	public partial class UIHighTowerActivityEntrance
	{
		public const string Name = "UIHighTowerActivityEntrance";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		
		private UIHighTowerActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			BtnStart = null;
			
			mData = null;
		}
		
		public UIHighTowerActivityEntranceData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIHighTowerActivityEntranceData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIHighTowerActivityEntranceData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
