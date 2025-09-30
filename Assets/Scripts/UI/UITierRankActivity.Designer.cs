using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:14617090-4533-42d9-8d12-9a06c75a79a9
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
		public UnityEngine.UI.Button BtnRewardInfo;
		[SerializeField]
		public UnityEngine.UI.Image RewardInfo;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRewardTip_Blue;
		[SerializeField]
		public UnityEngine.UI.Button BtnCloseRewardInfo;
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
			BtnRewardInfo = null;
			RewardInfo = null;
			TxtRewardTip_Blue = null;
			BtnCloseRewardInfo = null;
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
