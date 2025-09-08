using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:b62d3b2a-d236-45d4-b22a-a7d19ddac113
	public partial class UIGuideLevelStepBack
	{
		public const string Name = "UIGuideLevelStepBack";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Image StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRetry;
		[SerializeField]
		public RectTransform Step1;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle1;
		[SerializeField]
		public RectTransform Step2;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle2;
		[SerializeField]
		public RectTransform StepItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtGuide;
		
		private UIGuideLevelStepBackData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			StepGetItem = null;
			BtnGet = null;
			TxtRetry = null;
			Step1 = null;
			BtnBottle1 = null;
			Step2 = null;
			BtnBottle2 = null;
			StepItem = null;
			BtnItem = null;
			TxtGuide = null;
			
			mData = null;
		}
		
		public UIGuideLevelStepBackData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevelStepBackData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevelStepBackData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
