using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:9d6e790b-48b9-4ae3-9fd0-67a8a5f6e420
	public partial class UIVolcanicActivity
	{
		public const string Name = "UIVolcanicActivity";
		
		
		private UIVolcanicActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIVolcanicActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIVolcanicActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIVolcanicActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
