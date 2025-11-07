using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:67c84190-9a58-4700-9781-5d9b2b97c46b
	public partial class UISepecialOfferGift
	{
		public const string Name = "UISepecialOfferGift";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnBuy;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public RectTransform TimePanel;
		[SerializeField]
		public TMPro.TextMeshProUGUI Time_Red;
		
		private UISepecialOfferGiftData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnBuy = null;
			BtnClose = null;
			TimePanel = null;
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
