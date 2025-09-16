using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:579f1d35-17d9-4fce-b977-2ad7264f3d32
	public partial class UITierRankActivityEntrance
	{
		public const string Name = "UITierRankActivityEntrance";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Image ImgRankSprite;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		
		private UITierRankActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			ImgRankSprite = null;
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
