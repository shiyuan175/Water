using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f5b5f18e-2ac8-4eea-b9ca-0ca7b5289df9
	public partial class UIHighTowerActivity
	{
		public const string Name = "UIHighTowerActivity";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		
		private UIHighTowerActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			
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
