using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:3e83b100-106c-4508-9163-34a793c90487
	public partial class UIGuideLevelRemoveAll
	{
		public const string Name = "UIGuideLevelRemoveAll";
		
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
		
		private UIGuideLevelRemoveAllData mPrivateData = null;
		
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
