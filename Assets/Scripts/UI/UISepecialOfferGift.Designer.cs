using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:e79b4412-e36c-4561-a7aa-2bb29b538134
	public partial class UISepecialOfferGift
	{
		public const string Name = "UISepecialOfferGift";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI Time_Red;
		
		private UISepecialOfferGiftData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
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
