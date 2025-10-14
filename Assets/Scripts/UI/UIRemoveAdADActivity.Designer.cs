using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:cc867530-9bd6-4bf2-873b-0bde1c67f57a
	public partial class UIRemoveAdADActivity
	{
		public const string Name = "UIRemoveAdADActivity";
		
		
		private UIRemoveAdADActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIRemoveAdADActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIRemoveAdADActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIRemoveAdADActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
