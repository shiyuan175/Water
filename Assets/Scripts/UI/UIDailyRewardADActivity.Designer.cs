using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:90cc52e2-c8a6-4cbb-bf94-16217a742f32
	public partial class UIDailyRewardADActivity
	{
		public const string Name = "UIDailyRewardADActivity";
		
		
		private UIDailyRewardADActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIDailyRewardADActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIDailyRewardADActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIDailyRewardADActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
