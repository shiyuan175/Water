using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f08ceb4c-333b-4fd7-a4ff-3d61b0f5f60d
	public partial class SceneUnlockGuide
	{
		public const string Name = "SceneUnlockGuide";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		
		private SceneUnlockGuideData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			
			mData = null;
		}
		
		public SceneUnlockGuideData Data
		{
			get
			{
				return mData;
			}
		}
		
		SceneUnlockGuideData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new SceneUnlockGuideData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
