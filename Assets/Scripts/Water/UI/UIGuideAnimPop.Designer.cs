using UnityEngine;

namespace Game.Water
{
	// Generate Id:96fd3fe5-a6e5-49e6-97b7-205c5362fe0f
	public partial class UIGuideAnimPop
	{
		public const string Name = "UIGuideAnimPop";
		
		[SerializeField]
		public UnityEngine.UI.Text TxtGuide;
		[SerializeField]
		public UnityEngine.RectTransform GuideArrow;
		[SerializeField]
		public UnityEngine.UI.Button SkipBtn;
		
		private UIGuideAnimPopData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtGuide = null;
			GuideArrow = null;
			SkipBtn = null;
			
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
