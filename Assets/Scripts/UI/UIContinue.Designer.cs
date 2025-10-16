using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:be299b81-721f-4ccb-b928-f0b6cb5931d9
	public partial class UIContinue
	{
		public const string Name = "UIContinue";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnAddCoin;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoin;
		[SerializeField]
		public UnityEngine.UI.Image ImgRankIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtWarring;
		[SerializeField]
		public UnityEngine.UI.Button BtnContinue;
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
			ImgRankIcon = null;
			TxtWarring = null;
			BtnContinue = null;
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
