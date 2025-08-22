using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:b17ad752-3bb5-4a4a-b5ad-c5cfb0cec9de
	public partial class UIMagicStreakActivity
	{
		public const string Name = "UIMagicStreakActivity";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect RankScrollRect;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTitle_Blue;
		[SerializeField]
		public UnityEngine.RectTransform ImgSelected;
		[SerializeField]
		public UnityEngine.UI.Image ImgProgressBar;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtProgress_Red;
		
		private UIMagicStreakActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			RankScrollRect = null;
			BtnClose = null;
			TxtCountDown = null;
			TxtTitle_Blue = null;
			ImgSelected = null;
			ImgProgressBar = null;
			TxtProgress_Red = null;
			
			mData = null;
		}
		
		public UIMagicStreakActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMagicStreakActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMagicStreakActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
