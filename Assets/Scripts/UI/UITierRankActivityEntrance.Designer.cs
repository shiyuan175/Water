using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:0049ec89-95fb-4063-812c-5779b4ca15da
	public partial class UITierRankActivityEntrance
	{
		public const string Name = "UITierRankActivityEntrance";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		
		private UITierRankActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			TxtCountDown = null;
			BtnStart = null;
			
			mData = null;
		}
		
		public UITierRankActivityEntranceData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITierRankActivityEntranceData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITierRankActivityEntranceData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
