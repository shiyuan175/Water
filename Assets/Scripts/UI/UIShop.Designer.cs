using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:c4eb011b-e282-4f78-9607-277a7996037f
	// Generate Id:887289dd-978a-42d7-9b2f-31a69c6f0cbf
	public partial class UIShop
	{
		public const string Name = "UIShop";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect ShopScrollView;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		public UnityEngine.UI.Button BtnClose;
		
		private UIShopData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ShopScrollView = null;
			BtnClose = null;
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
