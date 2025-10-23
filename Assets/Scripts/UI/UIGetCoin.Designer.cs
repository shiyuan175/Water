using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:bcbb55f8-7321-44f0-bcd7-30818ce85d72
	public partial class UIGetCoin
	{
		public const string Name = "UIGetCoin";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtLevel;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoin;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextTimes;
		[SerializeField]
		public UnityEngine.GameObject ImgBoxProcessNode;
		[SerializeField]
		public UnityEngine.UI.Image ImgProcess;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtProcess;
		[SerializeField]
		public UnityEngine.GameObject ImgUnlockProcessNode;
		[SerializeField]
		public UnityEngine.UI.Image ImgUnlockProcess;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtUnlockProcess;
		[SerializeField]
		public UnityEngine.UI.Image ImgUnlock;
		[SerializeField]
		public UnityEngine.UI.Button BtnContinue;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtContinue;
		[SerializeField]
		public UnityEngine.UI.Image NewItemNode;
		[SerializeField]
		public UnityEngine.UI.Image ImgNewItem;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtNewItem_Red;
		[SerializeField]
		public UnityEngine.UI.Button BtnNewItemClose;
		
		private UIGetCoinData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			TxtLevel = null;
			TxtCoin = null;
			TextTimes = null;
			ImgBoxProcessNode = null;
			ImgProcess = null;
			TxtProcess = null;
			ImgUnlockProcessNode = null;
			ImgUnlockProcess = null;
			TxtUnlockProcess = null;
			ImgUnlock = null;
			BtnContinue = null;
			TxtContinue = null;
			NewItemNode = null;
			ImgNewItem = null;
			TxtNewItem_Red = null;
			BtnNewItemClose = null;
			
			mData = null;
		}
		
		public UIGetCoinData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGetCoinData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGetCoinData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
