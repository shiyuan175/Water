using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:219784f9-7f58-411c-9de4-edb116295a84
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
