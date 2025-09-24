using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:1b8dbc4f-6d0a-4ae5-afe4-cc867bd95e33
	public partial class UIBeginSelect
	{
		public const string Name = "UIBeginSelect";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtLevelTitle;
		[SerializeField]
		public UnityEngine.UI.Image ImgBox;
		[SerializeField]
		public UnityEngine.UI.Button BtnInfo;
		[SerializeField]
		public UnityEngine.UI.Image ImgProgress;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtProgress;
		[SerializeField]
		public RectTransform UnLimitNode;
		[SerializeField]
		public UnityEngine.UI.Image ImgReward;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		[SerializeField]
		public UnityEngine.UI.Image ContinueWinGoalNode;
		[SerializeField]
		public UnityEngine.UI.Image Goal;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtWinProcess;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextGoldCoin;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextGoldCoinBuff;
		[SerializeField]
		public RectTransform ItemGuidePanel;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHandleItem;
		[SerializeField]
		public RectTransform GoldCoinGuidePanel;
		[SerializeField]
		public UnityEngine.UI.Button BtnGoldGuide;
		
		private UIBeginSelectData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			TxtLevelTitle = null;
			ImgBox = null;
			BtnInfo = null;
			ImgProgress = null;
			TxtProgress = null;
			UnLimitNode = null;
			ImgReward = null;
			BtnStart = null;
			ContinueWinGoalNode = null;
			Goal = null;
			TxtWinProcess = null;
			TextGoldCoin = null;
			TextGoldCoinBuff = null;
			ItemGuidePanel = null;
			SpineHandleItem = null;
			GoldCoinGuidePanel = null;
			BtnGoldGuide = null;
			
			mData = null;
		}
		
		public UIBeginSelectData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIBeginSelectData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIBeginSelectData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
