using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:887289dd-978a-42d7-9b2f-31a69c6f0cbf
	public partial class UIShop
	{
		public const string Name = "UIShop";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		
		private UIShopData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			
			mData = null;
		}
		
		public UIShopData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIShopData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIShopData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
