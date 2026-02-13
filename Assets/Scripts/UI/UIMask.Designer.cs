namespace Game.Water
{
	// Generate Id:fc8f0108-f4d1-49e7-83cb-f72bc6aca0d5
	public partial class UIMask
	{
		public const string Name = "UIMask";
		
		
		private UIMaskData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIMaskData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMaskData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMaskData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
