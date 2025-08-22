using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:6145dde4-5b7d-4ced-9857-be93099295fe
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
