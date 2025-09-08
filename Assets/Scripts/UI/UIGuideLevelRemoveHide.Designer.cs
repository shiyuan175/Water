using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:760b8bf0-8e67-40b0-9e33-162989c19d2e
	public partial class UIGuideLevelRemoveHide
	{
		public const string Name = "UIGuideLevelRemoveHide";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Image StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRetry;
		[SerializeField]
		public RectTransform StepItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtGuide;
		
		private UIGuideLevelRemoveHideData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			StepGetItem = null;
			BtnGet = null;
			TxtRetry = null;
			StepItem = null;
			BtnItem = null;
			TxtGuide = null;
			
			mData = null;
		}
		
		public UIGuideLevelRemoveHideData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevelRemoveHideData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevelRemoveHideData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
