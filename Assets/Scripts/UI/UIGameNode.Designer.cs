using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:c29c5466-87e2-4309-a708-1cc1c0179791
	public partial class UIGameNode
	{
		public const string Name = "UIGameNode";
		
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
		public UnityEngine.UI.Button BtnAddBottle;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtAddBottleNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddAddBottle;
		[SerializeField]
		public UnityEngine.UI.Button BtnHalfBottle;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtAddHalfBottleNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddHalfBottle;
		[SerializeField]
		public UnityEngine.UI.Button BtnRemoveAll;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRemoveAllNum;
		[SerializeField]
		public UnityEngine.UI.Image BtnAddRemoveBottle;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtLevel;
		[SerializeField]
		public UnityEngine.UI.Image ImgRankLevel;
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
		
		private UIGameNodeData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnStepBack = null;
			TxtRefreshNum = null;
			BtnAddStepBack = null;
			BtnRemoveHide = null;
			TxtRemoveHideNum = null;
			BtnAddRemove = null;
			BtnAddBottle = null;
			TxtAddBottleNum = null;
			BtnAddAddBottle = null;
			BtnHalfBottle = null;
			TxtAddHalfBottleNum = null;
			BtnAddHalfBottle = null;
			BtnRemoveAll = null;
			TxtRemoveAllNum = null;
			BtnAddRemoveBottle = null;
			TxtLevel = null;
			ImgRankLevel = null;
			BtnReturn = null;
			BtnItemBg = null;
			BtnItem1 = null;
			TxtItem1 = null;
			BtnItem2 = null;
			TxtItem2 = null;
			BtnItem3 = null;
			TxtItem3 = null;
			
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
