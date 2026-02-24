namespace Game.Water
{
	// Generate Id:02ab2e82-eea1-41eb-ae4f-e34b06c46a5c
	public partial class UIPropMask
	{
		public const string Name = "UIPropMask";
		
		
		private UIPropMaskData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIPropMaskData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPropMaskData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPropMaskData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
