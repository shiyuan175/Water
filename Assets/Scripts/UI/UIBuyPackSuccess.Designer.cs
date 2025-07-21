using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:a84abcbd-28a1-4956-a13d-034e3dda67b4
	public partial class UIBuyPackSuccess
	{
		public const string Name = "UIBuyPackSuccess";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTopTitle;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTopDel;
		[SerializeField]
		public UnityEngine.UI.Button BtnContinue;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtContinue;
		
		private UIBuyPackSuccessData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtTopTitle = null;
			BtnClose = null;
			TxtTopDel = null;
			BtnContinue = null;
			TxtContinue = null;
			
			mData = null;
		}
		
		public UIBuyPackSuccessData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIBuyPackSuccessData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIBuyPackSuccessData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
