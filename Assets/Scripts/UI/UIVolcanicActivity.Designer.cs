using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:58154bb6-e18c-4d30-abb3-760a369bb412
	public partial class UIVolcanicActivity
	{
		public const string Name = "UIVolcanicActivity";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Prompt;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtLevels;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Levels;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtPlayers;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Players;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtCountDown;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtDailyRefresh;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic Spine_rongyanpaopao;
		[SerializeField]
		public RectTransform HeadNodesPar;
		
		private UIVolcanicActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			Txt_Prompt = null;
			TxtLevels = null;
			Txt_Levels = null;
			TxtPlayers = null;
			Txt_Players = null;
			TxtCountDown = null;
			TxtDailyRefresh = null;
			Spine_rongyanpaopao = null;
			HeadNodesPar = null;
			
			mData = null;
		}
		
		public UIVolcanicActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIVolcanicActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIVolcanicActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
