using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:36766bc0-ae5f-4891-97be-dca98ad7c20b
	public partial class UIShop
	{
		public const string Name = "UIShop";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect ShopScrollView;
		
		private UIShopData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ShopScrollView = null;
			
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
