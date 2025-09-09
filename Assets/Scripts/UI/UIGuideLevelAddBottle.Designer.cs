using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:bacfe06a-da64-48e9-a1d8-007c6ca43d84
	public partial class UIGuideLevelAddBottle
	{
		public const string Name = "UIGuideLevelAddBottle";
		
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
		
		private UIGuideLevelAddBottleData mPrivateData = null;
		
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
		
		public UIGuideLevelAddBottleData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevelAddBottleData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevelAddBottleData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
