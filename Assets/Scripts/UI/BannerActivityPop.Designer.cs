using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:39dff6c5-0365-44d7-a60f-94407ee8eb9d
	public partial class BannerActivityPop
	{
		public const string Name = "BannerActivityPop";
		
		[SerializeField]
		public UnityEngine.UI.Image ImgCup;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtNum;
		[SerializeField]
		public UnityEngine.UI.Image ImgDouble;
		
		private BannerActivityPopData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ImgCup = null;
			TxtNum = null;
			ImgDouble = null;
			
			mData = null;
		}
		
		public BannerActivityPopData Data
		{
			get
			{
				return mData;
			}
		}
		
		BannerActivityPopData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new BannerActivityPopData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
