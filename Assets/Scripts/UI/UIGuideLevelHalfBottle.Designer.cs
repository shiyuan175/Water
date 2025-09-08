using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:731d5fbd-1994-4426-b523-3f1f8c305ee3
	public partial class UIGuideLevelHalfBottle
	{
		public const string Name = "UIGuideLevelHalfBottle";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public RectTransform StepItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtGuide;
		[SerializeField]
		public UnityEngine.UI.Image StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRetry;
		
		private UIGuideLevelHalfBottleData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			StepItem = null;
			BtnItem = null;
			TxtGuide = null;
			StepGetItem = null;
			BtnGet = null;
			TxtRetry = null;
			
			mData = null;
		}
		
		public UIGuideLevelHalfBottleData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevelHalfBottleData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevelHalfBottleData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
