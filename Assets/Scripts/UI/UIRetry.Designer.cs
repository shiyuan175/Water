using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:96514af6-bb3b-43bf-bad9-75dc1a3a1eef
	public partial class UIRetry
	{
		public const string Name = "UIRetry";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoin;
		[SerializeField]
		public UnityEngine.UI.Button BtnAddCoin;
		[SerializeField]
		public UnityEngine.UI.Button BtnGiveUp;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtGiveUp;
		[SerializeField]
		public UnityEngine.UI.Button BtnAddBottle;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRetry;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoinCost;
		
		private UIRetryData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			TxtCoin = null;
			BtnAddCoin = null;
			BtnGiveUp = null;
			TxtGiveUp = null;
			BtnAddBottle = null;
			TxtRetry = null;
			TxtCoinCost = null;
			
			mData = null;
		}
		
		public UIRetryData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIRetryData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIRetryData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
