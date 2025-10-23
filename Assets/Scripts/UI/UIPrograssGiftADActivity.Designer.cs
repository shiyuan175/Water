using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:4556f317-9530-46c1-9dd6-a438810fd172
	public partial class UIPrograssGiftADActivity
	{
		public const string Name = "UIPrograssGiftADActivity";
		
		[SerializeField]
		public RectTransform TopPanel;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.RectTransform ButtomPanel;
		[SerializeField]
		public RectTransform control;
		[SerializeField]
		public UnityEngine.UI.Button BtnBuy;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Red;
		[SerializeField]
		public UnityEngine.UI.Image ImgLock;
		
		private UIPrograssGiftADActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TopPanel = null;
			TxtCountDown = null;
			BtnClose = null;
			ButtomPanel = null;
			control = null;
			BtnBuy = null;
			Txt_Red = null;
			ImgLock = null;
			
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
