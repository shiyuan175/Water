using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f2ad3299-3724-4480-a095-a5a32da7e726
	public partial class UISepecialOfferGift
	{
		public const string Name = "UISepecialOfferGift";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnBuy;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		
		private UISepecialOfferGiftData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnBuy = null;
			BtnClose = null;
			
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
