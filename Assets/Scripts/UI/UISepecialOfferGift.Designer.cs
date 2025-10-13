using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:dd849baa-779c-4c1a-8892-7448ae2921f1
	public partial class UISepecialOfferGift
	{
		public const string Name = "UISepecialOfferGift";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnBuy;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI Time_Red;
		
		private UISepecialOfferGiftData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnBuy = null;
			BtnClose = null;
			Time_Red = null;
			
			mData = null;
		}
		
		public UISepecialOfferGiftData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISepecialOfferGiftData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISepecialOfferGiftData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
