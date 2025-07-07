using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:0cc9b9a0-e7df-4e6b-970c-1677180a46f8
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
		
		private UIGuideLevel2Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnBottle1 = null;
			Step2 = null;
			BtnBottle2 = null;
			Step3 = null;
			BtnBottle3 = null;
			
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
