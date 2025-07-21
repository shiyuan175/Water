using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:d41289cd-2905-4e63-9ac7-eea14b745e56
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
