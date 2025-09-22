using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:43c8642c-7b50-4880-8614-3af0a1e59124
	public partial class PlotUnlockGuide
	{
		public const string Name = "SceneUnlockGuide";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnStep1;
		[SerializeField]
		public UnityEngine.UI.Button BtnStep2;
		
		private PlotUnlockGuideData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			BtnStep1 = null;
			BtnStep2 = null;
			
			mData = null;
		}
		
		public PlotUnlockGuideData Data
		{
			get
			{
				return mData;
			}
		}
		
		PlotUnlockGuideData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new PlotUnlockGuideData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
