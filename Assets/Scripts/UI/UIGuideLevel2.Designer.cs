using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f0d737a0-c757-42df-ac75-3848114bd7ee
	public partial class UIGuideLevel2
	{
		public const string Name = "UIGuideLevel2";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle1;
		[SerializeField]
		public RectTransform Step2;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle2;
		[SerializeField]
		public RectTransform Step3;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle3;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtGuide;
		
		private UIGuideLevel2Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnBottle1 = null;
			Step2 = null;
			BtnBottle2 = null;
			Step3 = null;
			BtnBottle3 = null;
			TxtGuide = null;
			
			mData = null;
		}
		
		public UIGuideLevel2Data Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevel2Data mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevel2Data());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
