using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:1350ac2e-a479-42ef-9cf0-73b945b4f19c
	public partial class UIGuideLevelRemoveAll
	{
		public const string Name = "UIGuideLevelRemoveAll";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem;
		[SerializeField]
		public RectTransform StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		
		private UIGuideLevelRemoveAllData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnItem = null;
			StepGetItem = null;
			BtnGet = null;
			
			mData = null;
		}
		
		public UIGuideLevelRemoveAllData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevelRemoveAllData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevelRemoveAllData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
