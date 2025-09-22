using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f7823d1a-1001-4df7-b952-077453261e8e
	public partial class UIDailyTaskADActivities
	{
		public const string Name = "UIDailyTaskADActivities";
		
		
		private UIDailyTaskADActivitiesData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIDailyTaskADActivitiesData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIDailyTaskADActivitiesData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIDailyTaskADActivitiesData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
