using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:0acb091a-487a-49ab-9c3e-47af74f1e3e6
	public partial class BannerActivityPop
	{
		public const string Name = "BannerActivityPop";
		
		[SerializeField]
		public UnityEngine.UI.Image ImgCup;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtNum;
		
		private BannerActivityPopData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ImgCup = null;
			TxtNum = null;
			
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
