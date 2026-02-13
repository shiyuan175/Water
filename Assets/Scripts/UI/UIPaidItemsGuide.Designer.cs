using UnityEngine;

namespace Game.Water
{
	// Generate Id:9cfacc4d-6a58-4135-9f09-017f88e2bc1c
	public partial class UIPaidItemsGuide
	{
		public const string Name = "UIPaidItemsGuide";
		
		[SerializeField]
		public RectTransform GetItemGuideNode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTitle_Red;
		[SerializeField]
		public UnityEngine.UI.Image ImgItem;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtItemDetails_Red;
		[SerializeField]
		public UnityEngine.UI.Button BtnGet;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandle;
		[SerializeField]
		public UnityEngine.UI.Button BtnUseGuide;
		
		private UIPaidItemsGuideData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			GetItemGuideNode = null;
			TxtTitle_Red = null;
			ImgItem = null;
			TxtItemDetails_Red = null;
			BtnGet = null;
			SpineHandle = null;
			BtnUseGuide = null;
			
			mData = null;
		}
		
		public UIPaidItemsGuideData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPaidItemsGuideData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPaidItemsGuideData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
