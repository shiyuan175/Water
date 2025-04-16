using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:472a2669-94c9-46f7-9bad-dc5931a01c85
	public partial class UIVictory
	{
		public const string Name = "UIVictory";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtLevel;
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
		public UnityEngine.UI.Button BtnClose;
		
		private UIVictoryData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtLevel = null;
			AnimGo = null;
			HornGo3 = null;
			HornSpine1 = null;
			HornGo4 = null;
			HornSpine2 = null;
			HornGo1 = null;
			HornSpine3 = null;
			HornGo2 = null;
			HornSpine4 = null;
			BtnClose = null;
			
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
