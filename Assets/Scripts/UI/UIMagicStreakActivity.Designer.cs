using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:e5c7a4bc-49bf-4e10-bfbc-2a4df5b2dc20
	public partial class UIMagicStreakActivity
	{
		public const string Name = "UIMagicStreakActivity";

        [Header("Bind UI")]
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
		[SerializeField]
		public UnityEngine.UI.Image ImgRewardUI;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRewardNum;
		
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
			ImgRewardUI = null;
			TxtRewardNum = null;
			
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
