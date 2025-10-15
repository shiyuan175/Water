using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:15626048-0147-41f6-9bda-1094fab24f4e
	public partial class UIRemoveAdADActivity
	{
		public const string Name = "UIRemoveAdADActivity";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnBuy;
		
		private UIRemoveAdADActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			BtnBuy = null;
			
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
