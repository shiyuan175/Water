using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:56ac02c5-62cc-4928-9ed1-f6b64adc24d4
	public partial class UIGuideLevelAddBottle
	{
		public const string Name = "UIGuideLevelAddBottle";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem;
		[SerializeField]
		public RectTransform StepGetItem;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		
		private UIGuideLevelAddBottleData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnItem = null;
			StepGetItem = null;
			BtnGet = null;
			
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
