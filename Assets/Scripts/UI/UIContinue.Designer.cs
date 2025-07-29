using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:de8b1294-307b-4801-8859-95724887d3a2
	public partial class UIContinue
	{
		public const string Name = "UIContinue";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnAddCoin;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoin;
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
