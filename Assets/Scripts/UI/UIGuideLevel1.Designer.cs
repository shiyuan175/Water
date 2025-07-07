using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:ee457644-c688-4082-a8d4-69567dbd5d6f
	public partial class UIGuideLevel1
	{
		public const string Name = "UIGuideLevel1";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle;
		
		private UIGuideLevel1Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnBottle = null;
			
			mData = null;
		}
		
		public UIGuideLevel1Data Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevel1Data mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevel1Data());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
