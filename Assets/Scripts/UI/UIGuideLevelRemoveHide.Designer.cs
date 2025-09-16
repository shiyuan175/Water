using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:fa606d9c-b617-40d8-9e9b-14bb8115f509
	public partial class UIGuideLevelRemoveHide
	{
		public const string Name = "UIGuideLevelRemoveHide";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem;
		[SerializeField]
		public RectTransform StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		
		private UIGuideLevelRemoveHideData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnItem = null;
			StepGetItem = null;
			BtnGet = null;
			
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
