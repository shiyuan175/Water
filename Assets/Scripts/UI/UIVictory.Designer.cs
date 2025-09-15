using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:a949f4c3-2ee6-4d30-b27f-7329580c5efa
	public partial class UIVictory
	{
		public const string Name = "UIVictory";
		
		[SerializeField]
		public Animator AnimGo;
		[SerializeField]
		public Animator HornGo3;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic HornSpine1;
		[SerializeField]
		public Animator HornGo4;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic HornSpine2;
		[SerializeField]
		public Animator HornGo1;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic HornSpine3;
		[SerializeField]
		public Animator HornGo2;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic HornSpine4;
		[SerializeField]
		public UnityEngine.UI.Button BtnSkip;
		
		private UIVictoryData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			AnimGo = null;
			HornGo3 = null;
			HornSpine1 = null;
			HornGo4 = null;
			HornSpine2 = null;
			HornGo1 = null;
			HornSpine3 = null;
			HornGo2 = null;
			HornSpine4 = null;
			BtnSkip = null;
			
			mData = null;
		}
		
		public UIVictoryData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIVictoryData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIVictoryData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
