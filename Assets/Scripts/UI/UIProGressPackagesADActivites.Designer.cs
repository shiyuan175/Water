using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:80dc4f81-702b-4da2-9b0d-681589f2f29e
	public partial class UIProGressPackagesADActivites
	{
		public const string Name = "UIProGressPackagesADActivites";
		
		
		private UIProGressPackagesADActivitesData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIProGressPackagesADActivitesData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIProGressPackagesADActivitesData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIProGressPackagesADActivitesData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
