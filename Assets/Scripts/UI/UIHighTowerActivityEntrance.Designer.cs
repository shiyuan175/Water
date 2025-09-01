using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f7903564-262c-4f7e-a038-e4c6e5dcb326
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
