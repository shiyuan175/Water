using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:4c94b4e2-54a5-4c1e-805d-9886252d8b4c
	public partial class UIPersonal
	{
		public const string Name = "UIPersonal";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTitle1_Blue;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTitle2_Blue;
		[SerializeField]
		public UnityEngine.UI.Button BtnHead;
		[SerializeField]
		public UnityEngine.UI.Image ImgHeadFrame;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtName1;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtName2;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		
		private UIPersonalData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtTitle1_Blue = null;
			TxtTitle2_Blue = null;
			BtnHead = null;
			ImgHeadFrame = null;
			TxtName1 = null;
			TxtName2 = null;
			BtnClose = null;
			
			mData = null;
		}
		
		public UIPersonalData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPersonalData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPersonalData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
