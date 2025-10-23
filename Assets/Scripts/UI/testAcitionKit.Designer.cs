using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:11e4cd27-b759-4f5b-a982-713e2cdff0d3
	public partial class testAcitionKit
	{
		public const string Name = "testAcitionKit";
		
		
		private testAcitionKitData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public testAcitionKitData Data
		{
			get
			{
				return mData;
			}
		}
		
		testAcitionKitData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new testAcitionKitData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
