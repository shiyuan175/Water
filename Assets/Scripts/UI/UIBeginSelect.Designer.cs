using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:4befb7c9-0d3f-4fba-8814-8f227d050297
	public partial class UIBeginSelect
	{
		public const string Name = "UIBeginSelect";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtLevelTitle;
		[SerializeField]
		public UnityEngine.UI.Button BtnInfo;
		[SerializeField]
		public UnityEngine.UI.Image ImgProgress;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtProgress;
		[SerializeField]
		public UnityEngine.UI.Image Mask;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		[SerializeField]
		public UnityEngine.UI.Image ContinueWinGoalNode;
		[SerializeField]
		public UnityEngine.UI.Image Goal;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtWinProcess;
		[SerializeField]
		public UnityEngine.UI.Image BuffTag;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCoinBuffTimer;
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
			BtnInfo = null;
			ImgProgress = null;
			TxtProgress = null;
			Mask = null;
			BtnStart = null;
			ContinueWinGoalNode = null;
			Goal = null;
			TxtWinProcess = null;
			BuffTag = null;
			TxtCoinBuffTimer = null;
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
