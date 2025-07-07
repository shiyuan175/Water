using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:0ac67219-03db-4cc5-965b-74610b4ee183
	public partial class UIGuideLevel1
	{
		public const string Name = "UIGuideLevel1";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnBottle;
		
		private UIGuideLevel1Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnBottle = null;
			
			mData = null;
		}
		
		public UIGuideLevel1Data Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevel1Data mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevel1Data());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
