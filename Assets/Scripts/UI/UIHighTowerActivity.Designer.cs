using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:694d1055-b890-48bb-a74d-b1312c10d60b
	public partial class UIHighTowerActivity
	{
		public const string Name = "UIHighTowerActivity";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		
		private UIHighTowerActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			TxtCountDown = null;
			
			mData = null;
		}
		
		public UIHighTowerActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIHighTowerActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIHighTowerActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
