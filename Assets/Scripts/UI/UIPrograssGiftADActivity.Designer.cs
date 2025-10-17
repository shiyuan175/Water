using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:8d63799a-9e42-4212-aec6-f66fbe7d2867
	public partial class UIPrograssGiftADActivity
	{
		public const string Name = "UIPrograssGiftADActivity";
		
		
		private UIPrograssGiftADActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIPrograssGiftADActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPrograssGiftADActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPrograssGiftADActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
