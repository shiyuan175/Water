using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:1d49a7a2-7230-446f-931a-3bf4687230c1
	public partial class UIGuideLevelHalfBottle
	{
		public const string Name = "UIGuideLevelHalfBottle";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem;
		[SerializeField]
		public RectTransform StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		
		private UIGuideLevelHalfBottleData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnItem = null;
			StepGetItem = null;
			BtnGet = null;
			
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
