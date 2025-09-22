using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:634fcc66-f531-4993-a5a1-18a768424b7b
	public partial class TestUIProGressPackagesADActivites
	{
		public const string Name = "TestUIProGressPackagesADActivites";
		
		
		private TestUIProGressPackagesADActivitesData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public TestUIProGressPackagesADActivitesData Data
		{
			get
			{
				return mData;
			}
		}
		
		TestUIProGressPackagesADActivitesData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new TestUIProGressPackagesADActivitesData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
