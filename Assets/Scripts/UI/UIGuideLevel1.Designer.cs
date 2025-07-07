using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:fea019e8-33c2-4efb-8580-3fd8e62e8e24
	public partial class UIGuideLevel1
	{
		public const string Name = "UIGuideLevel1";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle1;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle2;
		
		private UIGuideLevel1Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnBottle1 = null;
			BtnBottle2 = null;
			
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
