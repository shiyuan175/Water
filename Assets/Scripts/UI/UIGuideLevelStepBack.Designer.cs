using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:1c2fa083-4421-4d33-8e5f-1dd5f06e1696
	public partial class UIGuideLevelStepBack
	{
		public const string Name = "UIGuideLevelStepBack";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public RectTransform StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
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
		
		private UIGuideLevelStepBackData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			StepGetItem = null;
			BtnGet = null;
			Step1 = null;
			BtnBottle1 = null;
			Step2 = null;
			BtnBottle2 = null;
			StepItem = null;
			BtnItem = null;
			
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
