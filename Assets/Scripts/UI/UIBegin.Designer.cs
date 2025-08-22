using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:23f6bbde-09d8-49ac-8ff9-5306c73574a9
	public partial class UIBegin
	{
		public const string Name = "UIBegin";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect ShopScrollView;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtStart;
		[SerializeField]
		public UnityEngine.UI.Button BtnArea;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtArea;
		[SerializeField]
		public UnityEngine.UI.Image ImgProgressBg;
		[SerializeField]
		public UnityEngine.UI.Image ImgProgress;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtImgprogress;
		[SerializeField]
		public UnityEngine.UI.Button BtnHeart;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtHeart;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTime;
		[SerializeField]
		public UnityEngine.UI.Button BtnCoin;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoin;
		[SerializeField]
		public UnityEngine.UI.Button BtnStar;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtStar;
		[SerializeField]
		public UnityEngine.UI.Button BtnHead;
		[SerializeField]
		public UnityEngine.UI.Image ImgHeadFrame;
		[SerializeField]
		public UnityEngine.UI.Button BtnVANode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtVolcanicActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnMSNode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtMagicStreakActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnRANode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRocketActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnHTANode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtHighTowerActivity;
		[SerializeField]
		public UnityEngine.Animator TxtCoinAdd;
		[SerializeField]
		public UnityEngine.UI.Button BtnSelect;
		[SerializeField]
		public UnityEngine.UI.Image ImgSelected;
		[SerializeField]
		public RectTransform BottomMenuBtns;
		
		private UIBeginData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ShopScrollView = null;
			BtnStart = null;
			TxtStart = null;
			BtnArea = null;
			TxtArea = null;
			ImgProgressBg = null;
			ImgProgress = null;
			TxtImgprogress = null;
			BtnHeart = null;
			TxtHeart = null;
			TxtTime = null;
			BtnCoin = null;
			TxtCoin = null;
			BtnStar = null;
			TxtStar = null;
			BtnHead = null;
			ImgHeadFrame = null;
			BtnVANode = null;
			TxtVolcanicActivity = null;
			BtnMSNode = null;
			TxtMagicStreakActivity = null;
			BtnRANode = null;
			TxtRocketActivity = null;
			BtnHTANode = null;
			TxtHighTowerActivity = null;
			TxtCoinAdd = null;
			BtnSelect = null;
			ImgSelected = null;
			BottomMenuBtns = null;
			
			mData = null;
		}
		
		public UIBeginData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIBeginData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIBeginData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
