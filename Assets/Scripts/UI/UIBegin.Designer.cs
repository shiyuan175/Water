using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:24d2f144-59bd-43ae-b7f6-9d42fb9cd2de
	public partial class UIBegin
	{
		public const string Name = "UIBegin";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect ShopScrollView;
		[SerializeField]
		public UnityEngine.GameObject BattlePassContent;
		[SerializeField]
		public UnityEngine.GameObject ImgGiftFree;
		[SerializeField]
		public UnityEngine.GameObject ImgGiftVip;
		[SerializeField]
		public UnityEngine.UI.Image ImgProgressBar;
		[SerializeField]
		public UnityEngine.UI.Image ImgLevel;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextLevel;
		[SerializeField]
		public UnityEngine.UI.Image ImgButtomDividingLine;
		[SerializeField]
		public UnityEngine.UI.Image ImgTopDividingLine;
		[SerializeField]
		public UnityEngine.UI.Button BtnInfo;
		[SerializeField]
		public UnityEngine.UI.Image ImgBar;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextTaskProgressBar;
		[SerializeField]
		public UnityEngine.UI.Button BtnActivate;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtStartLevel;
		[SerializeField]
		public UnityEngine.UI.Image AnimStartFlash;
		[SerializeField]
		public UnityEngine.UI.Image ImgDoubleBuff;
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
		public UnityEngine.UI.Button BtnMSANode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtMagicStreakActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnTRANode;
		[SerializeField]
		public UnityEngine.UI.Image ImgTierRankIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTierRankActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnRANode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRocketActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnHTANode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtHighTowerActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnTTNode;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtTTActivity;
		[SerializeField]
		public UnityEngine.UI.Button BtnSelect;
		[SerializeField]
		public UnityEngine.UI.Image ImgSelected;
		[SerializeField]
		public UnityEngine.UI.ScrollRect ScrollView;
		[SerializeField]
		public RectTransform BottomMenuBtns;
		
		private UIBeginData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ShopScrollView = null;
			BattlePassContent = null;
			ImgGiftFree = null;
			ImgGiftVip = null;
			ImgProgressBar = null;
			ImgLevel = null;
			TextLevel = null;
			ImgButtomDividingLine = null;
			ImgTopDividingLine = null;
			BtnInfo = null;
			ImgBar = null;
			TextTaskProgressBar = null;
			BtnActivate = null;
			TxtCountDown = null;
			BtnStart = null;
			TxtStartLevel = null;
			AnimStartFlash = null;
			ImgDoubleBuff = null;
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
			TxtStar = null;
			BtnHead = null;
			ImgHeadFrame = null;
			BtnVANode = null;
			TxtVolcanicActivity = null;
			BtnMSANode = null;
			TxtMagicStreakActivity = null;
			BtnTRANode = null;
			ImgTierRankIcon = null;
			TxtTierRankActivity = null;
			BtnRANode = null;
			TxtRocketActivity = null;
			BtnHTANode = null;
			TxtHighTowerActivity = null;
			BtnTTNode = null;
			TxtTTActivity = null;
			BtnSelect = null;
			ImgSelected = null;
			ScrollView = null;
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
