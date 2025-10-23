using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:28531646-ace1-4bab-915e-21913e7151c2
	public partial class UIGameNode
	{
		public const string Name = "UIGameNode";

        [Header("Bind UI")]

        [SerializeField]
		public TMPro.TextMeshProUGUI TxtLevel;
		[SerializeField]
		public UnityEngine.UI.Button BtnReturn;
		[SerializeField]
		public UnityEngine.UI.Image BtnItemBg;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem1;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtItem1;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem2;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtItem2;
		[SerializeField]
		public UnityEngine.UI.Button BtnItem3;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtItem3;
		[SerializeField]
		public UnityEngine.UI.Image ImgRankLevel;
		[SerializeField]
		public UnityEngine.UI.Text TxtRankLevel;
		[SerializeField]
		public UnityEngine.UI.Image ImgRankLevel_Label;
		[SerializeField]
		public UnityEngine.UI.Button BtnStepBack;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRefreshNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddStepBack;
		[SerializeField]
		public UnityEngine.UI.Button BtnRemoveHide;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRemoveHideNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddRemove;
		[SerializeField]
		public UnityEngine.UI.Button BtnHalfBottle;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtAddHalfBottleNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddHalfBottle;
		[SerializeField]
		public UnityEngine.UI.Button BtnAddBottle;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtAddBottleNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddAddBottle;
		[SerializeField]
		public UnityEngine.UI.Button BtnRemoveAll;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRemoveAllNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddRemoveBottle;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineRankPromotion;
		[SerializeField]
		public UnityEngine.UI.Image ImgRankSprite_mid;
		[SerializeField]
		public RectTransform FlightEffects;
		[SerializeField]
		public UnityEngine.GameObject LevelTipPanel;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextLevelTip;
		
		private UIGameNodeData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TxtLevel = null;
			BtnReturn = null;
			BtnItemBg = null;
			BtnItem1 = null;
			TxtItem1 = null;
			BtnItem2 = null;
			TxtItem2 = null;
			BtnItem3 = null;
			TxtItem3 = null;
			ImgRankLevel = null;
			TxtRankLevel = null;
			ImgRankLevel_Label = null;
			BtnStepBack = null;
			TxtRefreshNum = null;
			BtnAddStepBack = null;
			BtnRemoveHide = null;
			TxtRemoveHideNum = null;
			BtnAddRemove = null;
			BtnHalfBottle = null;
			TxtAddHalfBottleNum = null;
			BtnAddHalfBottle = null;
			BtnAddBottle = null;
			TxtAddBottleNum = null;
			BtnAddAddBottle = null;
			BtnRemoveAll = null;
			TxtRemoveAllNum = null;
			BtnAddRemoveBottle = null;
			SpineRankPromotion = null;
			ImgRankSprite_mid = null;
			FlightEffects = null;
			LevelTipPanel = null;
			TextLevelTip = null;
			
			mData = null;
		}
		
		public UIGameNodeData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameNodeData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameNodeData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
