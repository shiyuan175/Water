using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:06435e2f-9b54-428f-ae79-2a157dc23a9d
	public partial class UITierRankActivity
	{
		public const string Name = "UITierRankActivity";
		
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
