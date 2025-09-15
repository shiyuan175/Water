using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f2b2229e-f27e-41c5-9182-767b52b11590
	public partial class UITierRankActivity
	{
		public const string Name = "UITierRankActivity";
		
		[SerializeField]
		public UnityEngine.UI.Image ImgTierRankIcon_Top;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		[SerializeField]
		public UnityEngine.UI.Button BtnClaimReward;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtClaimReward_Red;
		
		private UITierRankActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ImgTierRankIcon_Top = null;
			BtnClose = null;
			TxtCountDown = null;
			BtnClaimReward = null;
			TxtClaimReward_Red = null;
			
			mData = null;
		}
		
		public UITierRankActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITierRankActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITierRankActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
