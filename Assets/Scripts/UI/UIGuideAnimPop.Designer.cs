using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:cb55e26d-fca9-431f-b0d8-518b152a1d01
	public partial class UIGuideAnimPop
	{
		public const string Name = "UIGuideAnimPop";
		
		[SerializeField]
		public UnityEngine.UI.Text TxtGuide;
		[SerializeField]
		public UnityEngine.RectTransform GuideArrow;
		
		private UIGuideAnimPopData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtGuide = null;
			GuideArrow = null;
			
			mData = null;
		}
		
		public UIGuideAnimPopData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideAnimPopData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideAnimPopData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
