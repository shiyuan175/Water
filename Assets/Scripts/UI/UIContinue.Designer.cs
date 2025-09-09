using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:089db5e1-93b9-45a6-a014-7ab7d73029d5
	public partial class UIContinue
	{
		public const string Name = "UIContinue";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnAddCoin;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoin;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtWarring;
		[SerializeField]
		public UnityEngine.UI.Button BtnContinue;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRetry;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoinCost;
		[SerializeField]
		public UnityEngine.UI.Button BtnQuit;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtClose;
		
		private UIContinueData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnAddCoin = null;
			TxtCoin = null;
			TxtWarring = null;
			BtnContinue = null;
			TxtRetry = null;
			TxtCoinCost = null;
			BtnQuit = null;
			TxtClose = null;
			
			mData = null;
		}
		
		public UIContinueData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIContinueData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIContinueData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
