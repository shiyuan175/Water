using UnityEngine;

namespace Game.Water
{
	// Generate Id:4af2d562-3579-46d1-a905-130b66e7e59e
	public partial class UIGuideLevel1And2
	{
		public const string Name = "UIGuideLevel1And2";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtGuide;
		[SerializeField]
		public RectTransform Level2_Guide_Node;
		[SerializeField]
		public UnityEngine.UI.Image Img_Left;
		[SerializeField]
		public UnityEngine.UI.Image Img_Right;
		
		private UIGuideLevel1And2Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SpineHandle = null;
			TxtGuide = null;
			Level2_Guide_Node = null;
			Img_Left = null;
			Img_Right = null;
			
			mData = null;
		}
		
		public UIGuideLevel1And2Data Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGuideLevel1And2Data mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGuideLevel1And2Data());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
