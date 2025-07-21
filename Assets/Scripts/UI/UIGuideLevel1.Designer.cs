using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:c8e36cf2-b301-4aea-9e77-d6bec9d83124
	public partial class UIGuideLevel1
	{
		public const string Name = "UIGuideLevel1";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle1;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle2;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtGuide;
		
		private UIGuideLevel1Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnBottle1 = null;
			BtnBottle2 = null;
			TxtGuide = null;
			
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
